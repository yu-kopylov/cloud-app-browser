using NUnit.Framework;

namespace CloudAppBrowser.Core.Tests
{
    [TestFixture]
    public class StringUtilsTests
    {
        [Test]
        public void TestAbbreviate()
        {
            Assert.That(StringUtils.Abbreviate(null, 5), Is.EqualTo(null));
            Assert.That(StringUtils.Abbreviate("", 5), Is.EqualTo(""));
            Assert.That(StringUtils.Abbreviate("1", 5), Is.EqualTo("1"));
            Assert.That(StringUtils.Abbreviate("123", 5), Is.EqualTo("123"));
            Assert.That(StringUtils.Abbreviate("12345", 5), Is.EqualTo("12345"));
            Assert.That(StringUtils.Abbreviate("123456", 5), Is.EqualTo("12..."));
        }
    }
}