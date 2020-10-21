using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TranslatorWebApp.Clients
{
    public class MicrosoftTranslatorClient : ITranslatorClient
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly ILogger<MicrosoftLanguageDetectorClient> _logger;
        private readonly HttpClient _httpClient;

        public MicrosoftTranslatorClient(IHttpClientFactory httpClientFactory, ILogger<MicrosoftLanguageDetectorClient> logger)
        {
            _httpClient = httpClientFactory?.CreateClient("MicrosoftTranslatorClient")
                          ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> GetTranslatedText(string text, string from, string to)
        {
            object[] body = { new { Text = text } };
            var requestBody = JsonConvert.SerializeObject(body);
            using var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_httpClient.BaseAddress}&from={from}&to={to}"),
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(responseBody);
            }


            var result =
                JsonConvert.DeserializeObject<List<Dictionary<string, List<Dictionary<string, string>>>>>(
                    responseBody);
            return result[0]["translations"][0]["text"];
        }
    }
}
