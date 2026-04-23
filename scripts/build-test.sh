#!/usr/bin/env bash
# scripts/build-test.sh
# Smoke test: verifies expected export artefacts exist.
# Usage: bash scripts/build-test.sh <platform>
#   platform: windows | linux | macos

set -euo pipefail

PLATFORM="${1:-}"

assert_file_exists() {
    if [[ ! -f "$1" ]]; then
        echo "MISSING: $1"
        exit 1
    fi
    echo "OK: $1"
}

case "$PLATFORM" in
    windows)
        assert_file_exists "export/windows/EchoForest.exe"
        ;;
    linux)
        assert_file_exists "export/linux/EchoForest.x86_64"
        ;;
    macos)
        assert_file_exists "export/macos/EchoForest.zip"
        ;;
    "")
        # Run all checks (useful for local verification after manual export)
        assert_file_exists "export/windows/EchoForest.exe"
        assert_file_exists "export/linux/EchoForest.x86_64"
        assert_file_exists "export/macos/EchoForest.zip"
        ;;
    *)
        echo "Unknown platform: $PLATFORM"
        echo "Usage: $0 [windows|linux|macos]"
        exit 1
        ;;
esac

echo "Build smoke test passed for: ${PLATFORM:-all}"
