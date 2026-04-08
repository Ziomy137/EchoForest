#!/usr/bin/env bash
# setup-godot-path.sh - Locates the Godot 4 Mono binary and adds GODOT_PATH
# to your shell profile on macOS and Linux.
#
# Usage:
#   chmod +x scripts/setup-godot-path.sh
#   ./scripts/setup-godot-path.sh
#
# To use a custom path:
#   GODOT_BIN="/path/to/Godot" ./scripts/setup-godot-path.sh

set -eu

# --- Colour helpers ---
RED='\033[0;31m'; GREEN='\033[0;32m'; YELLOW='\033[1;33m'
CYAN='\033[0;36m'; BOLD='\033[1m'; RESET='\033[0m'

info()    { echo -e "${CYAN}[INFO]${RESET}  $*"; }
success() { echo -e "${GREEN}[OK]${RESET}    $*"; }
warn()    { echo -e "${YELLOW}[WARN]${RESET}  $*"; }
error()   { echo -e "${RED}[ERROR]${RESET} $*" >&2; }

# --- Detect OS ---
OS="$(uname -s)"

# --- Known search paths ---
CANDIDATES=()

if [[ "$OS" == "Darwin" ]]; then
    # macOS - .app bundles and common manual install locations
    CANDIDATES=(
        "/Applications/Godot_mono.app/Contents/MacOS/Godot"
        "/Applications/Godot.app/Contents/MacOS/Godot"
        "/Applications/Godot_v4.6.2_mono.app/Contents/MacOS/Godot"
        "/Applications/Godot_v4.6.2-stable_mono.app/Contents/MacOS/Godot"
        "$HOME/Applications/Godot_mono.app/Contents/MacOS/Godot"
        "$HOME/Applications/Godot.app/Contents/MacOS/Godot"
        "/opt/homebrew/bin/godot4"
        "/usr/local/bin/godot4"
    )
elif [[ "$OS" == "Linux" ]]; then
    GODOT4_CMD="$(command -v godot4 2>/dev/null || true)"
    GODOT_CMD="$(command -v godot 2>/dev/null || true)"
    CANDIDATES=(
        "/usr/bin/godot4"
        "/usr/local/bin/godot4"
        "/opt/godot4/godot4"
        "/opt/godot/godot"
        "$HOME/.local/bin/godot4"
        "$HOME/godot/godot4"
        "/snap/bin/godot4"
        "${GODOT4_CMD}"
        "${GODOT_CMD}"
    )
else
    error "Unsupported OS: $OS. Use setup-godot-path.ps1 on Windows."
    exit 1
fi

# --- Try to auto-detect ---
GODOT_BIN="${GODOT_BIN:-}"

if [[ -z "$GODOT_BIN" ]]; then
    info "Searching for Godot 4 binary..."
    for candidate in "${CANDIDATES[@]}"; do
        if [[ -n "$candidate" && -x "$candidate" ]]; then
            GODOT_BIN="$candidate"
            info "Found: $GODOT_BIN"
            break
        fi
    done
fi

# --- Fallback: mdfind (macOS Spotlight) ---
if [[ -z "$GODOT_BIN" && "$OS" == "Darwin" ]]; then
    info "Trying Spotlight search..."
    SPOTLIGHT="$(mdfind "kMDItemKind == 'Application' && kMDItemFSName == 'Godot*'" 2>/dev/null | grep -i mono | head -1 || true)"
    if [[ -n "$SPOTLIGHT" ]]; then
        CANDIDATE="$SPOTLIGHT/Contents/MacOS/Godot"
        if [[ -x "$CANDIDATE" ]]; then
            GODOT_BIN="$CANDIDATE"
            info "Found via Spotlight: $GODOT_BIN"
        fi
    fi
fi

# --- Fallback: find (Linux) ---
if [[ -z "$GODOT_BIN" && "$OS" == "Linux" ]]; then
    info "Searching filesystem (this may take a moment)..."
    FOUND="$(find /opt /usr/local "$HOME" -maxdepth 5 -name "godot*" -type f -executable 2>/dev/null | grep -i "4\." | head -1 || true)"
    if [[ -n "$FOUND" ]]; then
        GODOT_BIN="$FOUND"
        info "Found: $GODOT_BIN"
    fi
fi

# --- Prompt if still not found ---
if [[ -z "$GODOT_BIN" ]]; then
    warn "Godot 4 binary not found automatically."
    echo ""
    if [[ "$OS" == "Darwin" ]]; then
        echo -e "  Download from: ${BOLD}https://godotengine.org/download/macos/${RESET}"
        echo -e "  Expected path: ${BOLD}/Applications/Godot_mono.app/Contents/MacOS/Godot${RESET}"
    else
        echo -e "  Download from: ${BOLD}https://godotengine.org/download/linux/${RESET}"
        echo -e "  Expected path: ${BOLD}/usr/local/bin/godot4${RESET}"
    fi
    echo ""
    read -rp "  Enter full path to Godot binary (or press Enter to abort): " USER_PATH
    if [[ -z "$USER_PATH" ]]; then
        error "Aborted. Re-run after installing Godot 4."
        exit 1
    fi
    GODOT_BIN="$USER_PATH"
fi

# --- Validate ---
if [[ ! -x "$GODOT_BIN" ]]; then
    error "Not executable: $GODOT_BIN"
    exit 1
fi

# Try to confirm it is actually Godot 4
VERSION_OUTPUT="$("$GODOT_BIN" --version 2>&1 | head -1 || true)"
if echo "$VERSION_OUTPUT" | grep -qE "^4\."; then
    success "Godot version: $VERSION_OUTPUT"
else
    warn "Binary found but version check returned: $VERSION_OUTPUT. Continuing anyway."
fi

# --- Determine shell profile ---
EXPORT_LINE="export GODOT_PATH=\"$GODOT_BIN\""

if [[ "$OS" == "Darwin" ]]; then
    # macOS default since Catalina is zsh
    PROFILE="$HOME/.zshrc"
    if [[ -n "${BASH_VERSION:-}" ]]; then
        PROFILE="$HOME/.bash_profile"
    fi
else
    PROFILE="$HOME/.bashrc"
    if [[ "${SHELL:-}" == *zsh* ]]; then
        PROFILE="$HOME/.zshrc"
    fi
fi

# --- Write to profile (idempotent) ---
if grep -qF "GODOT_PATH=" "$PROFILE" 2>/dev/null; then
    warn "GODOT_PATH already set in $PROFILE - updating it."
    # Replace existing line
    if [[ "$OS" == "Darwin" ]]; then
        sed -i '' "s|.*GODOT_PATH=.*|$EXPORT_LINE|" "$PROFILE"
    else
        sed -i "s|.*GODOT_PATH=.*|$EXPORT_LINE|" "$PROFILE"
    fi
else
    {
        echo ""
        echo "# Godot 4 binary path (added by EchoForest/scripts/setup-godot-path.sh)"
        echo "$EXPORT_LINE"
    } >> "$PROFILE"
fi

# --- Apply to current session ---
export GODOT_PATH="$GODOT_BIN"

# --- Done ---
echo ""
success "GODOT_PATH set to: $GODOT_BIN"
success "Written to: $PROFILE"
echo ""
echo -e "  Reload your shell or run: ${BOLD}source $PROFILE${RESET}"
echo -e "  Then verify with:         ${BOLD}echo \$GODOT_PATH${RESET}"
echo ""
