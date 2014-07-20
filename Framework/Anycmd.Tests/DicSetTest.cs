
namespace Anycmd.Tests
{
    using AC.Infra.ViewModels.DicViewModels;
    using Exceptions;
    using Host;
    using Host.AC.Infra;
    using Host.AC.Infra.Messages;
    using Moq;
    using Repositories;
    using System;
    using System.Linq;
    using Xunit;

    public class DicSetTest
    {
        #region DicSet
        [Fact]
        public void DicSet()
        {
            var host = TestHelper.GetAppHost();
            Assert.Equal(0, host.DicSet.Count());

            var dicID = Guid.NewGuid();
            var dicItemID = Guid.NewGuid();

            DicState dicByID;
            DicState dicByCode;
            host.Handle(new AddDicCommand(new DicCreateInput
            {
                Id = dicID,
                Code = "dic1",
                Name = "测试1"
            }));
            Assert.Equal(1, host.DicSet.Count());
            Assert.True(host.DicSet.TryGetDic(dicID, out dicByID));
            Assert.True(host.DicSet.TryGetDic("dic1", out dicByCode));
            Assert.Equal(dicByCode, dicByID);
            Assert.True(ReferenceEquals(dicByID, dicByCode));

            host.Handle(new UpdateDicCommand(new DicUpdateInput
            {
                Id = dicID,
                Name = "test2",
                Code = "dic2"
            }));
            Assert.Equal(1, host.DicSet.Count());
            Assert.True(host.DicSet.TryGetDic(dicID, out dicByID));
            Assert.True(host.DicSet.TryGetDic("dic2", out dicByCode));
            Assert.Equal(dicByCode, dicByID);
            Assert.True(ReferenceEquals(dicByID, dicByCode));
            Assert.Equal("test2", dicByID.Name);
            Assert.Equal("dic2", dicByID.Code);

            host.Handle(new RemoveDicCommand(dicID));
            Assert.False(host.DicSet.TryGetDic(dicID, out dicByID));
            Assert.False(host.DicSet.TryGetDic("dic2", out dicByCode));
            Assert.Equal(0, host.DicSet.Count());

            // 开始测试DicItem
            host.Handle(new AddDicCommand(new DicCreateInput
            {
                Id = dicID,
                Code = "dic1",
                Name = "测试1"
            }));
            Assert.Equal(1, host.DicSet.Count());
            Assert.True(host.DicSet.TryGetDic(dicID, out dicByID));
            DicItemState dicItemByID;
            DicItemState dicItemByCode;
            host.Handle(new AddDicItemCommand(new DicItemCreateInput
            {
                Id = dicItemID,
                IsEnabled = 1,
                DicID = dicID,
                SortCode = 0,
                Description = string.Empty,
                Code = "dicItem1",
                Name = "测试1"
            }));
            Assert.Equal(1, host.DicSet.GetDicItems(dicByID).Count());
            Assert.True(host.DicSet.TryGetDicItem(dicItemID, out dicItemByID));
            Assert.True(host.DicSet.TryGetDicItem(dicByID, "dicItem1", out dicItemByCode));
            Assert.Equal(dicItemByCode, dicItemByID);
            Assert.True(ReferenceEquals(dicItemByID, dicItemByCode));

            host.Handle(new UpdateDicItemCommand(new DicItemUpdateInput
            {
                Id = dicItemID,
                Name = "test2",
                Code = "dicItem2"
            }));
            Assert.Equal(1, host.DicSet.GetDicItems(dicByID).Count);
            Assert.True(host.DicSet.TryGetDicItem(dicItemID, out dicItemByID));
            Assert.True(host.DicSet.TryGetDicItem(dicByID, "dicItem2", out dicItemByCode));
            Assert.Equal(dicItemByCode, dicItemByID);
            Assert.True(ReferenceEquals(dicItemByID, dicItemByCode));
            Assert.Equal("test2", dicItemByID.Name);
            Assert.Equal("dicItem2", dicItemByID.Code);

            host.Handle(new RemoveDicItemCommand(dicItemID));
            Assert.False(host.DicSet.TryGetDicItem(dicItemID, out dicItemByID));
            Assert.False(host.DicSet.TryGetDicItem(dicByID, "dicItem2", out dicItemByCode));
            Assert.Equal(0, host.DicSet.GetDicItems(dicByID).Count);
        }
        #endregion

