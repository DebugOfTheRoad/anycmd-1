
namespace Anycmd.Tests
{
    using DataContracts;
    using Xunit;
    using Util;

    public class TokenTest
    {
        [Fact]
        public void TokenIsValid()
        {
            var ticks = SystemTime.UtcNow().Ticks;
            var secKey = "DF25BCB5-35E3-41E4-980F-64D916D806FF";
            var appID = "87E9DAAB-2EA4-4A99-92BA-6C9DDB0F868C";
            TokenObject token = TokenObject.Create(
                TokenObject.Token(appID, ticks, secKey),
                appID,
                ticks);

            Assert.True(token.IsValid(secKey));
        }

        [Fact]
        public void SignatureIsValid()
        {
            var ticks = SystemTime.UtcNow().Ticks;
            var secKey = "123456";
            var orignalString = "appID=100&random=778899";
            Signature signature = Signature.Create(orignalString, Signature.Sign(orignalString, secKey));
            Assert.True(signature.IsValid(secKey));
        }
    }
}
