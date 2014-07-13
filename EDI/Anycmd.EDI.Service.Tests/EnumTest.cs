
namespace Anycmd.EDI.Service.Tests
{
    using Anycmd.Host.EDI;
    using Xunit;

    public class EnumTest
    {
        [Fact]
        public void EnumToStringTest()
        {
            Assert.Equal(Status.InvalidClientID.ToString(), "InvalidClientID");
            Assert.Equal(Status.InvalidClientID.ToName(), "InvalidClientID");
            Status stateCode;
            Assert.False(0.TryParse(out stateCode));
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
        }
    }
}
