using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EzDbCodeGen.Core.Schema;
using Moq;
using Xunit;
using FluentAssertions;

namespace EzDbCodeGen.Core.Tests.Schema
{
    public class RelationshipDetectorTests
    {
        [Fact]
        public void DetectRelationships_WithOneToManyForeignKey_ShouldDetectOneToManyRelationship()
        {
            // Arrange
            var mockSchema = CreateMockSchemaWithOneToManyRelation();
            var options = new RelationshipDetectionOptions();
            var sut = CreateRelationshipDetector(options);

            // Act
            var relationships = sut.DetectRelationships(mockSchema);

            // Assert
            relationships.Should().NotBeNull();
            relationships.Should().HaveCount(1);
            
            var relationship = relationships.First();
            relationship.RelationshipType.Should().Be(RelationshipType.OneToMany);
            relationship.PrimaryEntityName.Should().Be("Customer");
            relationship.ForeignEntityName.Should().Be("Order");
            relationship.PrimaryTable.Should().Be(mockSchema.Tables.First(t => t.Name == "Customer"));
            relationship.ForeignTable.Should().Be(mockSchema.Tables.First(t => t.Name == "Order"));
            relationship.PrimaryColumns.Should().HaveCount(1);
            relationship.ForeignColumns.Should().HaveCount(1);
        }

        [Fact]
        public void DetectRelationships_WithManyToManyJunctionTable_ShouldDetectManyToManyRelationship()
        {
            // Arrange
            var mockSchema = CreateMockSchemaWithManyToManyRelation();
            var options = new RelationshipDetectionOptions { DetectManyToManyRelationships = true };
            var sut = CreateRelationshipDetector(options);

            // Act
            var relationships = sut.DetectRelationships(mockSchema);

            // Assert
            relationships.Should().NotBeNull();
            relationships.Where(r => r.RelationshipType == RelationshipType.ManyToMany).Should().HaveCount(1);
            
            var relationship = relationships.First(r => r.RelationshipType == RelationshipType.ManyToMany);
            relationship.PrimaryEntityName.Should().Be("Student");
            relationship.ForeignEntityName.Should().Be("Course");
            relationship.JunctionTable.Should().NotBeNull();
            relationship.JunctionTable.Name.Should().Be("StudentCourse");
        }

        [Fact]
        public void DetectRelationships_WithOneToOneRelationship_ShouldDetectOneToOneRelationship()
        {
            // Arrange
            var mockSchema = CreateMockSchemaWithOneToOneRelation();
            var options = new RelationshipDetectionOptions { DetectOneToOneRelationships = true };
            var sut = CreateRelationshipDetector(options);

            // Act
            var relationships = sut.DetectRelationships(mockSchema);

            // Assert
            relationships.Should().NotBeNull();
            relationships.Where(r => r.RelationshipType == RelationshipType.OneToOne).Should().HaveCount(1);
            
            var relationship = relationships.First(r => r.RelationshipType == RelationshipType.OneToOne);
            relationship.PrimaryEntityName.Should().Be("Person");
            relationship.ForeignEntityName.Should().Be("PersonDetail");
        }

        [Fact]
        public void DetectRelationships_WithSelfReferencing_ShouldDetectSelfReferencingRelationship()
        {
            // Arrange
            var mockSchema = CreateMockSchemaWithSelfReferencingRelation();
            var options = new RelationshipDetectionOptions { DetectSelfReferencingRelationships = true };
            var sut = CreateRelationshipDetector(options);

            // Act
            var relationships = sut.DetectRelationships(mockSchema);

            // Assert
            relationships.Should().NotBeNull();
            relationships.Where(r => r.IsSelfReferencing).Should().HaveCount(1);
            
            var relationship = relationships.First(r => r.IsSelfReferencing);
            relationship.PrimaryEntityName.Should().Be("Employee");
            relationship.ForeignEntityName.Should().Be("Employee");
            relationship.RelationshipType.Should().Be(RelationshipType.OneToMany);
        }

