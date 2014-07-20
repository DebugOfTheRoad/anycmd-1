
namespace Anycmd.Tests
{
    using Host.AC.Infra;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class TempTest
    {
        [Fact]
        public void ConfigurationFileTest()
        {
            string fileName = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            Assert.True(fileName.EndsWith("dll.config", StringComparison.OrdinalIgnoreCase));
            Assert.True(null + string.Empty + " " == " ");
            HashSet<EntityTypeMap> _entityTypeMaps = new HashSet<EntityTypeMap>();
            var item = EntityTypeMap.Create(this.GetType(), "test", "test");
            _entityTypeMaps.Add(item);
            _entityTypeMaps.Add(item);
            Assert.Equal(1, _entityTypeMaps.Count);
        }
    }
}
