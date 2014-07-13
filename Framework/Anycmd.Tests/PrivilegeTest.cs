
namespace Anycmd.Tests
{
    using AC;
    using AC.Infra.ViewModels.AppSystemViewModels;
    using AC.ViewModels.GroupViewModels;
    using AC.ViewModels.PrivilegeViewModels;
    using AC.ViewModels.RoleViewModels;
    using Host;
    using Host.AC;
    using Host.AC.Identity;
    using Host.AC.Infra.Messages;
    using Host.AC.Messages;
    using Repositories;
    using System;
    using System.Linq;
    using Xunit;

    public class PrivilegeTest
    {
        #region AccountSubjectTypePrivilege
        [Fact]
        public void AccountSubjectTypePrivilege()
        {
            var host = TestHelper.GetAppHost();
            Assert.Equal(0, host.PrivilegeSet.Count());

            Guid groupID = Guid.NewGuid();
            host.Handle(new AddGroupCommand(new GroupCreateInput
            {
                Id = groupID,
                Name = "测试1",
                CategoryCode = "test",
                Description = "test",
                IsEnabled = 1,
                OrganizationCode = "111111",
                ShortName = "",
                SortCode = 10,
                TypeCode = "AC"
            }));
            Guid accountID = Guid.NewGuid();
            host.GetRequiredService<IRepository<Account>>().Add(new Account
            {
                Id = accountID,
                Code="test",
                Name="test"
            });
            host.GetRequiredService<IRepository<Account>>().Context.Commit();
            var entityID = Guid.NewGuid();

            host.Handle(new AddPrivilegeBigramCommand(new PrivilegeBigramCreateInput
            {
                Id = entityID,
                SubjectInstanceID = accountID,
                SubjectType = ACSubjectType.Account.ToString(),// 主体是账户
                PrivilegeConstraint = null,
                PrivilegeOrientation = 1,
                ObjectInstanceID = groupID,
                ObjectType = ACObjectType.Group.ToString()
            }));
            Assert.Equal(0, host.PrivilegeSet.Count()); // 主体为账户的权限记录不驻留在内存中所以为0
            var privilegeBigram = host.GetRequiredService<IRepository<PrivilegeBigram>>().FindAll().FirstOrDefault(a => a.Id == entityID);
            Assert.NotNull(privilegeBigram);
            Assert.Equal(accountID, privilegeBigram.SubjectInstanceID);
            Assert.Equal(groupID, privilegeBigram.ObjectInstanceID);

            host.Handle(new UpdatePrivilegeBigramCommand(new PrivilegeBigramUpdateInput
            {
                Id = entityID,
                PrivilegeConstraint = "this is a test"
            }));
            Assert.Equal(0, host.PrivilegeSet.Count());// 主体为账户的权限记录不驻留在内存中所以为0
            Assert.Equal("this is a test", host.GetRequiredService<IRepository<PrivilegeBigram>>().FindAll().FirstOrDefault(a => a.Id == entityID).PrivilegeConstraint);

            host.Handle(new RemovePrivilegeBigramCommand(entityID));
            Assert.Null(host.GetRequiredService<IRepository<PrivilegeBigram>>().FindAll().FirstOrDefault(a => a.Id == entityID));
        }
        #endregion

        [Fact]
        public void RoleSubjectTypePrivilege()
        {
            var host = TestHelper.GetAppHost();
            var roleID = Guid.NewGuid();

            RoleState roleByID;
            host.Handle(new AddRoleCommand(new RoleCreateInput
            {
                Id = roleID,
                Name = "测试1",
                CategoryCode = "test",
                Description = "test",
                IsEnabled = 1,
                SortCode = 10,
                Icon = null
            }));
            Assert.Equal(1, host.RoleSet.Count());
            Assert.True(host.RoleSet.TryGetRole(roleID, out roleByID));

            var functionID = Guid.NewGuid();

            FunctionState functionByID;
            host.Handle(new AddFunctionCommand(new FunctionCreateInput
            {
                Id = functionID,
                Code = "fun1",
                Description = string.Empty,
                DeveloperID = host.SysUsers.GetDevAccounts().First().Id,
                IsEnabled = 1,
                IsManaged = true,
                ResourceTypeID = host.ResourceSet.First().Id,
                SortCode = 10
            }));
            ResourceTypeState resource;
            Assert.True(host.ResourceSet.TryGetResource(host.ResourceSet.First().Id, out resource));
            Assert.Equal(1, host.FunctionSet.Count());
            Assert.True(host.FunctionSet.TryGetFunction(functionID, out functionByID));
            var entityID = Guid.NewGuid();

            host.Handle(new AddPrivilegeBigramCommand(new PrivilegeBigramCreateInput
            {
                Id = entityID,
                SubjectInstanceID = roleID,
                SubjectType = ACSubjectType.Role.ToString(),// 主体是角色
                PrivilegeConstraint = null,
                PrivilegeOrientation = 1,
                ObjectInstanceID = functionID,
                ObjectType = ACObjectType.Function.ToString()
            }));
            PrivilegeBigramState privilegeBigram = host.PrivilegeSet.First(a => a.Id == entityID);
            Assert.NotNull(privilegeBigram);
            Assert.NotNull(host.GetRequiredService<IRepository<PrivilegeBigram>>().FindAll().FirstOrDefault(a => a.Id == entityID));
            Assert.Equal(roleID, privilegeBigram.SubjectInstanceID);
            Assert.Equal(functionID, privilegeBigram.ObjectInstanceID);

            host.Handle(new UpdatePrivilegeBigramCommand(new PrivilegeBigramUpdateInput
            {
                Id = entityID,
                PrivilegeConstraint = "this is a test"
            }));
            Assert.Equal("this is a test", host.PrivilegeSet.Single(a => a.Id == entityID).PrivilegeConstraint);

            host.Handle(new RemovePrivilegeBigramCommand(entityID));
            Assert.Null(host.PrivilegeSet.FirstOrDefault(a => a.Id == entityID));
        }