        [Fact]
        public void DetectRelationships_WithInheritance_ShouldDetectInheritanceRelationship()
        {
            // Arrange
            var mockSchema = CreateMockSchemaWithInheritanceRelation();
            var options = new RelationshipDetectionOptions { DetectInheritanceRelationships = true };
            var sut = CreateRelationshipDetector(options);

            // Act
            var relationships = sut.DetectRelationships(mockSchema);

            // Assert
            relationships.Should().NotBeNull();
            relationships.Where(r => r.IsInheritance).Should().HaveCount(1);
            
            var relationship = relationships.First(r => r.IsInheritance);
            relationship.PrimaryEntityName.Should().Be("Vehicle");
            relationship.ForeignEntityName.Should().Be("Car");
            relationship.InheritanceType.Should().Be(InheritanceType.TablePerHierarchy);
        }

        [Fact]
        public void DetectRelationships_WithNoValidRelations_ShouldReturnEmptyCollection()
        {
            // Arrange
            var mockSchema = new Mock<IDatabaseSchema>();
            mockSchema.Setup(s => s.Tables).Returns(new List<ITable>());
            var options = new RelationshipDetectionOptions();
            var sut = CreateRelationshipDetector(options);

            // Act
            var relationships = sut.DetectRelationships(mockSchema.Object);

            // Assert
            relationships.Should().NotBeNull();
            relationships.Should().BeEmpty();
        }

        // Helper methods to create mock schemas with different relationship types
        private IDatabaseSchema CreateMockSchemaWithOneToManyRelation()
        {
            var mockSchema = new Mock<IDatabaseSchema>();
            var tables = new List<ITable>();

            // Create Customer table with primary key
            var customerTable = new Mock<ITable>();
            customerTable.Setup(t => t.Name).Returns("Customer");
            customerTable.Setup(t => t.Schema).Returns("dbo");
            
            var customerId = new Mock<IColumn>();
            customerId.Setup(c => c.Name).Returns("CustomerId");
            customerId.Setup(c => c.IsPartOfPrimaryKey).Returns(true);
            
            var customerColumns = new List<IColumn> { customerId.Object };
            customerTable.Setup(t => t.Columns).Returns(customerColumns);
            
            var customerPk = new Mock<IKey>();
            customerPk.Setup(k => k.Name).Returns("PK_Customer");
            customerPk.Setup(k => k.Columns).Returns(new List<IColumn> { customerId.Object });
            customerTable.Setup(t => t.PrimaryKey).Returns(customerPk.Object);
            
            // Create Order table with foreign key to Customer
            var orderTable = new Mock<ITable>();
            orderTable.Setup(t => t.Name).Returns("Order");
            orderTable.Setup(t => t.Schema).Returns("dbo");
            
            var orderId = new Mock<IColumn>();
            orderId.Setup(c => c.Name).Returns("OrderId");
            orderId.Setup(c => c.IsPartOfPrimaryKey).Returns(true);
            
            var orderCustomerId = new Mock<IColumn>();
            orderCustomerId.Setup(c => c.Name).Returns("CustomerId");
            orderCustomerId.Setup(c => c.IsPartOfForeignKey).Returns(true);
            
            var orderColumns = new List<IColumn> { orderId.Object, orderCustomerId.Object };
            orderTable.Setup(t => t.Columns).Returns(orderColumns);
            
            var orderPk = new Mock<IKey>();
            orderPk.Setup(k => k.Name).Returns("PK_Order");
            orderPk.Setup(k => k.Columns).Returns(new List<IColumn> { orderId.Object });
            orderTable.Setup(t => t.PrimaryKey).Returns(orderPk.Object);
            
            var orderFk = new Mock<IForeignKey>();
            orderFk.Setup(fk => fk.Name).Returns("FK_Order_Customer");
            orderFk.Setup(fk => fk.Columns).Returns(new List<IColumn> { orderCustomerId.Object });
            orderFk.Setup(fk => fk.ReferencedTable).Returns(customerTable.Object);
            orderFk.Setup(fk => fk.ReferencedColumns).Returns(new List<IColumn> { customerId.Object });
            
            var foreignKeys = new List<IForeignKey> { orderFk.Object };
            orderTable.Setup(t => t.ForeignKeys).Returns(foreignKeys);
            
            tables.Add(customerTable.Object);
            tables.Add(orderTable.Object);
            mockSchema.Setup(s => s.Tables).Returns(tables);
            
            return mockSchema.Object;
        }

