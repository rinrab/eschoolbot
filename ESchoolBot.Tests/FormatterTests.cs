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
        public void FormatLongDate()
        {
            Assert.AreEqual(
                "23 апреля 2032",
                Formatter.FormatLongDate(new DateTime(2032, 4, 23)));
        }

        [Test]
        public void FormatFullDateTest()
        {
            Assert.AreEqual(
                "2032-04-23T06:32:43",
                Formatter.FormatFullDate(new DateTime(2032, 4, 23, 6, 32, 43)));
        }

        [Test]
        public void FormatNewDiaryMessageTest()
        {
            var actual = Formatter.FormatNewDiaryMessage(new DiaryPeriod
            {
                Subject = "Алгебра и начало анализа",
                MarkValue = "5",
                MarkWeight = 1.0f,
                MarkDate = new DateTime(2023, 09, 02),
            });

            Assert.AreEqual(
                """
                Новая оценка по <b>Алгебра и начало анализа</b>
                Оценка: 5
                Коэффецент: 1.0

                <i>02 сентября 2023</i>
                """,
                actual);
        }
    }
}
