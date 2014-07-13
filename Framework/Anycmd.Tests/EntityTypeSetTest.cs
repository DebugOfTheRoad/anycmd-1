
namespace Anycmd.Tests
{
    using AC.Infra.ViewModels.EntityTypeViewModels;
    using Exceptions;
    using Host;
    using Host.AC.Infra;
    using Host.AC.Infra.Messages;
    using Moq;
    using Repositories;
    using System;
    using System.Linq;
    using Xunit;

    public class EntityTypeSetTest
    {
        #region EntityTypeSet
        [Fact]
        public void EntityTypeSet()
        {
            var host = TestHelper.GetAppHost();
            Assert.Equal(0, host.EntityTypeSet.Count());

            string codespace = "Test";
            var entityTypeID = Guid.NewGuid();
            var propertyID = Guid.NewGuid();

            EntityTypeState entityTypeByID;
            EntityTypeState entityTypeByCode;
            host.Handle(new AddEntityTypeCommand(new EntityTypeCreateInput
            {
                Id = entityTypeID,
                Code = "EntityType1",
                Name = "测试1",
                Codespace = codespace,
                DatabaseID = host.Rdbs.First().Database.Id,
                Description = string.Empty,
                DeveloperID = host.SysUsers.GetDevAccounts().First().Id,
                EditHeight = 100,
                EditWidth = 100,
                IsOrganizational = false,
                SchemaName = string.Empty,
                SortCode = 10,
                TableName = string.Empty
            }));
            Assert.Equal(1, host.EntityTypeSet.Count());
            Assert.True(host.EntityTypeSet.TryGetEntityType(entityTypeID, out entityTypeByID));
            Assert.True(host.EntityTypeSet.TryGetEntityType(codespace, "EntityType1", out entityTypeByCode));
            Assert.Equal(entityTypeByCode, entityTypeByID);
            Assert.True(ReferenceEquals(entityTypeByID, entityTypeByCode));

            host.Handle(new UpdateEntityTypeCommand(new EntityTypeUpdateInput
            {
                Id = entityTypeID,
                Name = "test2",
                Code = "EntityType2",
                Codespace = "test",
                DatabaseID = host.Rdbs.First().Database.Id,
                Description = string.Empty,
                DeveloperID = host.SysUsers.GetDevAccounts().First().Id,
                EditWidth = 100,
                EditHeight = 100,
                IsOrganizational = false,
                SchemaName = string.Empty,
                SortCode = 100,
                TableName = string.Empty
            }));
            Assert.Equal(1, host.EntityTypeSet.Count());
            Assert.True(host.EntityTypeSet.TryGetEntityType(entityTypeID, out entityTypeByID));
            Assert.True(host.EntityTypeSet.TryGetEntityType(codespace, "EntityType2", out entityTypeByCode));
            Assert.Equal(entityTypeByCode, entityTypeByID);
            Assert.True(ReferenceEquals(entityTypeByID, entityTypeByCode));
            Assert.Equal("test2", entityTypeByID.Name);
            Assert.Equal("EntityType2", entityTypeByID.Code);

            host.Handle(new RemoveEntityTypeCommand(entityTypeID));
            Assert.False(host.EntityTypeSet.TryGetEntityType(entityTypeID, out entityTypeByID));
            Assert.False(host.EntityTypeSet.TryGetEntityType(codespace, "EntityType2", out entityTypeByCode));
            Assert.Equal(0, host.EntityTypeSet.Count());

            // 开始测试Property
            host.Handle(new AddEntityTypeCommand(new EntityTypeCreateInput
            {
                Id = entityTypeID,
                Code = "EntityType1",
                Name = "测试1",
                Codespace = codespace,
                DatabaseID = host.Rdbs.First().Database.Id,
                Description = string.Empty,
                DeveloperID = host.SysUsers.GetDevAccounts().First().Id,
                EditHeight = 100,
                EditWidth = 100,
                IsOrganizational = false,
                SchemaName = string.Empty,
                SortCode = 10,
                TableName = string.Empty
            }));
            Assert.Equal(1, host.EntityTypeSet.Count());
            Assert.True(host.EntityTypeSet.TryGetEntityType(entityTypeID, out entityTypeByID));
            PropertyState propertyByID;
            PropertyState propertyByCode;
            host.Handle(new AddPropertyCommand(new PropertyCreateInput
            {
                Id = propertyID,
                DicID = null,
                ForeignPropertyID = null,
                GuideWords = string.Empty,
                Icon = string.Empty,
                InputType = string.Empty,
                IsDetailsShow = false,
                IsDeveloperOnly = false,
                IsInput = false,
                IsTotalLine = false,
                MaxLength = null,
                EntityTypeID = entityTypeID,
                SortCode = 0,
                Description = string.Empty,
                Code = "Property1",
                Name = "测试1"
            }));
            Assert.Equal(1, host.EntityTypeSet.GetProperties(entityTypeByID).Count());
            Assert.True(host.EntityTypeSet.TryGetProperty(propertyID, out propertyByID));
            Assert.True(host.EntityTypeSet.TryGetProperty(entityTypeByID, "Property1", out propertyByCode));
            Assert.Equal(propertyByCode, propertyByID);
            Assert.True(ReferenceEquals(propertyByID, propertyByCode));

            host.Handle(new UpdatePropertyCommand(new PropertyUpdateInput
            {
                Id = propertyID,
                Name = "test2",
                Code = "Property2"
            }));
            Assert.Equal(1, host.EntityTypeSet.GetProperties(entityTypeByID).Count);
            Assert.True(host.EntityTypeSet.TryGetProperty(propertyID, out propertyByID));
            Assert.True(host.EntityTypeSet.TryGetProperty(entityTypeByID, "Property2", out propertyByCode));
            Assert.Equal(propertyByCode, propertyByID);
            Assert.True(ReferenceEquals(propertyByID, propertyByCode));
            Assert.Equal("test2", propertyByID.Name);
            Assert.Equal("Property2", propertyByID.Code);

            host.Handle(new RemovePropertyCommand(propertyID));
            Assert.False(host.EntityTypeSet.TryGetProperty(propertyID, out propertyByID));
            Assert.False(host.EntityTypeSet.TryGetProperty(entityTypeByID, "Property2", out propertyByCode));
            Assert.Equal(0, host.EntityTypeSet.GetProperties(entityTypeByID).Count);
        }
        #endregion