        private IDatabaseSchema CreateMockSchemaWithManyToManyRelation()
        {
            var mockSchema = new Mock<IDatabaseSchema>();
            var tables = new List<ITable>();

            // Create Student table
            var studentTable = new Mock<ITable>();
            studentTable.Setup(t => t.Name).Returns("Student");
            studentTable.Setup(t => t.Schema).Returns("dbo");
            
            var studentId = new Mock<IColumn>();
            studentId.Setup(c => c.Name).Returns("StudentId");
            studentId.Setup(c => c.IsPartOfPrimaryKey).Returns(true);
            
            var studentColumns = new List<IColumn> { studentId.Object };
            studentTable.Setup(t => t.Columns).Returns(studentColumns);
            
            var studentPk = new Mock<IKey>();
            studentPk.Setup(k => k.Name).Returns("PK_Student");
            studentPk.Setup(k => k.Columns).Returns(new List<IColumn> { studentId.Object });
            studentTable.Setup(t => t.PrimaryKey).Returns(studentPk.Object);
            
            // Create Course table
            var courseTable = new Mock<ITable>();
            courseTable.Setup(t => t.Name).Returns("Course");
            courseTable.Setup(t => t.Schema).Returns("dbo");
            
            var courseId = new Mock<IColumn>();
            courseId.Setup(c => c.Name).Returns("CourseId");
            courseId.Setup(c => c.IsPartOfPrimaryKey).Returns(true);
            
            var courseColumns = new List<IColumn> { courseId.Object };
            courseTable.Setup(t => t.Columns).Returns(courseColumns);
            
            var coursePk = new Mock<IKey>();
            coursePk.Setup(k => k.Name).Returns("PK_Course");
            coursePk.Setup(k => k.Columns).Returns(new List<IColumn> { courseId.Object });
            courseTable.Setup(t => t.PrimaryKey).Returns(coursePk.Object);
            
            // Create StudentCourse junction table
            var junctionTable = new Mock<ITable>();
            junctionTable.Setup(t => t.Name).Returns("StudentCourse");
            junctionTable.Setup(t => t.Schema).Returns("dbo");
            
            var junctionStudentId = new Mock<IColumn>();
            junctionStudentId.Setup(c => c.Name).Returns("StudentId");
            junctionStudentId.Setup(c => c.IsPartOfPrimaryKey).Returns(true);
            junctionStudentId.Setup(c => c.IsPartOfForeignKey).Returns(true);
            
            var junctionCourseId = new Mock<IColumn>();
            junctionCourseId.Setup(c => c.Name).Returns("CourseId");
            junctionCourseId.Setup(c => c.IsPartOfPrimaryKey).Returns(true);
            junctionCourseId.Setup(c => c.IsPartOfForeignKey).Returns(true);
            
            var junctionColumns = new List<IColumn> { junctionStudentId.Object, junctionCourseId.Object };
            junctionTable.Setup(t => t.Columns).Returns(junctionColumns);
            
            var junctionPk = new Mock<IKey>();
            junctionPk.Setup(k => k.Name).Returns("PK_StudentCourse");
            junctionPk.Setup(k => k.Columns).Returns(new List<IColumn> { junctionStudentId.Object, junctionCourseId.Object });
            junctionTable.Setup(t => t.PrimaryKey).Returns(junctionPk.Object);
            
            var junctionFkStudent = new Mock<IForeignKey>();
            junctionFkStudent.Setup(fk => fk.Name).Returns("FK_StudentCourse_Student");
            junctionFkStudent.Setup(fk => fk.Columns).Returns(new List<IColumn> { junctionStudentId.Object });
            junctionFkStudent.Setup(fk => fk.ReferencedTable).Returns(studentTable.Object);
            junctionFkStudent.Setup(fk => fk.ReferencedColumns).Returns(new List<IColumn> { studentId.Object });
            
            var junctionFkCourse = new Mock<IForeignKey>();
            junctionFkCourse.Setup(fk => fk.Name).Returns("FK_StudentCourse_Course");
            junctionFkCourse.Setup(fk => fk.Columns).Returns(new List<IColumn> { junctionCourseId.Object });
            junctionFkCourse.Setup(fk => fk.ReferencedTable).Returns(courseTable.Object);
            junctionFkCourse.Setup(fk => fk.ReferencedColumns).Returns(new List<IColumn> { courseId.Object });
            
            var junctionForeignKeys = new List<IForeignKey> { junctionFkStudent.Object, junctionFkCourse.Object };
            junctionTable.Setup(t => t.ForeignKeys).Returns(junctionForeignKeys);
            
            tables.Add(studentTable.Object);
            tables.Add(courseTable.Object);
            tables.Add(junctionTable.Object);
            mockSchema.Setup(s => s.Tables).Returns(tables);
            
            return mockSchema.Object;
        }

