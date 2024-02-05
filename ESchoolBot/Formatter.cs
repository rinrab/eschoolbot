using System.Globalization;
using System.Text.RegularExpressions;

namespace ESchoolBot
{
    internal class Formatter
    {
        public const string StartMessage =
            """
            Пожалуйста нажмите на кнопку ниже чтобы залогиниться:
            """;

        public const string LoginButtonText = "🔑 Войти";
        public const string LoginPlaceholder = "Нажмите на кнопку чтобы войти";

        public const string HelpMessage =
            """
            Я буду присылать вам новые оценки, с сайта eschool.center
            
            /start - старт/логин
            
            Автор: Timofei Zhakov, Идея: Лев Волков
            """;

        public const string LoginRequired = 
            """
            Не удалось войти в аккаунт.
            Войдите пожалуйста еще раз:
            """;

        public const string IncorrectLoginOrPassword = 
            """
            Неправильный логин или пароль. Попробуйте еще раз:
            """;

        public const string PostLogin =
            """
            Вход завершен! Теперь вам будут приходить уведомления у новых оценках.
            """;

        public const string NotFound = "Не найдено";

        private static readonly CultureInfo culture = new CultureInfo("en-GB");

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
                Новая оценка по <b>{diary.Subject}</b>
                Оценка: {diary.MarkValue}
                Коэффецент: {diary.MarkWeight}
                """;
        }

        public static DateTime GetDate()
        {
            return DateTime.UtcNow.AddHours(3).Date;
        }
    }
}