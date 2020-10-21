using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TranslatorWebApp.Model;

namespace TranslatorWebApp.Clients
{
    public class MicrosoftLanguageDetectorClient: ILanguageDetectorClient
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly ILogger<MicrosoftLanguageDetectorClient> _logger;
        private readonly HttpClient _httpClient;

        public MicrosoftLanguageDetectorClient(IHttpClientFactory httpClientFactory, ILogger<MicrosoftLanguageDetectorClient> logger)
        {
            _httpClient = httpClientFactory?.CreateClient("MicrosoftLanguageDetectorClient")
                          ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> GetLanguage(string text)
        {
            object[] body = { new { Text = "[{ \"Text\": \"" + text + "\" }]" } };
            var requestBody = JsonConvert.SerializeObject(body);
            using var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };
            var detectResponse = JsonConvert.DeserializeObject<List<DetectedItem>>(responseBody, jsonSerializerSettings);
            var score = detectResponse?.Max(i => i.Score) ?? throw new Exception("Unable to confidently detect input language");
            if (score < 0.5)
            {
                throw new Exception("Unable to confidently detect input language");
            }

            return detectResponse.First(i => Math.Abs(i.Score - score) < 0.01)?.Language;

        }
    }
}