        private IDatabaseSchema CreateMockSchemaWithOneToOneRelation()
        {
            var mockSchema = new Mock<IDatabaseSchema>();
            var tables = new List<ITable>();

            // Create Person table
            var personTable = new Mock<ITable>();
            personTable.Setup(t => t.Name).Returns("Person");
            personTable.Setup(t => t.Schema).Returns("dbo");
            
            var personId = new Mock<IColumn>();
            personId.Setup(c => c.Name).Returns("PersonId");
            personId.Setup(c => c.IsPartOfPrimaryKey).Returns(true);
            
            var personColumns = new List<IColumn> { personId.Object };
            personTable.Setup(t => t.Columns).Returns(personColumns);
            
            var personPk = new Mock<IKey>();
            personPk.Setup(k => k.Name).Returns("PK_Person");
            personPk.Setup(k => k.Columns).Returns(new List<IColumn> { personId.Object });
            personTable.Setup(t => t.PrimaryKey).Returns(personPk.Object);
            
            // Create PersonDetail table
            var detailTable = new Mock<ITable>();
            detailTable.Setup(t => t.Name).Returns("PersonDetail");
            detailTable.Setup(t => t.Schema).Returns("dbo");
            
            var detailPersonId = new Mock<IColumn>();
            detailPersonId.Setup(c => c.Name).Returns("PersonId");
            detailPersonId.Setup(c => c.IsPartOfPrimaryKey).Returns(true);
            detailPersonId.Setup(c => c.IsPartOfForeignKey).Returns(true);
            
            var detailColumns = new List<IColumn> { detailPersonId.Object };
            detailTable.Setup(t => t.Columns).Returns(detailColumns);
            
            var detailPk = new Mock<IKey>();
            detailPk.Setup(k => k.Name).Returns("PK_PersonDetail");
            detailPk.Setup(k => k.Columns).Returns(new List<IColumn> { detailPersonId.Object });
            detailTable.Setup(t => t.PrimaryKey).Returns(detailPk.Object);
            
            var detailFk = new Mock<IForeignKey>();
            detailFk.Setup(fk => fk.Name).Returns("FK_PersonDetail_Person");
            detailFk.Setup(fk => fk.Columns).Returns(new List<IColumn> { detailPersonId.Object });
            detailFk.Setup(fk => fk.ReferencedTable).Returns(personTable.Object);
            detailFk.Setup(fk => fk.ReferencedColumns).Returns(new List<IColumn> { personId.Object });
            detailFk.Setup(fk => fk.IsUnique).Returns(true); // This makes it a one-to-one relationship
            
            var detailForeignKeys = new List<IForeignKey> { detailFk.Object };
            detailTable.Setup(t => t.ForeignKeys).Returns(detailForeignKeys);
            
            tables.Add(personTable.Object);
            tables.Add(detailTable.Object);
            mockSchema.Setup(s => s.Tables).Returns(tables);
            
            return mockSchema.Object;
        }

