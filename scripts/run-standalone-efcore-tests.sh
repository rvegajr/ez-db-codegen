#!/bin/bash

# Standalone EF Core Comparison Test Script
# This script runs the standalone EF Core comparison tests and generates a summary report

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

print_color "$BLUE" "=== Starting Standalone EF Core Comparison Tests ==="

# Step 1: Build the standalone test project
print_color "$YELLOW" "Building standalone test project..."
dotnet build standalone-tests/EFCoreComparison/StandaloneEFCoreComparison.csproj -c Release
check_success "Build standalone test project"

# Step 2: Run the standalone tests
print_color "$YELLOW" "Running standalone EF Core comparison tests..."
RESULTS_DIR="$ROOT_DIR/test-results"
mkdir -p "$RESULTS_DIR"

# Run the tests with detailed output
dotnet test standalone-tests/EFCoreComparison/StandaloneEFCoreComparison.csproj \
    -c Release \
    --logger "console;verbosity=detailed" \
    --logger "trx;LogFileName=$RESULTS_DIR/standalone-efcore-comparison.trx"
TEST_EXIT_CODE=$?

# Step 3: Generate a summary report
print_color "$YELLOW" "Generating summary report..."
REPORT_FILE="$RESULTS_DIR/standalone-efcore-comparison-summary.md"

echo "# Standalone EF Core Comparison Test Results" > "$REPORT_FILE"
echo "" >> "$REPORT_FILE"
echo "Generated on: $(date)" >> "$REPORT_FILE"
echo "" >> "$REPORT_FILE"

if [ $TEST_EXIT_CODE -eq 0 ]; then
    echo "## ✅ All tests passed" >> "$REPORT_FILE"
else
    echo "## ❌ Some tests failed" >> "$REPORT_FILE"
fi

echo "" >> "$REPORT_FILE"
echo "## Test Summary" >> "$REPORT_FILE"
echo "" >> "$REPORT_FILE"

# Extract test results from the TRX file
if [ -f "$RESULTS_DIR/standalone-efcore-comparison.trx" ]; then
    TOTAL_TESTS=$(grep -o "outcome=\"[^\"]*\"" "$RESULTS_DIR/standalone-efcore-comparison.trx" | wc -l)
    PASSED_TESTS=$(grep -o "outcome=\"Passed\"" "$RESULTS_DIR/standalone-efcore-comparison.trx" | wc -l)
    FAILED_TESTS=$(grep -o "outcome=\"Failed\"" "$RESULTS_DIR/standalone-efcore-comparison.trx" | wc -l)
    
    echo "- Total tests: $TOTAL_TESTS" >> "$REPORT_FILE"
    echo "- Passed: $PASSED_TESTS" >> "$REPORT_FILE"
    echo "- Failed: $FAILED_TESTS" >> "$REPORT_FILE"
    echo "" >> "$REPORT_FILE"
    
    # Extract test durations
    echo "## Test Durations" >> "$REPORT_FILE"
    echo "" >> "$REPORT_FILE"
    echo "| Test | Duration (ms) |" >> "$REPORT_FILE"
    echo "|------|--------------|" >> "$REPORT_FILE"
    
    # This is a simplified approach - a more robust solution would use XML parsing
    grep -A 2 "<UnitTestResult" "$RESULTS_DIR/standalone-efcore-comparison.trx" | grep -E "testName|duration" | \
    paste -d "|" - - | sed 's/.*testName="\([^"]*\)".*/\1/' | sed 's/.*duration="\([^"]*\)".*/\1/' | \
    awk -F'|' '{print "| " $1 " | " $2 " |"}' >> "$REPORT_FILE"
fi

print_color "$GREEN" "Summary report generated at $REPORT_FILE"

# Display the final status
if [ $TEST_EXIT_CODE -eq 0 ]; then
    print_color "$GREEN" "=== Standalone EF Core Comparison Tests Completed Successfully ==="
else
    print_color "$RED" "=== Standalone EF Core Comparison Tests Completed with Errors ==="
    exit $TEST_EXIT_CODE
fi