        #region CanNotDeleteEntityTypeWhenItHasProperties
        [Fact]
        public void CanNotDeleteEntityTypeWhenItHasProperties()
        {
            var host = TestHelper.GetAppHost();
            Assert.Equal(0, host.EntityTypeSet.Count());

            var entityTypeID = Guid.NewGuid();

            host.Handle(new AddEntityTypeCommand(new EntityTypeCreateInput
            {
                Id = entityTypeID,
                Code = "EntityType1",
                Name = "测试1",
                Codespace = "Test",
                DatabaseID = host.Rdbs.First().Database.Id,
                Description = string.Empty,
                DeveloperID = host.SysUsers.GetDevAccounts().First().Id,
                EditHeight = 100,
                EditWidth = 100,
                IsOrganizational = false,
                SchemaName = string.Empty,
                SortCode = 10,
                TableName = string.Empty
            }));
            Assert.Equal(1, host.EntityTypeSet.Count());

            host.Handle(new AddPropertyCommand(new PropertyCreateInput
            {
                Id = Guid.NewGuid(),
                DicID = null,
                ForeignPropertyID = null,
                GuideWords = string.Empty,
                Icon = string.Empty,
                InputType = string.Empty,
                IsDetailsShow = false,
                IsDeveloperOnly = false,
                IsInput = false,
                IsTotalLine = false,
                MaxLength = null,
                EntityTypeID = entityTypeID,
                SortCode = 0,
                Description = string.Empty,
                Code = "Property1",
                Name = "测试1"
            }));

            bool catched = false;
            try
            {
                host.Handle(new RemoveEntityTypeCommand(entityTypeID));
            }
            catch (ValidationException)
            {
                catched = true;
            }
            finally
            {
                Assert.True(catched);
                EntityTypeState EntityType;
                Assert.True(host.EntityTypeSet.TryGetEntityType(entityTypeID, out EntityType));
            }
        }
        #endregion