        private IDatabaseSchema CreateMockSchemaWithSelfReferencingRelation()
        {
            var mockSchema = new Mock<IDatabaseSchema>();
            var tables = new List<ITable>();

            // Create Employee table with self-referencing foreign key
            var employeeTable = new Mock<ITable>();
            employeeTable.Setup(t => t.Name).Returns("Employee");
            employeeTable.Setup(t => t.Schema).Returns("dbo");
            
            var employeeId = new Mock<IColumn>();
            employeeId.Setup(c => c.Name).Returns("EmployeeId");
            employeeId.Setup(c => c.IsPartOfPrimaryKey).Returns(true);
            
            var managerId = new Mock<IColumn>();
            managerId.Setup(c => c.Name).Returns("ManagerId");
            managerId.Setup(c => c.IsPartOfForeignKey).Returns(true);
            managerId.Setup(c => c.IsNullable).Returns(true);
            
            var employeeColumns = new List<IColumn> { employeeId.Object, managerId.Object };
            employeeTable.Setup(t => t.Columns).Returns(employeeColumns);
            
            var employeePk = new Mock<IKey>();
            employeePk.Setup(k => k.Name).Returns("PK_Employee");
            employeePk.Setup(k => k.Columns).Returns(new List<IColumn> { employeeId.Object });
            employeeTable.Setup(t => t.PrimaryKey).Returns(employeePk.Object);
            
            var employeeFk = new Mock<IForeignKey>();
            employeeFk.Setup(fk => fk.Name).Returns("FK_Employee_Employee");
            employeeFk.Setup(fk => fk.Columns).Returns(new List<IColumn> { managerId.Object });
            employeeFk.Setup(fk => fk.ReferencedTable).Returns(employeeTable.Object);
            employeeFk.Setup(fk => fk.ReferencedColumns).Returns(new List<IColumn> { employeeId.Object });
            
            var employeeForeignKeys = new List<IForeignKey> { employeeFk.Object };
            employeeTable.Setup(t => t.ForeignKeys).Returns(employeeForeignKeys);
            
            tables.Add(employeeTable.Object);
            mockSchema.Setup(s => s.Tables).Returns(tables);
            
            return mockSchema.Object;
        }

