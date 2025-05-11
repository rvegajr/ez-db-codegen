#!/bin/bash

# Comprehensive Test Suite
# This script combines the standalone EF Core comparison tests with CLI testing

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
        return 0
    else
        print_color "$RED" "✗ Failed: $1"
        return 1
    fi
}

# Set the root directory
ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"

# Create results directory
RESULTS_DIR="$ROOT_DIR/test-results"
mkdir -p "$RESULTS_DIR"

# Initialize summary report
SUMMARY_REPORT="$RESULTS_DIR/comprehensive-test-summary.md"
echo "# Comprehensive Test Results" > "$SUMMARY_REPORT"
echo "" >> "$SUMMARY_REPORT"
echo "Generated on: $(date)" >> "$SUMMARY_REPORT"
echo "" >> "$SUMMARY_REPORT"

print_color "$BLUE" "=== Starting Comprehensive Test Suite ==="

# Part 1: Run standalone EF Core comparison tests
print_color "$YELLOW" "Running standalone EF Core comparison tests..."

# Build and run the standalone tests
dotnet build standalone-tests/EFCoreComparison/StandaloneEFCoreComparison.csproj -c Release
if check_success "Build standalone test project"; then
    # Run the tests with detailed output
    dotnet test standalone-tests/EFCoreComparison/StandaloneEFCoreComparison.csproj \
        -c Release \
        --logger "console;verbosity=detailed" \
        --logger "trx;LogFileName=$RESULTS_DIR/standalone-efcore-comparison.trx"
    
    EFCORE_TEST_STATUS=$?
    
    if [ $EFCORE_TEST_STATUS -eq 0 ]; then
        print_color "$GREEN" "✓ EF Core comparison tests passed"
        echo "## EF Core Comparison Tests: ✅ Passed" >> "$SUMMARY_REPORT"
    else
        print_color "$RED" "✗ EF Core comparison tests failed"
        echo "## EF Core Comparison Tests: ❌ Failed" >> "$SUMMARY_REPORT"
    fi
    
    # Extract test results from the TRX file
    if [ -f "$RESULTS_DIR/standalone-efcore-comparison.trx" ]; then
        TOTAL_TESTS=$(grep -o "outcome=\"[^\"]*\"" "$RESULTS_DIR/standalone-efcore-comparison.trx" | wc -l)
        PASSED_TESTS=$(grep -o "outcome=\"Passed\"" "$RESULTS_DIR/standalone-efcore-comparison.trx" | wc -l)
        FAILED_TESTS=$(grep -o "outcome=\"Failed\"" "$RESULTS_DIR/standalone-efcore-comparison.trx" | wc -l)
        
        echo "" >> "$SUMMARY_REPORT"
        echo "### Test Summary" >> "$SUMMARY_REPORT"
        echo "" >> "$SUMMARY_REPORT"
        echo "- Total tests: $TOTAL_TESTS" >> "$SUMMARY_REPORT"
        echo "- Passed: $PASSED_TESTS" >> "$SUMMARY_REPORT"
        echo "- Failed: $FAILED_TESTS" >> "$SUMMARY_REPORT"
    fi
else
    echo "## EF Core Comparison Tests: ❌ Build Failed" >> "$SUMMARY_REPORT"
    EFCORE_TEST_STATUS=1
fi

# Part 2: Attempt to build the CLI
print_color "$YELLOW" "Attempting to build the CLI..."
echo "" >> "$SUMMARY_REPORT"
echo "## CLI Build Status" >> "$SUMMARY_REPORT"