        #region EntityTypeSetShouldRollbackedWhenPersistFailed
        [Fact]
        public void EntityTypeSetShouldRollbackedWhenPersistFailed()
        {
            var host = TestHelper.GetAppHost();
            Assert.Equal(0, host.EntityTypeSet.Count());

            host.Container.RemoveService(typeof(IRepository<EntityType>));
            var moEntityTypeRepository = host.GetMoqRepository<EntityType, IRepository<EntityType>>();
            var entityID1 = Guid.NewGuid();
            var entityID2 = Guid.NewGuid();
            moEntityTypeRepository.Setup(a => a.Add(It.Is<EntityType>(b => b.Id == entityID1))).Throws(new DbException(entityID1.ToString()));
            moEntityTypeRepository.Setup(a => a.Update(It.Is<EntityType>(b => b.Id == entityID2))).Throws(new DbException(entityID2.ToString()));
            moEntityTypeRepository.Setup(a => a.Remove(It.Is<EntityType>(b => b.Id == entityID2))).Throws(new DbException(entityID2.ToString()));
            moEntityTypeRepository.Setup<EntityType>(a => a.GetByKey(entityID1)).Returns(new EntityType
            {
                Id = entityID1,
                Name = "test1",
                Code = "EntityType1",
                Codespace = "test",
                DatabaseID = host.Rdbs.First().Database.Id,
                Description = string.Empty,
                DeveloperID = host.SysUsers.GetDevAccounts().First().Id,
                EditWidth = 100,
                EditHeight = 100,
                IsOrganizational = false,
                SchemaName = string.Empty,
                SortCode = 100,
                TableName = string.Empty
            });
            moEntityTypeRepository.Setup<EntityType>(a => a.GetByKey(entityID2)).Returns(new EntityType
            {
                Id = entityID2,
                Name = "test2",
                Code = "EntityType2",
                Codespace = "test",
                DatabaseID = host.Rdbs.First().Database.Id,
                Description = string.Empty,
                DeveloperID = host.SysUsers.GetDevAccounts().First().Id,
                EditWidth = 100,
                EditHeight = 100,
                IsOrganizational = false,
                SchemaName = string.Empty,
                SortCode = 100,
                TableName = string.Empty
            });
            host.Container.AddService(typeof(IRepository<EntityType>), moEntityTypeRepository.Object);


            bool catched = false;
            try
            {
                host.Handle(new AddEntityTypeCommand(new EntityTypeCreateInput
                {
                    Id = entityID1,
                    Code = "EntityType1",
                    Name = "测试1",
                    Codespace = "Test",
                    DatabaseID = host.Rdbs.First().Database.Id,
                    Description = string.Empty,
                    DeveloperID = host.SysUsers.GetDevAccounts().First().Id,
                    EditHeight = 100,
                    EditWidth = 100,
                    IsOrganizational = false,
                    SchemaName = string.Empty,
                    SortCode = 10,
                    TableName = string.Empty
                }));
            }
            catch (Exception e)
            {
                Assert.Equal(e.GetType(), typeof(DbException));
                catched = true;
                Assert.Equal(entityID1.ToString(), e.Message);
            }
            finally
            {
                Assert.True(catched);
                Assert.Equal(0, host.EntityTypeSet.Count());
            }

            host.Handle(new AddEntityTypeCommand(new EntityTypeCreateInput
            {
                Id = entityID2,
                Code = "EntityType2",
                Name = "测试2",
                Codespace = "Test",
                DatabaseID = host.Rdbs.First().Database.Id,
                Description = string.Empty,
                DeveloperID = host.SysUsers.GetDevAccounts().First().Id,
                EditHeight = 100,
                EditWidth = 100,
                IsOrganizational = false,
                SchemaName = string.Empty,
                SortCode = 10,
                TableName = string.Empty
            }));
            Assert.Equal(1, host.EntityTypeSet.Count());

            catched = false;
            try
            {
                host.Handle(new UpdateEntityTypeCommand(new EntityTypeUpdateInput
                {
                    Id = entityID2,
                    Name = "test2",
                    Code = "EntityType2",
                    Codespace = "test",
                    DatabaseID = host.Rdbs.First().Database.Id,
                    Description = string.Empty,
                    DeveloperID = host.SysUsers.GetDevAccounts().First().Id,
                    EditWidth = 100,
                    EditHeight = 100,
                    IsOrganizational = false,
                    SchemaName = string.Empty,
                    SortCode = 100,
                    TableName = string.Empty
                }));
            }
            catch (Exception e)
            {
                Assert.Equal(e.GetType(), typeof(DbException));
                catched = true;
                Assert.Equal(entityID2.ToString(), e.Message);
            }
            finally
            {
                Assert.True(catched);
                Assert.Equal(1, host.EntityTypeSet.Count());
                EntityTypeState EntityType;
                Assert.True(host.EntityTypeSet.TryGetEntityType(entityID2, out EntityType));
                Assert.Equal("EntityType2", EntityType.Code);
            }

            catched = false;
            try
            {
                host.Handle(new RemoveEntityTypeCommand(entityID2));
            }
            catch (Exception e)
            {
                Assert.Equal(e.GetType(), typeof(DbException));
                catched = true;
                Assert.Equal(entityID2.ToString(), e.Message);
            }
            finally
            {
                Assert.True(catched);
                EntityTypeState EntityType;
                Assert.True(host.EntityTypeSet.TryGetEntityType(entityID2, out EntityType));
                Assert.Equal(1, host.EntityTypeSet.Count());
            }
        }
        #endregion
    }
}
