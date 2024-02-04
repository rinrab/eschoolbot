﻿using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ESchoolBot
{
    public class Client : IClient
    {
        private readonly HttpClient httpClient;

        public Client(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Add("Cookie", "site_ver=app");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:122.0) Gecko/20100101 Firefox/122.0");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
            httpClient.BaseAddress = new Uri("https://app.eschool.center");
            this.httpClient = httpClient;
        }

        public async Task<string> LoginAsync(string email, string password, CancellationToken cancellationToken)
        {
            StringBuilder bodyBuilder = new StringBuilder();
            bodyBuilder.AppendUrlEncoded("username", email);
            var passwordHash = ComputeHash(password);
            bodyBuilder.AppendUrlEncoded("password", passwordHash);
            var device = new Device("web", "v.413", "eaBw24ID4Pz8UwK8nxfkwiW0aFnE9U56XJiA4GF1KtCXH6mKGzcVLDh08c1O2VjC", "Mozilla", 122, "Windows N", null);
            bodyBuilder.AppendUrlEncoded("device", JsonSerializer.Serialize(device));

            var response = await httpClient.PostAsync("/ec-server/login", new StringContent(bodyBuilder.ToString(), null, "application/x-www-form-urlencoded"), cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return response.Headers.GetValues("Set-Cookie").First();

            }
            else
            {
                throw new LoginException();
            }
        }

        public async Task GetDiaryPeriodAsync()
        {
            // https://app.eschool.center/ec-server/student/getDiaryPeriod/?userId=108231&eiId=406054
        }

        private string ComputeHash(string vpnProfileXml)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(vpnProfileXml));

                StringBuilder rv = new StringBuilder(hash.Length * 2);
                string symbols = "0123456789abcdef";
                foreach (byte b in hash)
                {
                    rv.Append(symbols[b / 16]);
                    rv.Append(symbols[b % 16]);
                }
                return rv.ToString();
            }
        }

        public record Device(string cliType,
                             string cliVer,
                             string? pushToken,
                             string deviceName,
                             int deviceModel,
                             string cliOs,
                             string? cliOsVer);
    }
}
