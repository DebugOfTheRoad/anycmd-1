
namespace Anycmd.Tests
{
    using AC.Infra.ViewModels.AppSystemViewModels;
    using AC.Infra.ViewModels.ButtonViewModels;
    using AC.Infra.ViewModels.PageViewModels;
    using Exceptions;
    using Host;
    using Host.AC.Infra;
    using Host.AC.Infra.Messages;
    using Moq;
    using Repositories;
    using System;
    using System.Linq;
    using Xunit;

    public class ButtonSetTest
    {
        #region ButtonSet
        [Fact]
        public void ButtonSet()
        {
            var host = TestHelper.GetAppHost();
            Assert.Equal(0, host.ButtonSet.Count());

            var entityID = Guid.NewGuid();

            ButtonState buttonByID;
            ButtonState buttonByCode;
            host.Handle(new AddButtonCommand(new ButtonCreateInput
            {
                Id = entityID,
                Code = "btn1",
                Name = "测试1"
            }));
            Assert.Equal(1, host.ButtonSet.Count());
            Assert.True(host.ButtonSet.ContainsButton(entityID));
            Assert.True(host.ButtonSet.ContainsButton("btn1"));
            Assert.True(host.ButtonSet.TryGetButton(entityID, out buttonByID));
            Assert.True(host.ButtonSet.TryGetButton("btn1", out buttonByCode));
            Assert.Equal(buttonByCode, buttonByID);
            Assert.True(ReferenceEquals(buttonByID, buttonByCode));

            host.Handle(new UpdateButtonCommand(new ButtonUpdateInput
            {
                Id = entityID,
                Name = "test2",
                Code = "btn2"
            }));
            Assert.Equal(1, host.ButtonSet.Count());
            Assert.True(host.ButtonSet.ContainsButton(entityID));
            Assert.True(host.ButtonSet.ContainsButton("btn2"));
            Assert.True(host.ButtonSet.TryGetButton(entityID, out buttonByID));
            Assert.True(host.ButtonSet.TryGetButton("btn2", out buttonByCode));
            Assert.Equal(buttonByCode, buttonByID);
            Assert.True(ReferenceEquals(buttonByID, buttonByCode));
            Assert.Equal("test2", buttonByID.Name);
            Assert.Equal("btn2", buttonByID.Code);

            host.Handle(new RemoveButtonCommand(entityID));
            Assert.False(host.ButtonSet.ContainsButton(entityID));
            Assert.False(host.ButtonSet.ContainsButton("btn2"));
            Assert.False(host.ButtonSet.TryGetButton(entityID, out buttonByID));
            Assert.False(host.ButtonSet.TryGetButton("btn2", out buttonByCode));
            Assert.Equal(0, host.ButtonSet.Count());
        }
        #endregion

        #region CanNotDeleteButtonWhenItHasPageButtons
        [Fact]
        public void CanNotDeleteButtonWhenItHasPageButtons()
        {
            var host = TestHelper.GetAppHost();
            Assert.Equal(0, host.ButtonSet.Count());

            var entityID = Guid.NewGuid();
            var appSystemID = Guid.NewGuid();
            var functionID = Guid.NewGuid();
            var pageID = functionID;
            var pageButtonID = Guid.NewGuid();

            host.Handle(new AddButtonCommand(new ButtonCreateInput
            {
                Id = entityID,
                Code = "app1",
                Name = "测试1"
            }));
            Assert.Equal(1, host.ButtonSet.Count());
            host.Handle(new AddAppSystemCommand(new AppSystemCreateInput
            {
                Id = appSystemID,
                Code = "app1",
                Name = "app1",
                PrincipalID = host.SysUsers.GetDevAccounts().First().Id
            }));
            host.Handle(new AddFunctionCommand(new FunctionCreateInput
            {
                Id = functionID,
                ResourceTypeID = host.ResourceTypeSet.First().Id,
                DeveloperID = host.SysUsers.GetDevAccounts().First().Id,
                Description = string.Empty,
                Code = "function1",
                IsEnabled = 1,
                IsManaged = true,
                SortCode = 0
            }));
            host.Handle(new AddPageCommand(new PageCreateInput
            {
                Id = functionID
            }));
            host.Handle(new AddPageButtonCommand(new PageButtonCreateInput
            {
                Id = pageButtonID,
                ButtonID = entityID,
                PageID = pageID,
                FunctionID = null,
                IsEnabled = 1
            }));

            bool catched = false;
            try
            {
                host.Handle(new RemoveButtonCommand(entityID));
            }
            catch (ValidationException)
            {
                catched = true;
            }
            finally
            {
                Assert.True(catched);
                ButtonState button;
                Assert.True(host.ButtonSet.TryGetButton(entityID, out button));
            }

            {
                host.Handle(new RemovePageButtonCommand(pageButtonID));
                host.Handle(new RemoveButtonCommand(entityID));
                ButtonState button;
                Assert.False(host.ButtonSet.TryGetButton(entityID, out button));
            }
        }
        #endregion

        #region ButtonSetShouldRollbackedWhenPersistFailed
        [Fact]
        public void ButtonSetShouldRollbackedWhenPersistFailed()
        {
            var host = TestHelper.GetAppHost();
            Assert.Equal(0, host.ButtonSet.Count());

            var moButtonRepository = host.GetMoqRepository<Button, IRepository<Button>>();
            var entityID1 = Guid.NewGuid();
            var entityID2 = Guid.NewGuid();
            var code = "btn1";
            var name = "测试1";
            host.RemoveService(typeof(IRepository<Button>));
            moButtonRepository.Setup(a => a.Add(It.Is<Button>(b => b.Id == entityID1))).Throws(new DbException(entityID1.ToString()));
            moButtonRepository.Setup(a => a.Update(It.Is<Button>(b => b.Id == entityID2))).Throws(new DbException(entityID2.ToString()));
            moButtonRepository.Setup(a => a.Remove(It.Is<Button>(b => b.Id == entityID2))).Throws(new DbException(entityID2.ToString()));
            moButtonRepository.Setup<Button>(a => a.GetByKey(entityID1)).Returns(new Button { Id = entityID1, Code = code, Name = name });
            moButtonRepository.Setup<Button>(a => a.GetByKey(entityID2)).Returns(new Button { Id = entityID2, Code = code, Name = name });
            host.AddService(typeof(IRepository<Button>), moButtonRepository.Object);


            bool catched = false;
            try
            {
                host.Handle(new AddButtonCommand(new ButtonCreateInput
                {
                    Id = entityID1,
                    Code = code,
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
                Assert.Equal(0, host.ButtonSet.Count());
            }

            host.Handle(new AddButtonCommand(new ButtonCreateInput
            {
                Id = entityID2,
                Code = code,
                Name = name
            }));
            Assert.Equal(1, host.ButtonSet.Count());

            catched = false;
            try
            {
                host.Handle(new UpdateButtonCommand(new ButtonUpdateInput
                {
                    Id = entityID2,
                    Name = "test2",
                    Code = "btn2"
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
                Assert.Equal(1, host.ButtonSet.Count());
                ButtonState button;
                Assert.True(host.ButtonSet.TryGetButton(entityID2, out button));
                Assert.Equal(code, button.Code);
            }

            catched = false;
            try
            {
                host.Handle(new RemoveButtonCommand(entityID2));
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
                ButtonState button;
                Assert.True(host.ButtonSet.TryGetButton(entityID2, out button));
                Assert.Equal(1, host.ButtonSet.Count());
            }
        }
        #endregion
    }
}
