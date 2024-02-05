using System.Text.Json.Serialization;

namespace ESchoolBot
{
    public class DiaryPeriodResponse
    {
        public required DiaryPeriod[] Result { get; set; }

        public class DiaryPeriod
        {
            public string? Subject { get; set; }

            [JsonPropertyName("mktWt")]
            public float MarkWeight { get; set; }

            [JsonPropertyName("markVal")]
            public string? MarkValue { get; set; }

            public DateTime? MarkDate { get; set; }
        }
    }
}