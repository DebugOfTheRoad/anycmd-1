
namespace Anycmd.Tests
{
    using AC.ViewModels.PrivilegeViewModels;
    using AC.ViewModels.RoleViewModels;
    using Anycmd.AC;
    using Host;
    using Host.AC;
    using Host.AC.Messages;
    using Moq;
    using Repositories;
    using System;
    using System.Linq;
    using Xunit;

    public class RoleTest
    {
        #region RoleSet
        [Fact]
        public void RoleSet()
        {
            var host = TestHelper.GetAppHost();
            Assert.Equal(0, host.RoleSet.Count());

            var entityID = Guid.NewGuid();

            RoleState roleByID;
            host.Handle(new AddRoleCommand(new RoleCreateInput
            {
                Id = entityID,
                Name = "测试1",
                CategoryCode = "test",
                Description = "test",
                IsEnabled = 1,
                SortCode = 10,
                Icon = null
            }));
            Assert.Equal(1, host.RoleSet.Count());
            Assert.True(host.RoleSet.TryGetRole(entityID, out roleByID));

            host.Handle(new UpdateRoleCommand(new RoleUpdateInput
            {
                Id = entityID,
                Name = "test2",
                CategoryCode = "test",
                Description = "test",
                IsEnabled = 1,
                SortCode = 10,
                Icon = null
            }));
            Assert.Equal(1, host.RoleSet.Count());
            Assert.True(host.RoleSet.TryGetRole(entityID, out roleByID));
            Assert.Equal("test2", roleByID.Name);

            host.Handle(new RemoveRoleCommand(entityID));
            Assert.False(host.RoleSet.TryGetRole(entityID, out roleByID));
            Assert.Equal(0, host.RoleSet.Count());
        }
        #endregion

