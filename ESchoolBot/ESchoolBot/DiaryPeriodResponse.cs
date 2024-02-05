using System.Text.Json.Serialization;

namespace ESchoolBot
{
    public class DiaryPeriodResponse
    {
        public required DiaryPeriod[] Result { get; set; }

        public class DiaryPeriod
        {
            public required int LessonId { get; set; }
            public string? Subject { get; set; }
            public string? SugTotalMark { get; set; }
            public string? TeachFio { get; set; }

            [JsonPropertyName("startDt")]
            public DateTime StartDate {  get; set; }

            [JsonPropertyName("mktWt")]
            public float MarkWeight { get; set; }

            [JsonPropertyName("markValId")]
            public int MarkValue { get; set; }
        }
    }
}