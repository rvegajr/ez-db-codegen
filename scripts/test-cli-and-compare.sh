#!/bin/bash
set -e

# Configuration
BASE_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
CLI_PROJECT_PATH="$BASE_DIR/src/EzDbCodeGen.Cli"
TEST_PROJECT_PATH="$BASE_DIR/tests/EzDbCodeGen.EFCoreComparison.Tests"
OUTPUT_DIR="$BASE_DIR/output"
TEMPLATE_DIR="$BASE_DIR/templates"
CONNECTION_STRING_BASE="Server=localhost;User Id=sa;Password=APADemo123!;TrustServerCertificate=True;"

# Sample databases to test
DATABASES=("Northwind" "AdventureWorks" "WideWorldImporters" "ContosoDataWarehouse")

# Colors for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored messages
print_message() {
    local color=$1
    local message=$2
    echo -e "${color}${message}${NC}"
}

# Function to run a command and print its output
run_command() {
    local command=$1
    local description=$2
    
    print_message "$BLUE" "Running: $description"
    print_message "$YELLOW" "$command"
    
    if eval "$command"; then
        print_message "$GREEN" "✓ Command succeeded"
    else
        print_message "$RED" "✗ Command failed with exit code $?"
        exit 1
    fi
    
    echo ""
}

# Create output directory if it doesn't exist
mkdir -p "$OUTPUT_DIR"

# Build the CLI project
run_command "dotnet build $CLI_PROJECT_PATH -c Release" "Building CLI project"

# Build the test project
run_command "dotnet build $TEST_PROJECT_PATH -c Release" "Building test project"

# Check if template directory exists, create it if not
if [ ! -d "$TEMPLATE_DIR" ]; then
    mkdir -p "$TEMPLATE_DIR"
    
    # Create a simple entity template
    cat > "$TEMPLATE_DIR/entity.hbs" << 'EOF'
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace {{namespace}}
{
    [Table("{{table.Name}}")]
    public class {{table.Name}}
    {
        {{#each table.Columns}}
        {{#if IsPrimaryKey}}
        [Key]
        {{/if}}
        {{#if IsRequired}}
        [Required]
        {{/if}}
        {{#if MaxLength}}
        [MaxLength({{MaxLength}})]
        {{/if}}
        [Column("{{Name}}")]
        public {{CSharpType}} {{Name}} { get; set; }
        {{/each}}

        {{#each table.ForeignKeys}}
        public virtual {{ReferencedTable.Name}} {{ReferencedTable.Name}} { get; set; }
        {{/each}}
    }
}
EOF
    
    print_message "$GREEN" "Created template directory and sample template"
fi

# Run the CLI for each database
for db in "${DATABASES[@]}"; do
    print_message "$BLUE" "Processing database: $db"
    
    # Create output directory for this database
    db_output_dir="$OUTPUT_DIR/$db"
    mkdir -p "$db_output_dir"
    
    # Generate models using the CLI
    connection_string="$CONNECTION_STRING_BASE;Database=$db;"
    
    run_command "dotnet run --project $CLI_PROJECT_PATH -- generate \
        --connection \"$connection_string\" \
        --template \"$TEMPLATE_DIR/entity.hbs\" \
        --output \"$db_output_dir\" \
        --namespace \"Generated.$db\" \
        --data-annotations true \
        --navigation-properties true" \
        "Generating models for $db"
    
    # Check if models were generated
    model_count=$(find "$db_output_dir" -type f -name "*.cs" | wc -l)
    print_message "$GREEN" "Generated $model_count models for $db"
done

# Run the EF Core comparison tests
print_message "$BLUE" "Running EF Core comparison tests"

run_command "dotnet test $TEST_PROJECT_PATH -c Release --logger \"console;verbosity=detailed\"" \
    "Running EF Core comparison tests"

# Generate a summary report
summary_file="$OUTPUT_DIR/summary.md"

cat > "$summary_file" << EOF
# EzDbCodeGen CLI and EF Core Comparison Test Results

## Generated Models

$(for db in "${DATABASES[@]}"; do
    model_count=$(find "$OUTPUT_DIR/$db" -type f -name "*.cs" | wc -l)
    echo "- **$db**: $model_count models generated"
done)

## Test Results

The EF Core comparison tests have been run to validate the generated models against EF Core's model generation.
See the test output above for detailed results.

## Next Steps

1. Review the generated models in the output directory
2. Check the test results for any discrepancies
3. Modify templates as needed to improve the generated code

EOF

print_message "$GREEN" "Summary report generated at $summary_file"
print_message "$GREEN" "All tests completed successfully!"
