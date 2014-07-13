
namespace Anycmd.Tests
{
    using System;
    using Xunit;
    using Model;
    using Exceptions;

    /// <summary>
    /// 实体标识采用“及早生成策略”，实体标识生成后是不能更改的。
    /// </summary>
    public class EntityIDTest
    {
        [Fact]
        public void EntityID_Can_Not_Change()
        {
            Assert.Throws<CoreException>(() =>
            {
                TestEntity entity = new TestEntity();
                entity.Id = Guid.NewGuid();
                entity.Id = Guid.NewGuid();
            });
        }

        [Fact]
        public void EntityID_Can_Init()
        {
            TestEntity entity = new TestEntity();
            Assert.True(entity.Id == Guid.Empty);
            var id = Guid.NewGuid();
            entity.Id = id;
            Assert.True(entity.Id == id);
        }
    }

    public class TestEntity : EntityObject
    {

    }
}
