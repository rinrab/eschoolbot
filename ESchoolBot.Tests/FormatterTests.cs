using NUnit.Framework;

namespace ESchoolBot.Tests
{
    public class FormatterTests
    {
        [Test]
        public void FormatShortDateTest()
        {
            Assert.AreEqual(
                "апр. 23",
                Formatter.FormatShortDate(new DateTime(2032, 4, 23)));
        }

        [Test]
        public void FormatFullDateTest()
        {
            Assert.AreEqual(
                "2032-04-23T06:32:43",
                Formatter.FormatFullDate(new DateTime(2032, 4, 23, 6, 32, 43)));
        }
    }
}
