using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TranslatorWebApp.Model.Request;
using TranslatorWebApp.Model.Response;
using TranslatorWebApp.Clients;
using TranslatorWebApp.Model;

namespace TranslatorWebApp.Services
{
    public class TranslatorService : ITranslatorService
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly ILogger<TranslatorService> _logger;
        private readonly Func<TranslatorVendorEnum, ILanguageDetectorClient> _languageDetectorClientFactory;
        private readonly Func<TranslatorVendorEnum, ITranslatorClient> _translatorClientFactory;

        public TranslatorService(Func<TranslatorVendorEnum, ILanguageDetectorClient> languageDetectorClientFactory,
                                 Func<TranslatorVendorEnum, ITranslatorClient> translatorClientFactory, ILogger<TranslatorService> logger)
        {
            _languageDetectorClientFactory = languageDetectorClientFactory ?? throw new ArgumentNullException(nameof(languageDetectorClientFactory));
            _translatorClientFactory = translatorClientFactory ?? throw new ArgumentNullException(nameof(translatorClientFactory)); _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        }

        public async Task<TranslateResponse> Translate(TranslateRequest translateRequest)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(translateRequest.Meta.From))
                {
                    var language = await GetLanguageDetectorClient(translateRequest.Meta.Vendor).GetLanguage(translateRequest.Items.First().Value);
                    translateRequest.Meta.From = language;
                }

                return await GetResponse(translateRequest);

            }
            catch (Exception e)
            {
                return new TranslateResponse() { Errors = new List<Error>() { new Error() { Message = e.Message } } };
            }

        }

        private async Task<TranslateResponse> GetResponse(TranslateRequest translateRequest)
        {
            var results = new Dictionary<string, string>();

            foreach (var item in translateRequest.Items)
            {
                results.Add(item.Id, await GetTranslatorClient(translateRequest.Meta.Vendor).GetTranslatedText(translateRequest.Items.First().Value,
                                                                                translateRequest.Meta.From, translateRequest.Meta.To));
            }

            return GetTranslateResponse(translateRequest, results);

        }

        private static TranslateResponse GetTranslateResponse(TranslateRequest translateRequest, Dictionary<string, string> results)
        {
            var response = new TranslateResponse
            {
                Meta = translateRequest.Meta,
                RequestedItems = translateRequest.Items,
                TranslatedItems = new List<Item>()
            };
            foreach (var (key, value) in results)
            {
                response.TranslatedItems.Add(new Item() { Id = key, Value = value });
            }

            return response;
        }

        private ILanguageDetectorClient GetLanguageDetectorClient(string vendor)
        {
            return vendor.ToLower() switch
            {
                "microsoft" => _languageDetectorClientFactory(TranslatorVendorEnum.Microsoft),
                "google" => _languageDetectorClientFactory(TranslatorVendorEnum.Google),
                _ => throw new Exception($"Unsupported vendor ({vendor})")
            };
        }

        private ITranslatorClient GetTranslatorClient(string vendor)
        {
            return vendor.ToLower() switch
            {
                "microsoft" => _translatorClientFactory(TranslatorVendorEnum.Microsoft),
                "google" => _translatorClientFactory(TranslatorVendorEnum.Google),
                _ => throw new Exception($"Unsupported vendor ({vendor})")
            };
        }

    }
}
