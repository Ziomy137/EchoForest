# setup-godot-path.ps1 — Locates the Godot 4 Mono binary and sets GODOT_PATH
# as a persistent user environment variable on Windows.
#
# Usage (from repo root):
#   powershell -ExecutionPolicy Bypass -File scripts\setup-godot-path.ps1
#
# To specify a path directly:
#   powershell -ExecutionPolicy Bypass -File scripts\setup-godot-path.ps1 -GodotBin "C:\Godot\Godot.exe"

param(
    [string]$GodotBin = ""
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# ── Colour helpers ────────────────────────────────────────────────────────────
function Write-Info    { param($msg) Write-Host "[INFO]  $msg" -ForegroundColor Cyan }
function Write-Ok      { param($msg) Write-Host "[OK]    $msg" -ForegroundColor Green }
function Write-Warn    { param($msg) Write-Host "[WARN]  $msg" -ForegroundColor Yellow }
function Write-Err     { param($msg) Write-Host "[ERROR] $msg" -ForegroundColor Red }

# ── Known search paths ────────────────────────────────────────────────────────
$Candidates = @(
    # Godot 4.3 Mono — common Windows install locations
    "C:\Godot\Godot_v4.3-stable_mono_win64.exe"
    "C:\Godot\Godot_v4.3_mono_win64.exe"
    "C:\Godot\Godot.exe"
    "$env:LOCALAPPDATA\Programs\Godot\Godot_mono.exe"
    "$env:LOCALAPPDATA\Programs\Godot\Godot.exe"
    "$env:ProgramFiles\Godot\Godot_mono.exe"
    "$env:ProgramFiles\Godot\Godot.exe"
    "${env:ProgramFiles(x86)}\Godot\Godot_mono.exe"
    # Scoop
    "$env:USERPROFILE\scoop\apps\godot4-mono\current\godot4-mono.exe"
    "$env:USERPROFILE\scoop\apps\godot4\current\godot4.exe"
    # Chocolatey
    "$env:ChocolateyInstall\bin\godot4.exe"
    # Common manual download locations
    "$env:USERPROFILE\Downloads\Godot_v4.3-stable_mono_win64\Godot_v4.3-stable_mono_win64.exe"
    "$env:USERPROFILE\Godot\Godot_mono.exe"
    "$env:USERPROFILE\Godot\Godot.exe"
)

# ── Auto-detect ───────────────────────────────────────────────────────────────
if ([string]::IsNullOrWhiteSpace($GodotBin)) {
    Write-Info "Searching for Godot 4 binary..."

    foreach ($candidate in $Candidates) {
        if (Test-Path $candidate -PathType Leaf) {
            $GodotBin = $candidate
            Write-Info "Found: $GodotBin"
            break
        }
    }
}

# ── Fallback: PATH ────────────────────────────────────────────────────────────
if ([string]::IsNullOrWhiteSpace($GodotBin)) {
    $fromPath = Get-Command "godot4" -ErrorAction SilentlyContinue
    if (-not $fromPath) {
        $fromPath = Get-Command "godot" -ErrorAction SilentlyContinue
    }
    if ($fromPath) {
        $GodotBin = $fromPath.Source
        Write-Info "Found in PATH: $GodotBin"
    }
}

# ── Fallback: filesystem search ───────────────────────────────────────────────
if ([string]::IsNullOrWhiteSpace($GodotBin)) {
    Write-Info "Searching common drives (C:\, D:\) — this may take a moment..."
    $searchRoots = @("C:\", "D:\") | Where-Object { Test-Path $_ }
    foreach ($root in $searchRoots) {
        $found = Get-ChildItem -Path $root -Filter "Godot*mono*.exe" -Recurse `
            -ErrorAction SilentlyContinue -Depth 5 | Select-Object -First 1
        if (-not $found) {
            $found = Get-ChildItem -Path $root -Filter "Godot*.exe" -Recurse `
                -ErrorAction SilentlyContinue -Depth 5 |
                Where-Object { $_.FullName -imatch "4\." } | Select-Object -First 1
        }
        if ($found) {
            $GodotBin = $found.FullName
            Write-Info "Found: $GodotBin"
            break
        }
    }
}

# ── Prompt if still not found ─────────────────────────────────────────────────
if ([string]::IsNullOrWhiteSpace($GodotBin)) {
    Write-Warn "Godot 4 binary not found automatically."
    Write-Host ""
    Write-Host "  Download from: https://godotengine.org/download/windows/" -ForegroundColor White
    Write-Host "  Get the 'Mono' version (C# support required)" -ForegroundColor Yellow
    Write-Host ""
    $GodotBin = Read-Host "  Enter full path to Godot binary (or press Enter to abort)"
    if ([string]::IsNullOrWhiteSpace($GodotBin)) {
        Write-Err "Aborted. Re-run after installing Godot 4."
        exit 1
    }
}

# ── Validate ──────────────────────────────────────────────────────────────────
if (-not (Test-Path $GodotBin -PathType Leaf)) {
    Write-Err "File not found: $GodotBin"
    exit 1
}

# ── Version check (non-fatal) ─────────────────────────────────────────────────
try {
    $version = & $GodotBin --version 2>&1 | Select-Object -First 1
    if ($version -match "^4\.") {
        Write-Ok "Godot version: $version"
    } else {
        Write-Warn "Version check returned unexpected output: $version"
    }
} catch {
    Write-Warn "Could not determine Godot version. Continuing anyway."
}

# ── Set User environment variable (persistent, no admin required) ─────────────
$existing = [System.Environment]::GetEnvironmentVariable("GODOT_PATH", "User")
if ($existing -eq $GodotBin) {
    Write-Ok "GODOT_PATH already set to the correct value — no change needed."
} else {
    if (-not [string]::IsNullOrWhiteSpace($existing)) {
        Write-Warn "Updating existing GODOT_PATH (was: $existing)"
    }
    [System.Environment]::SetEnvironmentVariable("GODOT_PATH", $GodotBin, "User")
    Write-Ok "GODOT_PATH written to User environment variables."
}

# ── Apply to current session ──────────────────────────────────────────────────
$env:GODOT_PATH = $GodotBin

# ── Done ──────────────────────────────────────────────────────────────────────
Write-Host ""
Write-Ok "GODOT_PATH = $GodotBin"
Write-Host ""
Write-Host "  The variable is now set for your user account (persists across reboots)." -ForegroundColor White
Write-Host "  Restart VS Code (or any open terminals) to pick it up." -ForegroundColor Yellow
Write-Host ""
Write-Host "  Verify with:  " -NoNewline
Write-Host '[System.Environment]::GetEnvironmentVariable("GODOT_PATH","User")' -ForegroundColor Cyan
Write-Host ""
