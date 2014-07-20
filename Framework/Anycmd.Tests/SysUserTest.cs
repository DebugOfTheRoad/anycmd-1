
namespace Anycmd.Tests
{
    using AC.Identity.ViewModels.AccountViewModels;
    using Host;
    using Host.AC.Identity;
    using Host.AC.Identity.Messages;
    using Moq;
    using Repositories;
    using System;
    using Xunit;

    public class SysUserTest
    {
        [Fact]
        public void SysUserSet()
        {
            var host = TestHelper.GetAppHost();
            Assert.True(host.SysUsers.GetDevAccounts().Count == 1);
            Guid accountID = Guid.NewGuid();
            host.GetRequiredService<IRepository<Account>>().Add(new Account
            {
                Id = accountID,
                Code = "test",
                Name = "test",
                LoginName = "anycmd"
            });
            host.GetRequiredService<IRepository<Account>>().Context.Commit();
            Assert.True(host.SysUsers.GetDevAccounts().Count == 1);
            host.Handle(new AddDeveloperCommand(accountID));
            AccountState developer;
            Assert.True(host.SysUsers.GetDevAccounts().Count == 2);
            Assert.True(host.SysUsers.TryGetDevAccount(accountID, out developer));
            Assert.True(host.SysUsers.TryGetDevAccount("anycmd", out developer));

            host.Handle(new RemoveDeveloperCommand(accountID));
            Assert.True(host.SysUsers.GetDevAccounts().Count == 1);
            Assert.False(host.SysUsers.TryGetDevAccount(accountID, out developer));
            Assert.False(host.SysUsers.TryGetDevAccount("anycmd", out developer));

            bool catched = false;
            try
            {
                host.Handle(new AddDeveloperCommand(Guid.NewGuid()));// 将不存在的账户设为开发人员时应引发异常
            }
            catch (Exception)
            {
                catched = true;
            }
            finally
            {
                Assert.True(catched);
            }
        }

        #region SysUserSetShouldRollbackedWhenPersistFailed
        [Fact]
        public void SysUserSetShouldRollbackedWhenPersistFailed()
        {
            var host = TestHelper.GetAppHost();
            Assert.Equal(1, host.SysUsers.GetDevAccounts().Count);

            host.RemoveService(typeof(IRepository<Account>));
            host.RemoveService(typeof(IRepository<DeveloperID>));
            var moAccountRepository = host.GetMoqRepository<Account, IRepository<Account>>();
            var moDeveloperRepository = host.GetMoqRepository<DeveloperID, IRepository<DeveloperID>>();
            var entityID1 = Guid.NewGuid();
            var entityID2 = Guid.NewGuid();
            var name = "测试1";
            var loginName1 = "anycmd1";
            var loginName2 = "anycmd2";
            moDeveloperRepository.Setup(a => a.Add(It.Is<DeveloperID>(b => b.Id == entityID1))).Throws(new DbException(entityID1.ToString()));
            moDeveloperRepository.Setup(a => a.Remove(It.Is<DeveloperID>(b => b.Id == entityID2))).Throws(new DbException(entityID2.ToString()));
            moAccountRepository.Setup<Account>(a => a.GetByKey(entityID1)).Returns(new Account { Id = entityID1, Name = name, LoginName = loginName1 });
            moAccountRepository.Setup<Account>(a => a.GetByKey(entityID2)).Returns(new Account { Id = entityID2, Name = name, LoginName = loginName2 });
            moDeveloperRepository.Setup<DeveloperID>(a => a.GetByKey(entityID1)).Returns(new DeveloperID { Id = entityID1 });
            moDeveloperRepository.Setup<DeveloperID>(a => a.GetByKey(entityID2)).Returns(new DeveloperID { Id = entityID2 });
            host.AddService(typeof(IRepository<Account>), moAccountRepository.Object);
            host.AddService(typeof(IRepository<DeveloperID>), moDeveloperRepository.Object);

            host.GetRequiredService<IRepository<Account>>().Add(new Account
            {
                Id = entityID1,
                Code = "test",
                Name = "test",
                LoginName = loginName1
            });
            host.GetRequiredService<IRepository<Account>>().Add(new Account
            {
                Id = entityID2,
                Code = "tes2t",
                Name = "test2",
                LoginName = loginName2
            });
            host.GetRequiredService<IRepository<Account>>().Context.Commit();
            Assert.True(host.SysUsers.GetDevAccounts().Count == 1);
            bool catched = false;
            try
            {
                host.Handle(new AddDeveloperCommand(entityID1));
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
                Assert.Equal(1, host.SysUsers.GetDevAccounts().Count);
            }

            host.Handle(new AddDeveloperCommand(entityID2));
            Assert.Equal(2, host.SysUsers.GetDevAccounts().Count);

            host.Handle(new UpdateAccountCommand(new AccountUpdateInput
            {
                Id = entityID2,
                Name = "test2"
            }));
            Assert.True(catched);
            Assert.Equal(2, host.SysUsers.GetDevAccounts().Count);

            catched = false;
            try
            {
                host.Handle(new RemoveDeveloperCommand(entityID2));
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
                Assert.Equal(2, host.SysUsers.GetDevAccounts().Count);
            }
        }
        #endregion
    }
}
