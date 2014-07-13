
namespace Anycmd.Tests
{
    using System;
    using Xunit;

    public class TempTest
    {
        [Fact]
        public void ConfigurationFileTest()
        {
            string fileName = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            Assert.True(fileName.EndsWith("dll.config", StringComparison.OrdinalIgnoreCase));
            Assert.True(null + string.Empty + " " == " ");
        }
    }
}
