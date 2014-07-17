
namespace Anycmd.Tests
{
    using AC.Infra.ViewModels.AppSystemViewModels;
    using AC.Infra.ViewModels.MenuViewModels;
    using Exceptions;
    using Host;
    using Host.AC.Infra;
    using Host.AC.Infra.Messages;
    using Moq;
    using Repositories;
    using System;
    using System.Linq;
    using Xunit;

    public class AppSystemTest
    {
        #region AppSystemSet
        [Fact]
        public void AppSystemSet()
        {
            var host = TestHelper.GetAppHost();
            Assert.Equal(1, host.AppSystemSet.Count());

            var entityID = Guid.NewGuid();

            AppSystemState appSystemByID;
            AppSystemState appSystemByCode;
            host.Handle(new AddAppSystemCommand(new AppSystemCreateInput
            {
                Id = entityID,
                Code = "app1",
                Name = "测试1",
                PrincipalID = host.SysUsers.GetDevAccounts().First().Id
            }));
            Assert.Equal(2, host.AppSystemSet.Count());
            Assert.True(host.AppSystemSet.TryGetAppSystem(entityID, out appSystemByID));
            Assert.True(host.AppSystemSet.TryGetAppSystem("app1", out appSystemByCode));
            Assert.Equal(appSystemByCode, appSystemByID);
            Assert.True(ReferenceEquals(appSystemByID, appSystemByCode));
            host.Handle(new UpdateAppSystemCommand(new AppSystemUpdateInput
            {
                Id = entityID,
                Name = "test2",
                Code = "app2",
                PrincipalID = host.SysUsers.GetDevAccounts().First().Id
            }));
            Assert.Equal(2, host.AppSystemSet.Count());
            Assert.True(host.AppSystemSet.TryGetAppSystem(entityID, out appSystemByID));
            Assert.True(host.AppSystemSet.TryGetAppSystem("app2", out appSystemByCode));
            Assert.Equal(appSystemByCode, appSystemByID);
            Assert.True(ReferenceEquals(appSystemByID, appSystemByCode));
            Assert.Equal("test2", appSystemByID.Name);
            Assert.Equal("app2", appSystemByID.Code);

            Assert.NotNull(host.GetRequiredService<IRepository<AppSystem>>().GetByKey(entityID));
            host.Handle(new RemoveAppSystemCommand(entityID));
            Assert.False(host.AppSystemSet.TryGetAppSystem(entityID, out appSystemByID));
            Assert.False(host.AppSystemSet.TryGetAppSystem("app2", out appSystemByCode));
            Assert.Equal(1, host.AppSystemSet.Count());
        }
        #endregion

        #region CanNotDeleteAppSystemWhenItHasMenus
        [Fact]
        public void CanNotDeleteAppSystemWhenItHasMenus()
        {
            var host = TestHelper.GetAppHost();
            Assert.Equal(1, host.AppSystemSet.Count());

            var entityID = Guid.NewGuid();

            host.Handle(new AddAppSystemCommand(new AppSystemCreateInput
            {
                Id = entityID,
                Code = "app1",
                Name = "测试1",
                PrincipalID = host.SysUsers.GetDevAccounts().First().Id
            }));
            Assert.Equal(2, host.AppSystemSet.Count());

            host.Handle(new AddMenuCommand(new MenuCreateInput
            {
                Id = Guid.NewGuid(),
                AppSystemID = entityID,
                Name = "menu1",
                SortCode = 10,
                Url = string.Empty,
                Description = string.Empty,
                Icon = string.Empty,
                ParentID = null
            }));

            bool catched = false;
            try
            {
                host.Handle(new RemoveAppSystemCommand(entityID));
            }
            catch (ValidationException)
            {
                catched = true;
            }
            finally
            {
                Assert.True(catched);
                AppSystemState appSystem;
                Assert.True(host.AppSystemSet.TryGetAppSystem(entityID, out appSystem));
            }
        }
        #endregion

        #region AppSystemSetShouldRollbackedWhenPersistFailed
        [Fact]
        public void AppSystemSetShouldRollbackedWhenPersistFailed()
        {
            var host = TestHelper.GetAppHost();
            Assert.Equal(1, host.AppSystemSet.Count());

            var moAppSystemRepository = host.GetMoqRepository<AppSystem, IRepository<AppSystem>>();
            var entityID1 = Guid.NewGuid();
            var entityID2 = Guid.NewGuid();
            var code = "app1";
            var name = "测试1";
            host.Container.RemoveService(typeof(IRepository<AppSystem>));
            moAppSystemRepository.Setup(a => a.Add(It.Is<AppSystem>(b => b.Id == entityID1))).Throws(new DbException(entityID1.ToString()));
            moAppSystemRepository.Setup(a => a.Update(It.Is<AppSystem>(b => b.Id == entityID2))).Throws(new DbException(entityID2.ToString()));
            moAppSystemRepository.Setup(a => a.Remove(It.Is<AppSystem>(b => b.Id == entityID2))).Throws(new DbException(entityID2.ToString()));
            moAppSystemRepository.Setup<AppSystem>(a => a.GetByKey(entityID1)).Returns(new AppSystem { Id = entityID1, Code = code, Name = name });
            moAppSystemRepository.Setup<AppSystem>(a => a.GetByKey(entityID2)).Returns(new AppSystem { Id = entityID2, Code = code, Name = name });
            host.Container.AddService(typeof(IRepository<AppSystem>), moAppSystemRepository.Object);

            bool catched = false;
            try
            {
                host.Handle(new AddAppSystemCommand(new AppSystemCreateInput
                {
                    Id = entityID1,
                    Code = code,
                    Name = name,
                    PrincipalID = host.SysUsers.GetDevAccounts().First().Id
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
                Assert.Equal(1, host.AppSystemSet.Count());
            }

            host.Handle(new AddAppSystemCommand(new AppSystemCreateInput
            {
                Id = entityID2,
                Code = code,
                Name = name,
                PrincipalID = host.SysUsers.GetDevAccounts().First().Id
            }));
            Assert.Equal(2, host.AppSystemSet.Count());

            catched = false;
            try
            {
                host.Handle(new UpdateAppSystemCommand(new AppSystemUpdateInput
                {
                    Id = entityID2,
                    Name = "test2",
                    Code = "app2",
                    PrincipalID = host.SysUsers.GetDevAccounts().First().Id
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
                Assert.Equal(2, host.AppSystemSet.Count());
                AppSystemState appSystem;
                Assert.True(host.AppSystemSet.TryGetAppSystem(entityID2, out appSystem));
                Assert.Equal(code, appSystem.Code);
            }

            catched = false;
            try
            {
                host.Handle(new RemoveAppSystemCommand(entityID2));
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
                AppSystemState appSystem;
                Assert.True(host.AppSystemSet.TryGetAppSystem(entityID2, out appSystem));
                Assert.Equal(2, host.AppSystemSet.Count());
            }
        }
        #endregion
    }
}
