using System.Globalization;

namespace ESchoolBot
{
    public class Formatter
    {
        public const string StartMessage =
            """
            Нажмите на кнопку ниже чтобы залогиниться:
            """;

        public const string LoginButtonText = "🔑 Войти";
        public const string LoginPlaceholder = "Нажмите на кнопку чтобы войти";

        public const string PostLogin =
            """
            Вход завершен! Теперь вам будут приходить уведомления у новых оценках.
            """;

        public const string HelpMessage =
            """
            Я буду присылать вам новые оценки с сайта eschool.center

            /start - старт/логин
            /off - выключить бота
            """;

        // Errors
        public const string LoginRequired =
            """
            Не удалось войти в аккаунт.
            Войдите, пожалуйста, еще раз:
            """;

        public const string IncorrectLoginOrPassword =
            """
            Неправильный логин или пароль. Попробуйте еще раз:
            """;

        public const string NotFound = "Не найдено";

        public const string BotDisabled =
            """
            Вы отключили нотификации. Используйте комманду /start, чтобы перезапустить бота.
            """;

        public static string FormatShortDate(DateTime date)
        {
            return date.ToString("MMM dd", culture);
        }

        public static string FormatLongDate(DateTime date)
        {
            return date.ToString("dd MMMM yyyy", culture);
        }

        public static string FormatFullDate(DateTime date)
        {
            return date.ToString("s", culture);
        }

        public static string FormatNewDiaryMessage(DiaryPeriod diary)
        {
            return
                $"""
                Новая оценка по <b>{diary.Subject}</b>
                Оценка: {diary.MarkValue}
                Коэффецент: {diary.MarkWeight:F1}

                <i>{FormatLongDate(diary.MarkDate!.Value)}</i>
                """;
        }

        public static DateTime GetDate()
        {
            return DateTime.UtcNow.AddHours(3);
        }

        private static readonly CultureInfo culture = new CultureInfo("ru-RU");
    }
}