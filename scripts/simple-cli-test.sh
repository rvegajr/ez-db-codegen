#!/bin/bash

# Simple CLI Test Script
# This script tests basic CLI functionality without requiring a full build

# Set colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[0;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored text
print_color() {
    local color=$1
    local text=$2
    echo -e "${color}${text}${NC}"
}

# Function to check if a command succeeded
check_success() {
    if [ $? -eq 0 ]; then
        print_color "$GREEN" "✓ Success: $1"
    else
        print_color "$RED" "✗ Failed: $1"
        print_color "$RED" "Stopping script due to error"
        exit 1
    fi
}

# Set the root directory
ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"

print_color "$BLUE" "=== Starting Simple CLI Test ==="

# Step 1: Build the CLI project with dotnet publish (more reliable than just build)
print_color "$YELLOW" "Building CLI project..."
dotnet publish -c Release -o ./publish/cli src/EzDbCodeGen.Cli/EzDbCodeGen.Cli.csproj
check_success "Build CLI project"

# Step 2: Run a simple help command to verify the CLI works
print_color "$YELLOW" "Testing CLI help command..."
./publish/cli/EzDbCodeGen.Cli --help
check_success "CLI help command"

print_color "$BLUE" "=== Simple CLI Test Completed Successfully ==="
