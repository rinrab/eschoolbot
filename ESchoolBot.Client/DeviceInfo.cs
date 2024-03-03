using System.Text.Json.Serialization;

namespace ESchoolBot.Client
{
    public class DeviceInfo
    {
        [JsonPropertyName("cliType")]
        public required string CliType { get; set; }

        [JsonPropertyName("cliVer")]
        public required string CliVersion { get; set; }

        [JsonPropertyName("pushToken")]
        public required string? PushToken { get; set; }

        [JsonPropertyName("deviceName")]
        public required string DeviceName { get; set; }

        [JsonPropertyName("deviceModel")]
        public required int DeviceModel { get; set; }

        [JsonPropertyName("cliOs")]
        public required string CliOperatingSystem { get; set; }

        [JsonPropertyName("cliOsVe")]
        public required string? CliOperatingSystemVersion { get; set; }
    };
}
