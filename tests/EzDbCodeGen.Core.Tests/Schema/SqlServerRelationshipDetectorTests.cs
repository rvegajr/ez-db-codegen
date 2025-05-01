using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EzDbCodeGen.Core.Logging;
using EzDbCodeGen.Core.Schema;
using EzDbCodeGen.Schema;
using FluentAssertions;
using Moq;
using Xunit;

namespace EzDbCodeGen.Core.Tests.Schema
{
    /// <summary>
    /// Unit tests for the SqlServer-specific implementation of RelationshipDetector.
    /// </summary>
    public class SqlServerRelationshipDetectorTests
    {
        private readonly Mock<ILogger> _loggerMock;
        private readonly RelationshipDetector _detector;

        public SqlServerRelationshipDetectorTests()
        {
            _loggerMock = new Mock<ILogger>();
            _detector = new RelationshipDetector(_loggerMock.Object);
        }

        [Fact]
        public async Task DetectRelationshipsAsync_WithOneToManyForeignKey_ShouldDetectOneToManyRelationship()
        {
            // Arrange
            var schema = CreateSchemaWithOneToManyRelation();

            // Act
            var relationships = await _detector.DetectRelationshipsAsync(schema);

            // Assert
            relationships.Should().NotBeNull();
            relationships.Should().HaveCount(1);
            
            var relationship = relationships.First();
            relationship.RelationshipType.Should().Be(RelationshipType.OneToMany);
            relationship.SourceTable.Name.Should().Be("Order");
            relationship.TargetTable.Name.Should().Be("Customer");
            relationship.SourceNavigationProperty.Should().Be("Customer");
            relationship.TargetNavigationProperty.Should().Be("Orders");
            relationship.IsSelfReferencing.Should().BeFalse();
        }

        [Fact]
        public async Task DetectRelationshipsAsync_WithOneToOneRelationship_ShouldDetectOneToOneRelationship()
        {
            // Arrange
            var schema = CreateSchemaWithOneToOneRelation();

            // Act
            var relationships = await _detector.DetectRelationshipsAsync(schema);

            // Assert
            relationships.Should().NotBeNull();
            var oneToOneRelationships = relationships.Where(r => r.RelationshipType == RelationshipType.OneToOne).ToList();
            oneToOneRelationships.Should().HaveCount(1);
            
            var relationship = oneToOneRelationships.First();
            relationship.SourceTable.Name.Should().Be("PersonDetail");
            relationship.TargetTable.Name.Should().Be("Person");
            relationship.SourceNavigationProperty.Should().Be("Person");
            relationship.TargetNavigationProperty.Should().Be("PersonDetail");
        }

        [Fact]
        public async Task DetectRelationshipsAsync_WithManyToManyJunctionTable_ShouldDetectManyToManyRelationship()
        {
            // Arrange
            var schema = CreateSchemaWithManyToManyRelation();

            // Act
            var relationships = await _detector.DetectRelationshipsAsync(schema);

            // Assert
            relationships.Should().NotBeNull();
            var manyToManyRelationships = relationships.Where(r => r.RelationshipType == RelationshipType.ManyToMany).ToList();
            manyToManyRelationships.Should().HaveCount(1);
            
            var relationship = manyToManyRelationships.First();
            relationship.SourceTable.Name.Should().Be("Student");
            relationship.TargetTable.Name.Should().Be("Course");
            relationship.JunctionTable.Should().NotBeNull();
            relationship.JunctionTable.Name.Should().Be("StudentCourse");
            relationship.SourceNavigationProperty.Should().Be("Courses");
            relationship.TargetNavigationProperty.Should().Be("Students");
        }

        [Fact]
        public async Task DetectRelationshipsAsync_WithSelfReferencingRelationship_ShouldDetectSelfReferencingRelationship()
        {
            // Arrange
            var schema = CreateSchemaWithSelfReferencingRelation();

            // Act
            var relationships = await _detector.DetectRelationshipsAsync(schema);

            // Assert
            relationships.Should().NotBeNull();
            var selfReferencingRelationships = relationships.Where(r => r.IsSelfReferencing).ToList();
            selfReferencingRelationships.Should().HaveCount(1);
            
            var relationship = selfReferencingRelationships.First();
            relationship.RelationshipType.Should().Be(RelationshipType.OneToMany);
            relationship.SourceTable.Name.Should().Be("Employee");
            relationship.TargetTable.Name.Should().Be("Employee");
            relationship.IsSelfReferencing.Should().BeTrue();
        }

