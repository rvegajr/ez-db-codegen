using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.Data.SqlClient;
using Xunit;

namespace EzDbCodeGen.Schema.IntegrationTests;

public class SchemaExtractionTests : IClassFixture<SqlServerSampleFixture>
{
    private readonly SqlServerSampleFixture _fixture;
    
    public SchemaExtractionTests(SqlServerSampleFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Theory]
    [InlineData("AdventureWorks")]
    [InlineData("Northwind")]
    [InlineData("WideWorldImporters")]
    [InlineData("ContosoUniversity")]
    public async Task Extracted_Table_Count_Matches_SysObjects(string databaseName)
    {
        // Arrange
        var fixture = new SqlServerSampleFixture(databaseName);
        var provider = fixture.SchemaProvider;
        
        // Act
        var extractionTask = provider.GetSchemaAsync();
        await extractionTask.Should().CompleteWithin(10.Seconds());
        var schema = await extractionTask;
        
        // Get actual table count from the database
        var connectionString = provider.ConnectionString;
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        
        await using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT COUNT(*) 
            FROM INFORMATION_SCHEMA.TABLES 
            WHERE TABLE_TYPE='BASE TABLE'";
            
        var expectedCount = Convert.ToInt32(await command.ExecuteScalarAsync());
        
        // Assert
        schema.Tables.Count.Should().Be(expectedCount, 
            because: "extracted schema should contain all base tables from the database");
    }
    
    [Theory]
    [InlineData("AdventureWorks")]
    [InlineData("Northwind")]
    [InlineData("WideWorldImporters")]
    [InlineData("ContosoUniversity")]
    public async Task Schema_Extraction_Performance_Under_10_Seconds(string databaseName)
    {
        // Arrange
        var fixture = new SqlServerSampleFixture(databaseName);
        var provider = fixture.SchemaProvider;
        
        // Act & Assert
        await provider.GetSchemaAsync()
            .Should()
            .CompleteWithin(10.Seconds());
    }
}
