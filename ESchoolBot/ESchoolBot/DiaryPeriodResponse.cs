namespace ESchoolBot
{
    public class DiaryPeriodResponse
    {
        public required DiaryPeriod[] Result { get; set; }

        public class DiaryPeriod
        {
            public required int LessonId { get; set; }
            public string? Subject { get; set; }
        }
    }
}