        [Fact]
        public async Task DetectRelationshipsAsync_WithInheritanceRelationship_ShouldDetectInheritanceRelationship()
        {
            // Arrange
            var schema = CreateSchemaWithInheritanceRelation();

            // Act
            var relationships = await _detector.DetectRelationshipsAsync(schema);

            // Assert
            relationships.Should().NotBeNull();
            var inheritanceRelationships = relationships.Where(r => r.RelationshipType == RelationshipType.Inheritance).ToList();
            inheritanceRelationships.Should().HaveCount(1);
            
            var relationship = inheritanceRelationships.First();
            relationship.SourceTable.Name.Should().Be("Car");
            relationship.TargetTable.Name.Should().Be("Vehicle");
            relationship.InheritanceType.Should().Be(InheritanceType.TablePerType);
        }

        #region Helper Methods for Creating Test Schemas

        private ISchemaModel CreateSchemaWithOneToManyRelation()
        {
            var schema = new SchemaModel();
            
            // Create Customer table
            var customerTable = new TableModel
            {
                Name = "Customer",
                Schema = "dbo"
            };
            
            var customerId = new ColumnModel
            {
                Name = "CustomerId",
                DataType = "int",
                IsIdentity = true,
                IsPrimaryKey = true
            };
            
            var customerName = new ColumnModel
            {
                Name = "Name",
                DataType = "nvarchar",
                MaxLength = 100
            };
            
            customerTable.Columns.Add(customerId);
            customerTable.Columns.Add(customerName);
            customerTable.PrimaryKey = new PrimaryKeyModel
            {
                Name = "PK_Customer",
                Columns = { customerId }
            };
            
            // Create Order table
            var orderTable = new TableModel
            {
                Name = "Order",
                Schema = "dbo"
            };
            
            var orderId = new ColumnModel
            {
                Name = "OrderId",
                DataType = "int",
                IsIdentity = true,
                IsPrimaryKey = true
            };
            
            var orderCustomerId = new ColumnModel
            {
                Name = "CustomerId",
                DataType = "int",
                IsForeignKey = true
            };
            
            var orderDate = new ColumnModel
            {
                Name = "OrderDate",
                DataType = "datetime"
            };
            
            orderTable.Columns.Add(orderId);
            orderTable.Columns.Add(orderCustomerId);
            orderTable.Columns.Add(orderDate);
            orderTable.PrimaryKey = new PrimaryKeyModel
            {
                Name = "PK_Order",
                Columns = { orderId }
            };
            
            // Create foreign key
            var foreignKey = new ForeignKeyModel
            {
                Name = "FK_Order_Customer",
                ReferencedTable = customerTable
            };
            foreignKey.Columns.Add(orderCustomerId);
            foreignKey.ReferencedColumns.Add(customerId);
            orderTable.ForeignKeys.Add(foreignKey);
            
            // Add tables to schema
            schema.Tables.Add(customerTable);
            schema.Tables.Add(orderTable);
            
            return schema;
        }

