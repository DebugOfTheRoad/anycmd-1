
namespace Anycmd.Tests
{
    using AC.ViewModels.GroupViewModels;
    using Host;
    using Host.AC;
    using Host.AC.Messages;
    using Moq;
    using Repositories;
    using System;
    using System.Linq;
    using Xunit;

    public class GroupSetTest
    {
        #region GroupSet
        [Fact]
        public void GroupSet()
        {
            var host = TestHelper.GetAppHost();
            Assert.Equal(0, host.GroupSet.Count());

            var entityID = Guid.NewGuid();

            GroupState groupByID;
            host.Handle(new AddGroupCommand(new GroupCreateInput
            {
                Id = entityID,
                Name = "测试1",
                CategoryCode = "test",
                Description = "test",
                IsEnabled = 1,
                OrganizationCode = "111111",
                ShortName = "",
                SortCode = 10,
                TypeCode = "AC"
            }));
            Assert.Equal(1, host.GroupSet.Count());
            Assert.True(host.GroupSet.TryGetGroup(entityID, out groupByID));

            host.Handle(new UpdateGroupCommand(new GroupUpdateInput
            {
                Id = entityID,
                Name = "test2",
                CategoryCode = "test",
                Description = "test",
                IsEnabled = 1,
                OrganizationCode = "111111",
                ShortName = "",
                SortCode = 10,
                TypeCode = "AC"
            }));
            Assert.Equal(1, host.GroupSet.Count());
            Assert.True(host.GroupSet.TryGetGroup(entityID, out groupByID));
            Assert.Equal("test2", groupByID.Name);

            host.Handle(new RemoveGroupCommand(entityID));
            Assert.False(host.GroupSet.TryGetGroup(entityID, out groupByID));
            Assert.Equal(0, host.GroupSet.Count());
        }
        #endregion

        #region GroupSetShouldRollbackedWhenPersistFailed
        [Fact]
        public void GroupSetShouldRollbackedWhenPersistFailed()
        {
            var host = TestHelper.GetAppHost();
            Assert.Equal(0, host.GroupSet.Count());

            host.Container.RemoveService(typeof(IRepository<Group>));
            var moGroupRepository = host.GetMoqRepository<Group, IRepository<Group>>();
            var entityID1 = Guid.NewGuid();
            var entityID2 = Guid.NewGuid();
            var name = "测试1";
            moGroupRepository.Setup(a => a.Add(It.Is<Group>(b => b.Id == entityID1))).Throws(new DbException(entityID1.ToString()));
            moGroupRepository.Setup(a => a.Update(It.Is<Group>(b => b.Id == entityID2))).Throws(new DbException(entityID2.ToString()));
            moGroupRepository.Setup(a => a.Remove(It.Is<Group>(b => b.Id == entityID2))).Throws(new DbException(entityID2.ToString()));
            moGroupRepository.Setup<Group>(a => a.GetByKey(entityID1)).Returns(new Group { Id = entityID1, Name = name });
            moGroupRepository.Setup<Group>(a => a.GetByKey(entityID2)).Returns(new Group { Id = entityID2, Name = name });
            host.Container.AddService(typeof(IRepository<Group>), moGroupRepository.Object);

            bool catched = false;
            try
            {
                host.Handle(new AddGroupCommand(new GroupCreateInput
                {
                    Id = entityID1,
                    Name = name,
                    TypeCode = "AC"
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
                Assert.Equal(0, host.GroupSet.Count());
            }

            host.Handle(new AddGroupCommand(new GroupCreateInput
            {
                Id = entityID2,
                Name = name,
                TypeCode = "AC"
            }));
            Assert.Equal(1, host.GroupSet.Count());

            catched = false;
            try
            {
                host.Handle(new UpdateGroupCommand(new GroupUpdateInput
                {
                    Id = entityID2,
                    Name = "test2",
                    TypeCode = "AC"
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
                Assert.Equal(1, host.GroupSet.Count());
                GroupState Group;
                Assert.True(host.GroupSet.TryGetGroup(entityID2, out Group));
                Assert.Equal(name, Group.Name);
            }

            catched = false;
            try
            {
                host.Handle(new RemoveGroupCommand(entityID2));
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
                GroupState Group;
                Assert.True(host.GroupSet.TryGetGroup(entityID2, out Group));
                Assert.Equal(1, host.GroupSet.Count());
            }
        }
        #endregion
    }
}
