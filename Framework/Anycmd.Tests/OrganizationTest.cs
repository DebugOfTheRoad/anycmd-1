
namespace Anycmd.Tests
{
    using AC.Infra.ViewModels.OrganizationViewModels;
    using Host;
    using Host.AC.Infra;
    using Host.AC.Infra.Messages;
    using Moq;
    using Repositories;
    using System;
    using System.Linq;
    using Xunit;

    public class OrganizationTest
    {
        #region OrganizationSet
        [Fact]
        public void OrganizationSet()
        {
            var host = TestHelper.GetAppHost();
            Assert.Equal(1, host.OrganizationSet.Count());
            Assert.Equal(OrganizationState.VirtualRoot, host.OrganizationSet.First());

            var entityID = Guid.NewGuid();

            OrganizationState OrganizationByID;
            host.Handle(new AddOrganizationCommand(new OrganizationCreateInput
            {
                Id = entityID,
                Code = "100",
                Name = "测试1",
                Description = "test",
                SortCode = 10,
                Icon = null,
            }));
            Assert.Equal(2, host.OrganizationSet.Count());
            Assert.True(host.OrganizationSet.TryGetOrganization(entityID, out OrganizationByID));

            host.Handle(new UpdateOrganizationCommand(new OrganizationUpdateInput
            {
                Id = entityID,
                Code = "110",
                Name = "test2",
                Description = "test",
                SortCode = 10,
                Icon = null,
            }));
            Assert.Equal(2, host.OrganizationSet.Count());
            Assert.True(host.OrganizationSet.TryGetOrganization(entityID, out OrganizationByID));
            Assert.Equal("test2", OrganizationByID.Name);

            host.Handle(new RemoveOrganizationCommand(entityID));
            Assert.False(host.OrganizationSet.TryGetOrganization(entityID, out OrganizationByID));
            Assert.Equal(1, host.OrganizationSet.Count());
        }
        #endregion

        [Fact]
        public void OrganizationCodeMustBeUnique()
        {
            var host = TestHelper.GetAppHost();
            Assert.Equal(1, host.OrganizationSet.Count());

            var entityID = Guid.NewGuid();
            var entityID2 = Guid.NewGuid();

            OrganizationState OrganizationByID;
            host.Handle(new AddOrganizationCommand(new OrganizationCreateInput
            {
                Id = entityID,
                Code = "100",
                Name = "测试1",
                Description = "test",
                SortCode = 10,
                Icon = null,
            }));
            Assert.Equal(2, host.OrganizationSet.Count());
            Assert.True(host.OrganizationSet.TryGetOrganization(entityID, out OrganizationByID));
            bool catched = false;
            try
            {
                host.Handle(new AddOrganizationCommand(new OrganizationCreateInput
                {
                    Id = entityID2,
                    ParentCode = null,
                    Code = "100",
                    Name = "测试2",
                    Description = "test",
                    SortCode = 10,
                    Icon = null,
                }));
                host.Handle(new AddOrganizationCommand(new OrganizationCreateInput
                {
                    Id = entityID2,
                    ParentCode = "100",
                    Code = "100",
                    Name = "测试2",
                    Description = "test",
                    SortCode = 10,
                    Icon = null,
                }));
            }
            catch (Exception)
            {
                catched = true;
            }
            finally
            {
                Assert.True(catched);
                Assert.Equal(2, host.OrganizationSet.Count());
            }
        }

        [Fact]
        public void OrganizationCanNotRemoveWhenItHasChildOrganizations()
        {
            var host = TestHelper.GetAppHost();
            Assert.Equal(1, host.OrganizationSet.Count());

            var entityID = Guid.NewGuid();
            var entityID2 = Guid.NewGuid();

            OrganizationState OrganizationByID;
            host.Handle(new AddOrganizationCommand(new OrganizationCreateInput
            {
                Id = entityID,
                Code = "100",
                Name = "测试1",
                Description = "test",
                SortCode = 10,
                Icon = null,
            }));
            host.Handle(new AddOrganizationCommand(new OrganizationCreateInput
            {
                Id = entityID2,
                ParentCode = "100",
                Code = "100100",
                Name = "测试2",
                Description = "test",
                SortCode = 10,
                Icon = null,
            }));
            Assert.Equal(3, host.OrganizationSet.Count());
            Assert.True(host.OrganizationSet.TryGetOrganization(entityID, out OrganizationByID));
            bool catched = false;
            try
            {
                host.Handle(new RemoveOrganizationCommand(entityID));
            }
            catch (Exception)
            {
                catched = true;
            }
            finally
            {
                Assert.True(catched);
                Assert.Equal(3, host.OrganizationSet.Count());
            }
        }

        #region OrganizationSetShouldRollbackedWhenPersistFailed
        [Fact]
        public void OrganizationSetShouldRollbackedWhenPersistFailed()
        {
            var host = TestHelper.GetAppHost();
            Assert.Equal(1, host.OrganizationSet.Count());
            Assert.Equal(OrganizationState.VirtualRoot, host.OrganizationSet.First());

            host.Container.RemoveService(typeof(IRepository<Organization>));
            var moOrganizationRepository = host.GetMoqRepository<Organization, IRepository<Organization>>();
            var entityID1 = Guid.NewGuid();
            var entityID2 = Guid.NewGuid();
            var name = "测试1";
            moOrganizationRepository.Setup(a => a.Add(It.Is<Organization>(b => b.Id == entityID1))).Throws(new DbException(entityID1.ToString()));
            moOrganizationRepository.Setup(a => a.Update(It.Is<Organization>(b => b.Id == entityID2))).Throws(new DbException(entityID2.ToString()));
            moOrganizationRepository.Setup(a => a.Remove(It.Is<Organization>(b => b.Id == entityID2))).Throws(new DbException(entityID2.ToString()));
            moOrganizationRepository.Setup<Organization>(a => a.GetByKey(entityID1)).Returns(new Organization { Id = entityID1, Name = name });
            moOrganizationRepository.Setup<Organization>(a => a.GetByKey(entityID2)).Returns(new Organization { Id = entityID2, Name = name });
            host.Container.AddService(typeof(IRepository<Organization>), moOrganizationRepository.Object);

            bool catched = false;
            try
            {
                host.Handle(new AddOrganizationCommand(new OrganizationCreateInput
                {
                    Id = entityID1,
                    Code = "100",
                    Description = "test",
                    SortCode = 10,
                    Icon = null,
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
                Assert.Equal(1, host.OrganizationSet.Count());
            }

            host.Handle(new AddOrganizationCommand(new OrganizationCreateInput
            {
                Id = entityID2,
                Code = "100",
                Description = "test",
                SortCode = 10,
                Icon = null,
                Name = name
            }));
            Assert.Equal(2, host.OrganizationSet.Count());

            catched = false;
            try
            {
                host.Handle(new UpdateOrganizationCommand(new OrganizationUpdateInput
                {
                    Id = entityID2,
                    Code = "100",
                    Description = "test",
                    SortCode = 10,
                    Icon = null,
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
                Assert.Equal(2, host.OrganizationSet.Count());
                OrganizationState Organization;
                Assert.True(host.OrganizationSet.TryGetOrganization(entityID2, out Organization));
                Assert.Equal(name, Organization.Name);
            }

            catched = false;
            try
            {
                host.Handle(new RemoveOrganizationCommand(entityID2));
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
                OrganizationState Organization;
                Assert.True(host.OrganizationSet.TryGetOrganization(entityID2, out Organization));
                Assert.Equal(2, host.OrganizationSet.Count());
            }
        }
        #endregion
    }
}