        private ISchemaModel CreateSchemaWithOneToOneRelation()
        {
            var schema = new SchemaModel();
            
            // Create Person table
            var personTable = new TableModel
            {
                Name = "Person",
                Schema = "dbo"
            };
            
            var personId = new ColumnModel
            {
                Name = "PersonId",
                DataType = "int",
                IsIdentity = true,
                IsPrimaryKey = true
            };
            
            var personName = new ColumnModel
            {
                Name = "Name",
                DataType = "nvarchar",
                MaxLength = 100
            };
            
            personTable.Columns.Add(personId);
            personTable.Columns.Add(personName);
            personTable.PrimaryKey = new PrimaryKeyModel
            {
                Name = "PK_Person",
                Columns = { personId }
            };
            
            // Create PersonDetail table with primary key that is also a foreign key
            var personDetailTable = new TableModel
            {
                Name = "PersonDetail",
                Schema = "dbo"
            };
            
            var personDetailId = new ColumnModel
            {
                Name = "PersonId", // Same name as PK in Person
                DataType = "int",
                IsPrimaryKey = true,
                IsForeignKey = true
            };
            
            var address = new ColumnModel
            {
                Name = "Address",
                DataType = "nvarchar",
                MaxLength = 200
            };
            
            personDetailTable.Columns.Add(personDetailId);
            personDetailTable.Columns.Add(address);
            personDetailTable.PrimaryKey = new PrimaryKeyModel
            {
                Name = "PK_PersonDetail",
                Columns = { personDetailId }
            };
            
            // Create foreign key (that is also the primary key)
            var foreignKey = new ForeignKeyModel
            {
                Name = "FK_PersonDetail_Person",
                ReferencedTable = personTable
            };
            foreignKey.Columns.Add(personDetailId);
            foreignKey.ReferencedColumns.Add(personId);
            personDetailTable.ForeignKeys.Add(foreignKey);
            
            // Add tables to schema
            schema.Tables.Add(personTable);
            schema.Tables.Add(personDetailTable);
            
            return schema;
        }

        private ISchemaModel CreateSchemaWithManyToManyRelation()
        {
            var schema = new SchemaModel();
            
            // Create Student table
            var studentTable = new TableModel
            {
                Name = "Student",
                Schema = "dbo"
            };
            
            var studentId = new ColumnModel
            {
                Name = "StudentId",
                DataType = "int",
                IsIdentity = true,
                IsPrimaryKey = true
            };
            
            var studentName = new ColumnModel
            {
                Name = "Name",
                DataType = "nvarchar",
                MaxLength = 100
            };
            
            studentTable.Columns.Add(studentId);
            studentTable.Columns.Add(studentName);
            studentTable.PrimaryKey = new PrimaryKeyModel
            {
                Name = "PK_Student",
                Columns = { studentId }
            };
            
            // Create Course table
            var courseTable = new TableModel
            {
                Name = "Course",
                Schema = "dbo"
            };
            
            var courseId = new ColumnModel
            {
                Name = "CourseId",
                DataType = "int",
                IsIdentity = true,
                IsPrimaryKey = true
            };
            
            var courseName = new ColumnModel
            {
                Name = "Name",
                DataType = "nvarchar",
                MaxLength = 100
            };
            
            courseTable.Columns.Add(courseId);
            courseTable.Columns.Add(courseName);
            courseTable.PrimaryKey = new PrimaryKeyModel
            {
                Name = "PK_Course",
                Columns = { courseId }
            };
            
            // Create junction table
            var junctionTable = new TableModel
            {
                Name = "StudentCourse",
                Schema = "dbo"
            };
            
            var junctionStudentId = new ColumnModel
            {
                Name = "StudentId",
                DataType = "int",
                IsPrimaryKey = true,
                IsForeignKey = true
            };
            
            var junctionCourseId = new ColumnModel
            {
                Name = "CourseId",
                DataType = "int",
                IsPrimaryKey = true,
                IsForeignKey = true
            };
            
            junctionTable.Columns.Add(junctionStudentId);
            junctionTable.Columns.Add(junctionCourseId);
            junctionTable.PrimaryKey = new PrimaryKeyModel
            {
                Name = "PK_StudentCourse",
                Columns = { junctionStudentId, junctionCourseId }
            };
            
            // Create foreign keys
            var fkStudent = new ForeignKeyModel
            {
                Name = "FK_StudentCourse_Student",
                ReferencedTable = studentTable
            };
            fkStudent.Columns.Add(junctionStudentId);
            fkStudent.ReferencedColumns.Add(studentId);
            junctionTable.ForeignKeys.Add(fkStudent);
            
            var fkCourse = new ForeignKeyModel
            {
                Name = "FK_StudentCourse_Course",
                ReferencedTable = courseTable
            };
            fkCourse.Columns.Add(junctionCourseId);
            fkCourse.ReferencedColumns.Add(courseId);
            junctionTable.ForeignKeys.Add(fkCourse);
            
            // Add tables to schema
            schema.Tables.Add(studentTable);
            schema.Tables.Add(courseTable);
            schema.Tables.Add(junctionTable);
            
            return schema;
        }