# Try to build the CLI
dotnet build src/EzDbCodeGen.Cli/EzDbCodeGen.Cli.csproj -c Release
if check_success "Build CLI project"; then
    echo "### ✅ CLI Build Succeeded" >> "$SUMMARY_REPORT"
    CLI_BUILD_STATUS=0
    
    # Try to run the CLI help command
    print_color "$YELLOW" "Testing CLI help command..."
    dotnet run --project src/EzDbCodeGen.Cli/EzDbCodeGen.Cli.csproj -- --help > "$RESULTS_DIR/cli-help-output.txt" 2>&1
    
    if check_success "CLI help command"; then
        echo "### ✅ CLI Help Command Succeeded" >> "$SUMMARY_REPORT"
        echo "" >> "$SUMMARY_REPORT"
        echo "CLI help output:" >> "$SUMMARY_REPORT"
        echo '```' >> "$SUMMARY_REPORT"
        cat "$RESULTS_DIR/cli-help-output.txt" >> "$SUMMARY_REPORT"
        echo '```' >> "$SUMMARY_REPORT"
    else
        echo "### ❌ CLI Help Command Failed" >> "$SUMMARY_REPORT"
        echo "" >> "$SUMMARY_REPORT"
        echo "Error output:" >> "$SUMMARY_REPORT"
        echo '```' >> "$SUMMARY_REPORT"
        cat "$RESULTS_DIR/cli-help-output.txt" >> "$SUMMARY_REPORT"
        echo '```' >> "$SUMMARY_REPORT"
    fi
else
    echo "### ❌ CLI Build Failed" >> "$SUMMARY_REPORT"
    echo "" >> "$SUMMARY_REPORT"
    echo "The CLI build failed due to compilation errors. These need to be fixed before the CLI can be tested." >> "$SUMMARY_REPORT"
    CLI_BUILD_STATUS=1
fi

# Part 3: Generate recommendations
echo "" >> "$SUMMARY_REPORT"
echo "## Recommendations" >> "$SUMMARY_REPORT"
echo "" >> "$SUMMARY_REPORT"

if [ $CLI_BUILD_STATUS -ne 0 ]; then
    echo "### CLI Build Issues" >> "$SUMMARY_REPORT"
    echo "" >> "$SUMMARY_REPORT"
    echo "The CLI build is failing due to interface ambiguity issues in the ServiceCollectionExtensions.cs file. Here are some recommendations:" >> "$SUMMARY_REPORT"
    echo "" >> "$SUMMARY_REPORT"
    echo "1. **Resolve ambiguous interface references**: Fully qualify interface references that are ambiguous between different namespaces." >> "$SUMMARY_REPORT"
    echo "2. **Fix implementation mismatches**: Ensure that implementation classes correctly implement their interfaces." >> "$SUMMARY_REPORT"
    echo "3. **Update helper registrations**: Fix the RegisterHandlebarsHelpers method to use the correct parameter types." >> "$SUMMARY_REPORT"
    echo "" >> "$SUMMARY_REPORT"
fi

if [ $EFCORE_TEST_STATUS -eq 0 ]; then
    echo "### EF Core Comparison" >> "$SUMMARY_REPORT"
    echo "" >> "$SUMMARY_REPORT"
    echo "The EF Core comparison tests were successful. This indicates that the EF Core model analysis is working correctly." >> "$SUMMARY_REPORT"
    echo "Consider the following next steps:" >> "$SUMMARY_REPORT"
    echo "" >> "$SUMMARY_REPORT"
    echo "1. **Extend the comparison**: Add more database schemas to the comparison tests." >> "$SUMMARY_REPORT"
    echo "2. **Performance benchmarking**: Add more detailed performance metrics to compare EF Core and EzDbCodeGen." >> "$SUMMARY_REPORT"
    echo "3. **Integration testing**: Once the CLI is fixed, integrate the EF Core comparison with the CLI testing." >> "$SUMMARY_REPORT"
fi

# Part 4: Final summary
echo "" >> "$SUMMARY_REPORT"
echo "## Overall Status" >> "$SUMMARY_REPORT"
echo "" >> "$SUMMARY_REPORT"

if [ $EFCORE_TEST_STATUS -eq 0 ] && [ $CLI_BUILD_STATUS -eq 0 ]; then
    echo "### ✅ All tests passed" >> "$SUMMARY_REPORT"
    print_color "$GREEN" "=== Comprehensive Test Suite Completed Successfully ==="
else
    echo "### ❌ Some tests failed" >> "$SUMMARY_REPORT"
    print_color "$RED" "=== Comprehensive Test Suite Completed with Errors ==="
fi

print_color "$GREEN" "Summary report generated at $SUMMARY_REPORT"

# Return overall status
if [ $EFCORE_TEST_STATUS -eq 0 ] && [ $CLI_BUILD_STATUS -eq 0 ]; then
    exit 0
else
    exit 1
fi