        [Fact]
        public void OrganizationSubjectTypePrivilege()
        {
            // TODO:
        }

        [Fact]
        public void SubjectTypeTest()
        {
            var host = TestHelper.GetAppHost();
            Assert.Equal(0, host.PrivilegeSet.Count());

            bool catched = false;
            try
            {
                host.Handle(new AddPrivilegeBigramCommand(new PrivilegeBigramCreateInput
                {
                    Id = Guid.NewGuid(),
                    SubjectInstanceID = Guid.NewGuid(),
                    SubjectType = "Group",// 用户类别的主体类型只有Account、Organization、Role。Group不是合法的主体类型所以会报错。
                    PrivilegeConstraint = null,
                    PrivilegeOrientation = 1,
                    ObjectInstanceID = Guid.NewGuid(),
                    ObjectType = ACObjectType.Group.ToString()
                }));
            }
            catch (Exception)
            {
                catched = true;
            }
            finally
            {
                Assert.True(catched);
                Assert.Equal(0, host.PrivilegeSet.Count());
            }
            catched = false;
            try
            {
                host.Handle(new AddPrivilegeBigramCommand(new PrivilegeBigramCreateInput
                {
                    Id = Guid.NewGuid(),
                    SubjectInstanceID = Guid.NewGuid(),
                    SubjectType = "InvalidSubjectType",// 非法的AC元素类型
                    PrivilegeConstraint = null,
                    PrivilegeOrientation = 1,
                    ObjectInstanceID = Guid.NewGuid(),
                    ObjectType = ACObjectType.Group.ToString()
                }));
            }
            catch (Exception)
            {
                catched = true;
            }
            finally
            {
                Assert.True(catched);
                Assert.Equal(0, host.PrivilegeSet.Count());
            }
            catched = false;
            try
            {
                host.Handle(new AddPrivilegeBigramCommand(new PrivilegeBigramCreateInput
                {
                    Id = Guid.NewGuid(),
                    SubjectInstanceID = Guid.NewGuid(),// 标识为它的账户不存在，应报错
                    SubjectType = "Account",
                    PrivilegeConstraint = null,
                    PrivilegeOrientation = 1,
                    ObjectInstanceID = Guid.NewGuid(),
                    ObjectType = ACObjectType.Group.ToString()
                }));
            }
            catch (Exception)
            {
                catched = true;
            }
            finally
            {
                Assert.True(catched);
                Assert.Equal(0, host.PrivilegeSet.Count());
            }

            Guid groupID = Guid.NewGuid();
            host.Handle(new AddGroupCommand(new GroupCreateInput
            {
                Id = groupID,
                Name = "测试1",
                CategoryCode = "test",
                Description = "test",
                IsEnabled = 1,
                OrganizationCode = "111111",
                ShortName = "",
                SortCode = 10,
                TypeCode = "AC"
            }));
            Guid accountID = Guid.NewGuid();
            host.GetRequiredService<IRepository<Account>>().Add(new Account
            {
                Id = accountID,
                Code = "test",
                Name = "test"
            });
            host.GetRequiredService<IRepository<Account>>().Context.Commit();
            catched = false;
            try
            {
                host.Handle(new AddPrivilegeBigramCommand(new PrivilegeBigramCreateInput
                {
                    Id = Guid.NewGuid(),
                    SubjectInstanceID = accountID,
                    SubjectType = "Account",
                    PrivilegeConstraint = null,
                    PrivilegeOrientation = 1,
                    ObjectInstanceID = groupID,
                    ObjectType = "InvalidObjectType"// 非法的AC客体类型应报错
                }));
            }
            catch (Exception)
            {
                catched = true;
            }
            finally
            {
                Assert.True(catched);
                Assert.Equal(0, host.PrivilegeSet.Count());
            }
        }
    }
}