        private ISchemaModel CreateSchemaWithSelfReferencingRelation()
        {
            var schema = new SchemaModel();
            
            // Create Employee table
            var employeeTable = new TableModel
            {
                Name = "Employee",
                Schema = "dbo"
            };
            
            var employeeId = new ColumnModel
            {
                Name = "EmployeeId",
                DataType = "int",
                IsIdentity = true,
                IsPrimaryKey = true
            };
            
            var employeeName = new ColumnModel
            {
                Name = "Name",
                DataType = "nvarchar",
                MaxLength = 100
            };
            
            var managerId = new ColumnModel
            {
                Name = "ManagerId",
                DataType = "int",
                IsNullable = true,
                IsForeignKey = true
            };
            
            employeeTable.Columns.Add(employeeId);
            employeeTable.Columns.Add(employeeName);
            employeeTable.Columns.Add(managerId);
            employeeTable.PrimaryKey = new PrimaryKeyModel
            {
                Name = "PK_Employee",
                Columns = { employeeId }
            };
            
            // Create self-referencing foreign key
            var foreignKey = new ForeignKeyModel
            {
                Name = "FK_Employee_Manager",
                ReferencedTable = employeeTable
            };
            foreignKey.Columns.Add(managerId);
            foreignKey.ReferencedColumns.Add(employeeId);
            employeeTable.ForeignKeys.Add(foreignKey);
            
            // Add table to schema
            schema.Tables.Add(employeeTable);
            
            return schema;
        }

        private ISchemaModel CreateSchemaWithInheritanceRelation()
        {
            var schema = new SchemaModel();
            
            // Create base table
            var vehicleTable = new TableModel
            {
                Name = "Vehicle",
                Schema = "dbo"
            };
            
            var vehicleId = new ColumnModel
            {
                Name = "VehicleId",
                DataType = "int",
                IsIdentity = true,
                IsPrimaryKey = true
            };
            
            var vehicleName = new ColumnModel
            {
                Name = "Name",
                DataType = "nvarchar",
                MaxLength = 100
            };
            
            vehicleTable.Columns.Add(vehicleId);
            vehicleTable.Columns.Add(vehicleName);
            vehicleTable.PrimaryKey = new PrimaryKeyModel
            {
                Name = "PK_Vehicle",
                Columns = { vehicleId }
            };
            
            // Create derived table
            var carTable = new TableModel
            {
                Name = "Car",
                Schema = "dbo"
            };
            
            var carId = new ColumnModel
            {
                Name = "VehicleId", // Same name as PK in Vehicle
                DataType = "int",
                IsPrimaryKey = true,
                IsForeignKey = true
            };
            
            var doors = new ColumnModel
            {
                Name = "Doors",
                DataType = "int"
            };
            
            var engineSize = new ColumnModel
            {
                Name = "EngineSize",
                DataType = "decimal",
                Precision = 5,
                Scale = 2
            };
            
            carTable.Columns.Add(carId);
            carTable.Columns.Add(doors);
            carTable.Columns.Add(engineSize);
            carTable.PrimaryKey = new PrimaryKeyModel
            {
                Name = "PK_Car",
                Columns = { carId }
            };
            
            // Create inheritance foreign key
            var foreignKey = new ForeignKeyModel
            {
                Name = "FK_Car_Vehicle",
                ReferencedTable = vehicleTable
            };
            foreignKey.Columns.Add(carId);
            foreignKey.ReferencedColumns.Add(vehicleId);
            carTable.ForeignKeys.Add(foreignKey);
            
            // Add tables to schema
            schema.Tables.Add(vehicleTable);
            schema.Tables.Add(carTable);
            
            return schema;
        }

        #endregion
    }
}