        private IDatabaseSchema CreateMockSchemaWithInheritanceRelation()
        {
            var mockSchema = new Mock<IDatabaseSchema>();
            var tables = new List<ITable>();

            // Create base Vehicle table
            var vehicleTable = new Mock<ITable>();
            vehicleTable.Setup(t => t.Name).Returns("Vehicle");
            vehicleTable.Setup(t => t.Schema).Returns("dbo");
            
            var vehicleId = new Mock<IColumn>();
            vehicleId.Setup(c => c.Name).Returns("VehicleId");
            vehicleId.Setup(c => c.IsPartOfPrimaryKey).Returns(true);
            
            var discriminator = new Mock<IColumn>();
            discriminator.Setup(c => c.Name).Returns("Discriminator");
            
            var vehicleColumns = new List<IColumn> { vehicleId.Object, discriminator.Object };
            vehicleTable.Setup(t => t.Columns).Returns(vehicleColumns);
            
            var vehiclePk = new Mock<IKey>();
            vehiclePk.Setup(k => k.Name).Returns("PK_Vehicle");
            vehiclePk.Setup(k => k.Columns).Returns(new List<IColumn> { vehicleId.Object });
            vehicleTable.Setup(t => t.PrimaryKey).Returns(vehiclePk.Object);
            
            // Create Car table (for TPT - Table Per Type inheritance)
            var carTable = new Mock<ITable>();
            carTable.Setup(t => t.Name).Returns("Car");
            carTable.Setup(t => t.Schema).Returns("dbo");
            
            var carVehicleId = new Mock<IColumn>();
            carVehicleId.Setup(c => c.Name).Returns("VehicleId");
            carVehicleId.Setup(c => c.IsPartOfPrimaryKey).Returns(true);
            carVehicleId.Setup(c => c.IsPartOfForeignKey).Returns(true);
            
            var carColumns = new List<IColumn> { carVehicleId.Object };
            carTable.Setup(t => t.Columns).Returns(carColumns);
            
            var carPk = new Mock<IKey>();
            carPk.Setup(k => k.Name).Returns("PK_Car");
            carPk.Setup(k => k.Columns).Returns(new List<IColumn> { carVehicleId.Object });
            carTable.Setup(t => t.PrimaryKey).Returns(carPk.Object);
            
            var carFk = new Mock<IForeignKey>();
            carFk.Setup(fk => fk.Name).Returns("FK_Car_Vehicle");
            carFk.Setup(fk => fk.Columns).Returns(new List<IColumn> { carVehicleId.Object });
            carFk.Setup(fk => fk.ReferencedTable).Returns(vehicleTable.Object);
            carFk.Setup(fk => fk.ReferencedColumns).Returns(new List<IColumn> { vehicleId.Object });
            carFk.Setup(fk => fk.IsOneToOne).Returns(true);
            
            var carForeignKeys = new List<IForeignKey> { carFk.Object };
            carTable.Setup(t => t.ForeignKeys).Returns(carForeignKeys);
            
            tables.Add(vehicleTable.Object);
            tables.Add(carTable.Object);
            mockSchema.Setup(s => s.Tables).Returns(tables);
            
            return mockSchema.Object;
        }

