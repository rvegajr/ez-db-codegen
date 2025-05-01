#!/bin/bash
set -e

# Configuration
SQLCMD_OPTS="-b -S localhost,1433 -U sa -P Your_password123"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
BACKUPS_DIR="$SCRIPT_DIR/backups"

# Ensure backups directory exists
mkdir -p "$BACKUPS_DIR"

# Function to restore a database
restore_database() {
    local db_name=$1
    local backup_file="$BACKUPS_DIR/$db_name.bak"
    local sql_file="$BACKUPS_DIR/$db_name.sql"
    
    echo "Restoring $db_name..."
    
    # Drop database if exists
    sqlcmd $SQLCMD_OPTS -Q "IF DB_ID('$db_name') IS NOT NULL DROP DATABASE [$db_name]"
    
    if [ -f "$backup_file" ]; then
        # Restore from .bak file
        sqlcmd $SQLCMD_OPTS -Q "RESTORE DATABASE [$db_name] FROM DISK='$backup_file' WITH MOVE '${db_name}' TO '/var/opt/mssql/data/${db_name}.mdf', MOVE '${db_name}_log' TO '/var/opt/mssql/data/${db_name}_log.ldf'"
    elif [ -f "$sql_file" ]; then
        # Create and populate from .sql file
        sqlcmd $SQLCMD_OPTS -Q "CREATE DATABASE [$db_name]"
        sqlcmd $SQLCMD_OPTS -d "$db_name" -i "$sql_file"
    else
        echo "Error: Neither $backup_file nor $sql_file found!"
        exit 1
    fi
    
    echo "$db_name restored successfully"
}

# Main
if [ $# -eq 0 ]; then
    echo "Usage: $0 <database1> [database2 ...]"
    exit 1
fi

# Process each database
for db in "$@"; do
    restore_database "$db"
done

echo "All databases restored successfully"