        #region CanNotDeleteDicWhenItHasDicItems
        [Fact]
        public void CanNotDeleteDicWhenItHasDicItems()
        {
            var host = TestHelper.GetAppHost();
            Assert.Equal(0, host.DicSet.Count());

            var dicID = Guid.NewGuid();

            host.Handle(new AddDicCommand(new DicCreateInput
            {
                Id = dicID,
                Code = "dic1",
                Name = "测试1"
            }));
            Assert.Equal(1, host.DicSet.Count());

            host.Handle(new AddDicItemCommand(new DicItemCreateInput
            {
                Id = Guid.NewGuid(),
                DicID = dicID,
                Name = "item1",
                Code = "item1",
                IsEnabled = 1,
                SortCode = 10,
                Description = string.Empty,
            }));

            bool catched = false;
            try
            {
                host.Handle(new RemoveDicCommand(dicID));
            }
            catch (ValidationException)
            {
                catched = true;
            }
            finally
            {
                Assert.True(catched);
                DicState Dic;
                Assert.True(host.DicSet.TryGetDic(dicID, out Dic));
            }
        }
        #endregion

        #region DicSetShouldRollbackedWhenPersistFailed
        [Fact]
        public void DicSetShouldRollbackedWhenPersistFailed()
        {
            var host = TestHelper.GetAppHost();
            Assert.Equal(0, host.DicSet.Count());

            var moDicRepository = host.GetMoqRepository<Dic, IRepository<Dic>>();
            var entityID1 = Guid.NewGuid();
            var entityID2 = Guid.NewGuid();
            var code = "dic1";
            var name = "测试1";
            host.RemoveService(typeof(IRepository<Dic>));
            moDicRepository.Setup(a => a.Add(It.Is<Dic>(b => b.Id == entityID1))).Throws(new DbException(entityID1.ToString()));
            moDicRepository.Setup(a => a.Update(It.Is<Dic>(b => b.Id == entityID2))).Throws(new DbException(entityID2.ToString()));
            moDicRepository.Setup(a => a.Remove(It.Is<Dic>(b => b.Id == entityID2))).Throws(new DbException(entityID2.ToString()));
            moDicRepository.Setup<Dic>(a => a.GetByKey(entityID1)).Returns(new Dic { Id = entityID1, Code = code, Name = name });
            moDicRepository.Setup<Dic>(a => a.GetByKey(entityID2)).Returns(new Dic { Id = entityID2, Code = code, Name = name });
            host.AddService(typeof(IRepository<Dic>), moDicRepository.Object);


            bool catched = false;
            try
            {
                host.Handle(new AddDicCommand(new DicCreateInput
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
                Assert.Equal(0, host.DicSet.Count());
            }

            host.Handle(new AddDicCommand(new DicCreateInput
            {
                Id = entityID2,
                Code = code,
                Name = name
            }));
            Assert.Equal(1, host.DicSet.Count());

            catched = false;
            try
            {
                host.Handle(new UpdateDicCommand(new DicUpdateInput
                {
                    Id = entityID2,
                    Name = "test2",
                    Code = "dic2"
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
                Assert.Equal(1, host.DicSet.Count());
                DicState Dic;
                Assert.True(host.DicSet.TryGetDic(entityID2, out Dic));
                Assert.Equal(code, Dic.Code);
            }

            catched = false;
            try
            {
                host.Handle(new RemoveDicCommand(entityID2));
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
                DicState Dic;
                Assert.True(host.DicSet.TryGetDic(entityID2, out Dic));
                Assert.Equal(1, host.DicSet.Count());
            }
        }
        #endregion
    }
}