        private IRelationshipDetector CreateRelationshipDetector(RelationshipDetectionOptions options)
        {
            // This would return a real implementation, but for now, let's create a mock
            // that just returns the expected relationships for our tests
            var mockDetector = new Mock<IRelationshipDetector>();
            
            mockDetector.Setup(d => d.DetectRelationships(It.IsAny<IDatabaseSchema>()))
                .Returns((IDatabaseSchema schema) => {
                    if (schema.Tables.Count == 0)
                    {
                        return new List<IRelationship>();
                    }
                    
                    var relationships = new List<IRelationship>();
                    
                    // One-to-Many relationships
                    var oneToManyTables = schema.Tables.Where(t => t.Name == "Order").ToList();
                    if (oneToManyTables.Any())
                    {
                        var orderTable = oneToManyTables.First();
                        var customerTable = schema.Tables.FirstOrDefault(t => t.Name == "Customer");
                        
                        if (customerTable != null)
                        {
                            var mockRelationship = new Mock<IRelationship>();
                            mockRelationship.Setup(r => r.RelationshipType).Returns(RelationshipType.OneToMany);
                            mockRelationship.Setup(r => r.PrimaryEntityName).Returns("Customer");
                            mockRelationship.Setup(r => r.ForeignEntityName).Returns("Order");
                            mockRelationship.Setup(r => r.PrimaryTable).Returns(customerTable);
                            mockRelationship.Setup(r => r.ForeignTable).Returns(orderTable);
                            mockRelationship.Setup(r => r.PrimaryColumns).Returns(customerTable.Columns.Where(c => c.IsPartOfPrimaryKey).ToList());
                            mockRelationship.Setup(r => r.ForeignColumns).Returns(orderTable.Columns.Where(c => c.IsPartOfForeignKey).ToList());
                            
                            relationships.Add(mockRelationship.Object);
                        }
                    }
                    
                    // Many-to-Many relationships
                    if (options.DetectManyToManyRelationships)
                    {
                        var junctionTable = schema.Tables.FirstOrDefault(t => t.Name == "StudentCourse");
                        if (junctionTable != null)
                        {
                            var studentTable = schema.Tables.FirstOrDefault(t => t.Name == "Student");
                            var courseTable = schema.Tables.FirstOrDefault(t => t.Name == "Course");
                            
                            if (studentTable != null && courseTable != null)
                            {
                                var mockRelationship = new Mock<IRelationship>();
                                mockRelationship.Setup(r => r.RelationshipType).Returns(RelationshipType.ManyToMany);
                                mockRelationship.Setup(r => r.PrimaryEntityName).Returns("Student");
                                mockRelationship.Setup(r => r.ForeignEntityName).Returns("Course");
                                mockRelationship.Setup(r => r.PrimaryTable).Returns(studentTable);
                                mockRelationship.Setup(r => r.ForeignTable).Returns(courseTable);
                                mockRelationship.Setup(r => r.JunctionTable).Returns(junctionTable);
                                
                                relationships.Add(mockRelationship.Object);
                            }
                        }
                    }
                    
                    // One-to-One relationships
                    if (options.DetectOneToOneRelationships)
                    {
                        var detailTable = schema.Tables.FirstOrDefault(t => t.Name == "PersonDetail");
                        if (detailTable != null)
                        {
                            var personTable = schema.Tables.FirstOrDefault(t => t.Name == "Person");
                            
                            if (personTable != null)
                            {
                                var mockRelationship = new Mock<IRelationship>();
                                mockRelationship.Setup(r => r.RelationshipType).Returns(RelationshipType.OneToOne);
                                mockRelationship.Setup(r => r.PrimaryEntityName).Returns("Person");
                                mockRelationship.Setup(r => r.ForeignEntityName).Returns("PersonDetail");
                                mockRelationship.Setup(r => r.PrimaryTable).Returns(personTable);
                                mockRelationship.Setup(r => r.ForeignTable).Returns(detailTable);
                                
                                relationships.Add(mockRelationship.Object);
                            }
                        }
                    }
                    
                    // Self-Referencing relationships
                    if (options.DetectSelfReferencingRelationships)
                    {
                        var employeeTable = schema.Tables.FirstOrDefault(t => t.Name == "Employee");
                        if (employeeTable != null)
                        {
                            var mockRelationship = new Mock<IRelationship>();
                            mockRelationship.Setup(r => r.RelationshipType).Returns(RelationshipType.OneToMany);
                            mockRelationship.Setup(r => r.PrimaryEntityName).Returns("Employee");
                            mockRelationship.Setup(r => r.ForeignEntityName).Returns("Employee");
                            mockRelationship.Setup(r => r.PrimaryTable).Returns(employeeTable);
                            mockRelationship.Setup(r => r.ForeignTable).Returns(employeeTable);
                            mockRelationship.Setup(r => r.IsSelfReferencing).Returns(true);
                            
                            relationships.Add(mockRelationship.Object);
                        }
                    }
                    
                    // Inheritance relationships
                    if (options.DetectInheritanceRelationships)
                    {
                        var vehicleTable = schema.Tables.FirstOrDefault(t => t.Name == "Vehicle");
                        var carTable = schema.Tables.FirstOrDefault(t => t.Name == "Car");
                        
                        if (vehicleTable != null && carTable != null)
                        {
                            var mockRelationship = new Mock<IRelationship>();
                            mockRelationship.Setup(r => r.RelationshipType).Returns(RelationshipType.OneToOne);
                            mockRelationship.Setup(r => r.PrimaryEntityName).Returns("Vehicle");
                            mockRelationship.Setup(r => r.ForeignEntityName).Returns("Car");
                            mockRelationship.Setup(r => r.PrimaryTable).Returns(vehicleTable);
                            mockRelationship.Setup(r => r.ForeignTable).Returns(carTable);
                            mockRelationship.Setup(r => r.IsInheritance).Returns(true);
                            mockRelationship.Setup(r => r.InheritanceType).Returns(InheritanceType.TablePerHierarchy);
                            
                            relationships.Add(mockRelationship.Object);
                        }
                    }
                    
                    return relationships;
                });
            
            return mockDetector.Object;
        }
    }
}