        #region RoleSetShouldRollbackedWhenPersistFailed
        [Fact]
        public void RoleSetShouldRollbackedWhenPersistFailed()
        {
            var host = TestHelper.GetAppHost();
            Assert.Equal(0, host.RoleSet.Count());

            host.Container.RemoveService(typeof(IRepository<Role>));
            var moRoleRepository = host.GetMoqRepository<Role, IRepository<Role>>();
            var entityID1 = Guid.NewGuid();
            var entityID2 = Guid.NewGuid();
            var name = "测试1";
            moRoleRepository.Setup(a => a.Add(It.Is<Role>(b => b.Id == entityID1))).Throws(new DbException(entityID1.ToString()));
            moRoleRepository.Setup(a => a.Update(It.Is<Role>(b => b.Id == entityID2))).Throws(new DbException(entityID2.ToString()));
            moRoleRepository.Setup(a => a.Remove(It.Is<Role>(b => b.Id == entityID2))).Throws(new DbException(entityID2.ToString()));
            moRoleRepository.Setup<Role>(a => a.GetByKey(entityID1)).Returns(new Role { Id = entityID1, Name = name });
            moRoleRepository.Setup<Role>(a => a.GetByKey(entityID2)).Returns(new Role { Id = entityID2, Name = name });
            host.Container.AddService(typeof(IRepository<Role>), moRoleRepository.Object);

            bool catched = false;
            try
            {
                host.Handle(new AddRoleCommand(new RoleCreateInput
                {
                    Id = entityID1,
                    Name = name
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
                Assert.Equal(0, host.RoleSet.Count());
            }

            host.Handle(new AddRoleCommand(new RoleCreateInput
            {
                Id = entityID2,
                Name = name
            }));
            Assert.Equal(1, host.RoleSet.Count());

            catched = false;
            try
            {
                host.Handle(new UpdateRoleCommand(new RoleUpdateInput
                {
                    Id = entityID2,
                    Name = "test2"
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
                Assert.Equal(1, host.RoleSet.Count());
                RoleState Role;
                Assert.True(host.RoleSet.TryGetRole(entityID2, out Role));
                Assert.Equal(name, Role.Name);
            }

            catched = false;
            try
            {
                host.Handle(new RemoveRoleCommand(entityID2));
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
                RoleState Role;
                Assert.True(host.RoleSet.TryGetRole(entityID2, out Role));
                Assert.Equal(1, host.RoleSet.Count());
            }
        }
        #endregion

        [Fact]
        public void TestRoleHierarchy()
        {
            var host = TestHelper.GetAppHost();
            Assert.Equal(0, host.RoleSet.Count());

            var roleID1 = Guid.NewGuid();
            host.Handle(new AddRoleCommand(new RoleCreateInput
            {
                Id = roleID1,
                Name = "role1",
                CategoryCode = "test",
                Description = "test",
                IsEnabled = 1,
                SortCode = 10,
                Icon = null
            }));

            var roleID2 = Guid.NewGuid();
            host.Handle(new AddRoleCommand(new RoleCreateInput
            {
                Id = roleID2,
                Name = "role2",
                CategoryCode = "test",
                Description = "test",
                IsEnabled = 1,
                SortCode = 10,
                Icon = null
            }));
            Assert.Equal(2, host.RoleSet.Count());
            RoleState role1;
            RoleState role2;
            Assert.True(host.RoleSet.TryGetRole(roleID1, out role1));
            Assert.True(host.RoleSet.TryGetRole(roleID2, out role2));
            Assert.Equal(0, host.RoleSet.GetDescendantRoles(role1).Count);
            Assert.Equal(0, host.RoleSet.GetDescendantRoles(role2).Count);

            var privilegeID = Guid.NewGuid();
            host.Handle(new AddPrivilegeBigramCommand(new PrivilegeBigramCreateInput
            {
                Id = privilegeID,
                SubjectInstanceID = roleID1,
                SubjectType = ACSubjectType.Role.ToString(),// 主体是角色
                PrivilegeConstraint = null,
                PrivilegeOrientation = 1,
                ObjectInstanceID = roleID2,
                ObjectType = ACObjectType.Role.ToString()// 客体也是角色
            }));
            PrivilegeBigramState privilegeBigram = host.PrivilegeSet.First(a => a.Id == privilegeID);
            Assert.NotNull(privilegeBigram);
            Assert.NotNull(host.GetRequiredService<IRepository<PrivilegeBigram>>().FindAll().FirstOrDefault(a => a.Id == privilegeID));
            Assert.Equal(1, host.RoleSet.GetDescendantRoles(role1).Count);
            Assert.Equal(0, host.RoleSet.GetDescendantRoles(role2).Count);

            var roleID3 = Guid.NewGuid();
            host.Handle(new AddRoleCommand(new RoleCreateInput
            {
                Id = roleID3,
                Name = "role3",
                CategoryCode = "test",
                Description = "test",
                IsEnabled = 1,
                SortCode = 10,
                Icon = null
            }));
            privilegeID = Guid.NewGuid();
            host.Handle(new AddPrivilegeBigramCommand(new PrivilegeBigramCreateInput
            {
                Id = privilegeID,
                SubjectInstanceID = roleID2,
                SubjectType = ACSubjectType.Role.ToString(),// 主体是角色
                PrivilegeConstraint = null,
                PrivilegeOrientation = 1,
                ObjectInstanceID = roleID3,
                ObjectType = ACObjectType.Role.ToString()// 客体也是角色
            }));
            RoleState role3;
            Assert.True(host.RoleSet.TryGetRole(roleID3, out role3));
            Assert.Equal(2, host.RoleSet.GetDescendantRoles(role1).Count);
            Assert.Equal(1, host.RoleSet.GetDescendantRoles(role2).Count);
            var roleID4 = Guid.NewGuid();
            host.Handle(new AddRoleCommand(new RoleCreateInput
            {
                Id = roleID4,
                Name = "role4",
                CategoryCode = "test",
                Description = "test",
                IsEnabled = 1,
                SortCode = 10,
                Icon = null
            }));
            privilegeID = Guid.NewGuid();
            host.Handle(new AddPrivilegeBigramCommand(new PrivilegeBigramCreateInput
            {
                Id = privilegeID,
                SubjectInstanceID = roleID3,
                SubjectType = ACSubjectType.Role.ToString(),// 主体是角色
                PrivilegeConstraint = null,
                PrivilegeOrientation = 1,
                ObjectInstanceID = roleID4,
                ObjectType = ACObjectType.Role.ToString()// 客体也是角色
            }));
            Assert.Equal(3, host.RoleSet.GetDescendantRoles(role1).Count);
            Assert.Equal(2, host.RoleSet.GetDescendantRoles(role2).Count);
            Assert.Equal(1, host.RoleSet.GetDescendantRoles(role3).Count);
        }
    }
}
