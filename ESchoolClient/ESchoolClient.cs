using ESchoolClient;
using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace ESchoolBot
{
    public class ESchoolClient : IESchoolClient
    {
        private readonly HttpClient httpClient;

        public ESchoolClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<string> LoginAsync(string email, string passwordHash, CancellationToken cancellationToken)
        {
            Device device = new Device
            {
                CliType = "web",
                CliVersion = "v.413",
                PushToken = "eaBw24ID4Pz8UwK8nxfkwiW0aFnE9U56XJiA4GF1KtCXH6mKGzcVLDh08c1O2VjC",
                DeviceName = "Mozilla",
                DeviceModel = 122,
                CliOperatingSystem = "Windows N",
                CliOperatingSystemVersion = null
            };

            string body = string.Format("username={0}&password={1}&device={2}",
                                        Uri.EscapeDataString(email),
                                        Uri.EscapeDataString(passwordHash),
                                        Uri.EscapeDataString(JsonSerializer.Serialize(device)));

            HttpContent content = new StringContent(body, null, "application/x-www-form-urlencoded");
            HttpResponseMessage response = await httpClient.PostAsync("/ec-server/login", content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                string cookieHeader = response.Headers.GetValues("Set-Cookie").First();
                return new Regex("^JSESSIONID=([a-zA-Z0-9]*)").Match(cookieHeader).Groups[1].Value;
            }
            else
            {
                throw new LoginException();
            }
        }

        public static string ComputeHash(string str)
        {
            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(str));

            StringBuilder rv = new StringBuilder(hash.Length * 2);
            string symbols = "0123456789abcdef";
            foreach (byte b in hash)
            {
                rv.Append(symbols[b / 16]);
                rv.Append(symbols[b % 16]);
            }

            return rv.ToString();
        }

        public static void ConfigureDefaultHttpClient(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Add("Cookie", "site_ver=app");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:122.0) Gecko/20100101 Firefox/122.0");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
            httpClient.BaseAddress = new Uri("https://app.eschool.center");
        }

        public async Task<StateResponse> GetStateAsync(string sessionId)
        {
            return await GetRequestHelper<StateResponse>(sessionId, "/ec-server/state");
        }

        private async Task<T> GetRequestHelper<T>(string sessionId, string url)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                request.Headers.Add("Cookie", "JSESSIONID=" + sessionId);

                HttpResponseMessage response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    T? parsedResponse = await response.Content.ReadFromJsonAsync<T>();

                    if (parsedResponse == null)
                    {
                        throw new Exception("Invalid json response");
                    }
                    else
                    {
                        return parsedResponse;
                    }
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new LoginException();
                }
                else
                {
                    throw new Exception(string.Format("HTTP request failed: {0}", response.StatusCode));
                }
            }
        }

        public async Task<DiaryPeriodResponse> GetDiaryPeriodAsync(string sessionId, int userId, int periodId)
        {
            return await GetRequestHelper<DiaryPeriodResponse>(sessionId, $"/ec-server/student/getDiaryPeriod/?userId={userId}&eiId={periodId}");
        }

        public async Task<DiaryUnitsResponse> GetDiaryUnitsAsync(string sessionId, int userId, int periodId)
        {
            return await GetRequestHelper<DiaryUnitsResponse>(sessionId,
                $"/ec-server/student/getDiaryUnits/?userId={userId}&eiId={periodId}");
        }

        public async Task<GroupsResponse> GetGroupsAsync(string sessionId, int userId)
        {
            return await GetRequestHelper<GroupsResponse>(sessionId, $"/ec-server/usr/getClassByUser?userId={userId}");
        }

        public async Task<PeriodsResponse> GetPeriodsAsync(string sessionId, int groupId)
        {
            return await GetRequestHelper<PeriodsResponse>(sessionId, $"/ec-server/dict/periods/0?groupId={groupId}");
        }

        public class Device
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
}
