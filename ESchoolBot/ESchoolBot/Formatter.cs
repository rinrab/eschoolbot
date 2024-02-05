using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;

namespace ESchoolBot
{
    internal class Formatter
    {
        public const string StartMessage =
                   "Please log in to subscribe to new resources and enable other bot features.";

        public const string LoginButtonText = "🔑 Log in";
        public const string LoginPlaceholder = "👇 Click log in 👇";

        public const string HelpMessage =
            "I will send you new resources. Additionally, you can:\n" +
            "\n" +
            "/resources - View the latest resources\n" +
            "/homework - Check the homework for the next days\n" +
            "/reports - Access your reports and points\n" +
            "/start - Start or log in\n" +
            "\n" +
            "Bot created by Timofei Zhakov.";

        public const string LoginRequired = "Please log in to use this bot.";

        public const string IncorrectLoginOrPassword = "Login or password is incorrect. Please log in again.";

        public const string FetchLoginError = "Cannot login while fetch. Please login again:";

        public const string PostLogin = "Login completed! You are now subscribed to new resources. For assistance, type /help.";

        public const string NotFound = "Не найдено";

        private static readonly CultureInfo culture = new CultureInfo("en-GB");
        private static readonly Regex htmlTagsRegex = new Regex(@"<.*?>|&nbsp;");

        public static string FormatShortDate(DateTime date)
        {
            return date.ToString("MMM dd", culture);
        }

        public static string FormatFullDate(DateTime date)
        {
            return date.ToString("s", culture);
        }

        public static string FormatNewDiaryMessage(DiaryPeriodResponse.DiaryPeriod diary)
        {
            return
                $"""
                Новая оценка по <b>{diary.Subject ?? NotFound}</b>
                Оценка: {diary.MarkValue}
                Коэффецент: {diary.MarkWeight}
                Учитель: {diary.TeachFio ?? NotFound}
                """;
        }

        public static DateTime GetDate()
        {
            return DateTime.UtcNow.AddHours(3).Date;
        }
    }
}