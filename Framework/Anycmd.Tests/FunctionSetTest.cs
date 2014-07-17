
namespace Anycmd.Tests
{
    using AC.Infra.ViewModels.AppSystemViewModels;
    using Host;
    using Host.AC.Infra;
    using Host.AC.Infra.Messages;
    using Moq;
    using Repositories;
    using System;
    using System.Linq;
    using Xunit;

    public class FunctionSetTest
    {
        #region FunctionSet
        [Fact]
        public void FunctionSet()
        {
            var host = TestHelper.GetAppHost();
            Assert.Equal(0, host.FunctionSet.Count());

            var entityID = Guid.NewGuid();

            FunctionState functionByID;
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
            ResourceTypeState resource;
            Assert.True(host.ResourceSet.TryGetResource(host.ResourceSet.First().Id, out resource));
            Assert.Equal(1, host.FunctionSet.Count());
            Assert.True(host.FunctionSet.TryGetFunction(entityID, out functionByID));

            host.Handle(new UpdateFunctionCommand(new FunctionUpdateInput
            {
                Id = entityID,
                Description = "test2",
                Code = "fun2",
                DeveloperID = host.SysUsers.GetDevAccounts().First().Id,
                IsEnabled = 1,
                IsManaged = false,
                SortCode = 10
            }));
            Assert.Equal(1, host.FunctionSet.Count());
            Assert.True(host.FunctionSet.TryGetFunction(entityID, out functionByID));
            Assert.Equal("test2", functionByID.Description);
            Assert.Equal("fun2", functionByID.Code);

            host.Handle(new RemoveFunctionCommand(entityID));
            Assert.False(host.FunctionSet.TryGetFunction(entityID, out functionByID));
            Assert.Equal(0, host.FunctionSet.Count());
        }
        #endregion

        [Fact]
        public void FunctionCodeMustBeUnique()
        {
            var host = TestHelper.GetAppHost();
            Assert.Equal(0, host.FunctionSet.Count());

            var entityID = Guid.NewGuid();

            FunctionState functionByID;
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
            ResourceTypeState resource;
            Assert.True(host.ResourceSet.TryGetResource(host.ResourceSet.First().Id, out resource));
            Assert.Equal(1, host.FunctionSet.Count());
            Assert.True(host.FunctionSet.TryGetFunction(entityID, out functionByID));
            bool catched = false;
            try
            {
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

        #region FunctionSetShouldRollbackedWhenPersistFailed
        [Fact]
        public void FunctionSetShouldRollbackedWhenPersistFailed()
        {
            var host = TestHelper.GetAppHost();
            Assert.Equal(0, host.FunctionSet.Count());

            host.Container.RemoveService(typeof(IRepository<Function>));
            var moFunctionRepository = host.GetMoqRepository<Function, IRepository<Function>>();
            var entityID1 = Guid.NewGuid();
            var entityID2 = Guid.NewGuid();
            var appsystemID = Guid.NewGuid();
            moFunctionRepository.Setup(a => a.Add(It.Is<Function>(b => b.Id == entityID1))).Throws(new DbException(entityID1.ToString()));
            moFunctionRepository.Setup(a => a.Update(It.Is<Function>(b => b.Id == entityID2))).Throws(new DbException(entityID2.ToString()));
            moFunctionRepository.Setup(a => a.Remove(It.Is<Function>(b => b.Id == entityID2))).Throws(new DbException(entityID2.ToString()));
            moFunctionRepository.Setup<Function>(a => a.GetByKey(entityID1)).Returns(new Function
            {
                Id = entityID1,
                ResourceTypeID = host.ResourceSet.First().Id,
                DeveloperID = host.SysUsers.GetDevAccounts().First().Id
            });
            moFunctionRepository.Setup<Function>(a => a.GetByKey(entityID2)).Returns(new Function
            {
                Id = entityID2,
                ResourceTypeID = host.ResourceSet.First().Id,
                DeveloperID = host.SysUsers.GetDevAccounts().First().Id
            });
            host.Container.AddService(typeof(IRepository<Function>), moFunctionRepository.Object);

            host.Handle(new AddAppSystemCommand(new AppSystemCreateInput
            {
                Id = appsystemID,
                Code = "app1",
                Name = "测试1",
                PrincipalID = host.SysUsers.GetDevAccounts().First().Id
            }));

            bool catched = false;
            try
            {
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
                Assert.Equal(0, host.FunctionSet.Count());
            }

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
            Assert.Equal(1, host.FunctionSet.Count());

            catched = false;
            try
            {
                host.Handle(new UpdateFunctionCommand(new FunctionUpdateInput
                {
                    Id = entityID2,
                    Description = "test2",
                    Code = "fun",
                    DeveloperID = host.SysUsers.GetDevAccounts().First().Id,
                    IsEnabled = 1,
                    IsManaged = false,
                    SortCode = 10
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
                Assert.Equal(1, host.FunctionSet.Count());
                FunctionState function;
                Assert.True(host.FunctionSet.TryGetFunction(entityID2, out function));
                Assert.Equal("fun2", function.Code);
            }

            catched = false;
            try
            {
                host.Handle(new RemoveFunctionCommand(entityID2));
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
                FunctionState function;
                Assert.True(host.FunctionSet.TryGetFunction(entityID2, out function));
                Assert.Equal(1, host.FunctionSet.Count());
            }
        }
        #endregion
    }
}
