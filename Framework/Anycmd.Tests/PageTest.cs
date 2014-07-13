
namespace Anycmd.Tests
{
    using AC.Infra.ViewModels.AppSystemViewModels;
    using AC.Infra.ViewModels.PageViewModels;
    using Host;
    using Host.AC.Infra;
    using Host.AC.Infra.Messages;
    using Moq;
    using Repositories;
    using System;
    using System.Linq;
    using Xunit;

    public class PageTest
    {
        #region PageSet
        [Fact]
        public void PageSet()
        {
            var host = TestHelper.GetAppHost();
            Assert.Equal(0, host.PageSet.Count());

            var entityID = Guid.NewGuid();

            host.Handle(new AddFunctionCommand(new FunctionCreateInput
            {
                Id = entityID,
                Code = "fun1",
                Description = string.Empty,
                DeveloperID = host.SysUsers.GetDevAccounts().First().Id,
                IsEnabled = 1,
                IsManaged = true,
                ResourceTypeID = host.ResourceSet.First().Id,
                SortCode = 10
            }));
            FunctionState functionByID;
            Assert.Equal(1, host.FunctionSet.Count());
            Assert.True(host.FunctionSet.TryGetFunction(entityID, out functionByID));
            PageState PageByID;
            host.Handle(new AddPageCommand(new PageCreateInput
            {
                Id = entityID,
                Icon = null,
                Tooltip = null
            }));
            Assert.Equal(1, host.PageSet.Count());
            Assert.True(host.PageSet.TryGetPage(entityID, out PageByID));
            bool catched = false;
            try
            {
                host.Handle(new AddPageCommand(new PageCreateInput
                {
                    Id = Guid.NewGuid(),
                    Icon = null,
                    Tooltip = null
                }));
            }
            catch (Exception)
            {
                catched = true;
            }
            finally
            {
                Assert.True(catched);
            }
            host.Handle(new UpdatePageCommand(new PageUpdateInput
            {
                Id = entityID,
                Icon = null,
                Tooltip = null
            }));
            Assert.Equal(1, host.PageSet.Count());
            Assert.True(host.PageSet.TryGetPage(entityID, out PageByID));

            host.Handle(new RemovePageCommand(entityID));
            Assert.False(host.PageSet.TryGetPage(entityID, out PageByID));
            Assert.Equal(0, host.PageSet.Count());
        }
        #endregion

        #region PageSetShouldRollbackedWhenPersistFailed
        [Fact]
        public void PageSetShouldRollbackedWhenPersistFailed()
        {
            var host = TestHelper.GetAppHost();
            Assert.Equal(0, host.PageSet.Count());

            host.Container.RemoveService(typeof(IRepository<Page>));
            var moPageRepository = host.GetMoqRepository<Page, IRepository<Page>>();
            var entityID1 = Guid.NewGuid();
            var entityID2 = Guid.NewGuid();
            moPageRepository.Setup(a => a.Add(It.Is<Page>(b => b.Id == entityID1))).Throws(new DbException(entityID1.ToString()));
            moPageRepository.Setup(a => a.Update(It.Is<Page>(b => b.Id == entityID2))).Throws(new DbException(entityID2.ToString()));
            moPageRepository.Setup(a => a.Remove(It.Is<Page>(b => b.Id == entityID2))).Throws(new DbException(entityID2.ToString()));
            moPageRepository.Setup<Page>(a => a.GetByKey(entityID1)).Returns(new Page { Id = entityID1 });
            moPageRepository.Setup<Page>(a => a.GetByKey(entityID2)).Returns(new Page { Id = entityID2 });
            host.Container.AddService(typeof(IRepository<Page>), moPageRepository.Object);

            host.Handle(new AddFunctionCommand(new FunctionCreateInput
            {
                Id = entityID1,
                Code = "fun1",
                Description = string.Empty,
                DeveloperID = host.SysUsers.GetDevAccounts().First().Id,
                IsEnabled = 1,
                IsManaged = true,
                ResourceTypeID = host.ResourceSet.First().Id,
                SortCode = 10
            }));
            host.Handle(new AddFunctionCommand(new FunctionCreateInput
            {
                Id = entityID2,
                Code = "fun2",
                Description = string.Empty,
                DeveloperID = host.SysUsers.GetDevAccounts().First().Id,
                IsEnabled = 1,
                IsManaged = true,
                ResourceTypeID = host.ResourceSet.First().Id,
                SortCode = 10
            }));
            FunctionState functionByID;
            Assert.Equal(2, host.FunctionSet.Count());
            Assert.True(host.FunctionSet.TryGetFunction(entityID1, out functionByID));
            Assert.True(host.FunctionSet.TryGetFunction(entityID2, out functionByID));

            bool catched = false;
            try
            {
                host.Handle(new AddPageCommand(new PageCreateInput
                {
                    Id = entityID1
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
                Assert.Equal(0, host.PageSet.Count());
            }

            host.Handle(new AddPageCommand(new PageCreateInput
            {
                Id = entityID2
            }));
            Assert.Equal(1, host.PageSet.Count());

            catched = false;
            try
            {
                host.Handle(new UpdatePageCommand(new PageUpdateInput
                {
                    Id = entityID2
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
                Assert.Equal(1, host.PageSet.Count());
                PageState Page;
                Assert.True(host.PageSet.TryGetPage(entityID2, out Page));
            }

            catched = false;
            try
            {
                host.Handle(new RemovePageCommand(entityID2));
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
                PageState Page;
                Assert.True(host.PageSet.TryGetPage(entityID2, out Page));
                Assert.Equal(1, host.PageSet.Count());
            }
        }
        #endregion
    }
}
