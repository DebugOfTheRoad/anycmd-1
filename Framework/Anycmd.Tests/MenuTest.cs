
namespace Anycmd.Tests
{
    using AC.Infra.ViewModels.MenuViewModels;
    using Host;
    using Host.AC.Infra;
    using Host.AC.Infra.Messages;
    using Moq;
    using Repositories;
    using System;
    using System.Linq;
    using Xunit;

    public class MenuTest
    {
        #region MenuSet
        [Fact]
        public void MenuSet()
        {
            var host = TestHelper.GetAppHost();
            Assert.Equal(0, host.MenuSet.Count());

            var entityID = Guid.NewGuid();

            MenuState MenuByID;
            host.Handle(new AddMenuCommand(new MenuCreateInput
            {
                Id = entityID,
                Name = "测试1",
                Description = "test",
                SortCode = 10,
                AppSystemID = host.AppSystemSet.First().Id,
                Icon = null,
                ParentID = null,
                Url = string.Empty
            }));
            Assert.Equal(1, host.MenuSet.Count());
            Assert.True(host.MenuSet.TryGetMenu(entityID, out MenuByID));

            host.Handle(new UpdateMenuCommand(new MenuUpdateInput
            {
                Id = entityID,
                Name = "test2",
                Description = "test",
                SortCode = 10,
                AppSystemID = host.AppSystemSet.First().Id,
                Icon = null,
                Url = string.Empty
            }));
            Assert.Equal(1, host.MenuSet.Count());
            Assert.True(host.MenuSet.TryGetMenu(entityID, out MenuByID));
            Assert.Equal("test2", MenuByID.Name);

            host.Handle(new RemoveMenuCommand(entityID));
            Assert.False(host.MenuSet.TryGetMenu(entityID, out MenuByID));
            Assert.Equal(0, host.MenuSet.Count());
        }
        #endregion

        [Fact]
        public void MenuCanNotRemoveWhenItHasChildMenus()
        {
            var host = TestHelper.GetAppHost();
            Assert.Equal(0, host.MenuSet.Count());

            var entityID = Guid.NewGuid();
            var entityID2 = Guid.NewGuid();

            MenuState MenuByID;
            host.Handle(new AddMenuCommand(new MenuCreateInput
            {
                Id = entityID,
                Name = "测试1",
                Description = "test",
                SortCode = 10,
                AppSystemID = host.AppSystemSet.First().Id,
                Icon = null,
                ParentID = null,
                Url = string.Empty
            }));
            host.Handle(new AddMenuCommand(new MenuCreateInput
            {
                Id = entityID2,
                Name = "测试2",
                Description = "test",
                SortCode = 10,
                AppSystemID = host.AppSystemSet.First().Id,
                Icon = null,
                ParentID = entityID,
                Url = string.Empty
            }));
            Assert.Equal(2, host.MenuSet.Count());
            Assert.NotNull(host.GetRequiredService<IRepository<Menu>>().GetByKey(entityID));
            Assert.NotNull(host.GetRequiredService<IRepository<Menu>>().GetByKey(entityID2));
            Assert.Equal(entityID, host.GetRequiredService<IRepository<Menu>>().GetByKey(entityID2).ParentID.Value);
            Assert.True(host.MenuSet.TryGetMenu(entityID, out MenuByID));
            bool catched = false;
            try
            {
                host.Handle(new RemoveMenuCommand(entityID));
            }
            catch (Exception)
            {
                catched = true;
            }
            finally
            {
                Assert.True(catched);
                Assert.Equal(2, host.MenuSet.Count());
            }
        }

        #region MenuSetShouldRollbackedWhenPersistFailed
        [Fact]
        public void MenuSetShouldRollbackedWhenPersistFailed()
        {
            var host = TestHelper.GetAppHost();
            Assert.Equal(0, host.MenuSet.Count());

            host.Container.RemoveService(typeof(IRepository<Menu>));
            var moMenuRepository = host.GetMoqRepository<Menu, IRepository<Menu>>();
            var entityID1 = Guid.NewGuid();
            var entityID2 = Guid.NewGuid();
            var name = "测试1";
            moMenuRepository.Setup(a => a.Add(It.Is<Menu>(b => b.Id == entityID1))).Throws(new DbException(entityID1.ToString()));
            moMenuRepository.Setup(a => a.Update(It.Is<Menu>(b => b.Id == entityID2))).Throws(new DbException(entityID2.ToString()));
            moMenuRepository.Setup(a => a.Remove(It.Is<Menu>(b => b.Id == entityID2))).Throws(new DbException(entityID2.ToString()));
            moMenuRepository.Setup<Menu>(a => a.GetByKey(entityID1)).Returns(new Menu { Id = entityID1, Name = name });
            moMenuRepository.Setup<Menu>(a => a.GetByKey(entityID2)).Returns(new Menu { Id = entityID2, Name = name });
            host.Container.AddService(typeof(IRepository<Menu>), moMenuRepository.Object);

            bool catched = false;
            try
            {
                host.Handle(new AddMenuCommand(new MenuCreateInput
                {
                    Id = entityID1,
                    AppSystemID = host.AppSystemSet.First().Id,
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
                Assert.Equal(0, host.MenuSet.Count());
            }

            host.Handle(new AddMenuCommand(new MenuCreateInput
            {
                Id = entityID2,
                AppSystemID = host.AppSystemSet.First().Id,
                Name = name
            }));
            Assert.Equal(1, host.MenuSet.Count());

            catched = false;
            try
            {
                host.Handle(new UpdateMenuCommand(new MenuUpdateInput
                {
                    Id = entityID2,
                    AppSystemID = host.AppSystemSet.First().Id,
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
                Assert.Equal(1, host.MenuSet.Count());
                MenuState Menu;
                Assert.True(host.MenuSet.TryGetMenu(entityID2, out Menu));
                Assert.Equal(name, Menu.Name);
            }

            catched = false;
            try
            {
                host.Handle(new RemoveMenuCommand(entityID2));
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
                MenuState Menu;
                Assert.True(host.MenuSet.TryGetMenu(entityID2, out Menu));
                Assert.Equal(1, host.MenuSet.Count());
            }
        }
        #endregion
    }
}
