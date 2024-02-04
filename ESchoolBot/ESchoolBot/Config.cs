namespace ESchoolBot
{
    public class Config
    {
        public required string BotToken { get; set; }
        public required string ConnectionString { get; set; }

        public static readonly string SectionName = "Config";
    }
}
