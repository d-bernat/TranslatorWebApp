using System;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.Translate.v2;
using Google.Cloud.Translation.V2;
using TranslatorWebApp.Settings;
using TranslatorWebApp.utilities;

namespace TranslatorWebApp.Clients
{
    public class GoogleTranslatorClient: ITranslatorClient
    {
        private readonly TranslatorsSettings _translatorsSettings;

        public GoogleTranslatorClient(TranslatorsSettings translatorsSettings)
        {
            _translatorsSettings = translatorsSettings ?? throw new ArgumentNullException(nameof(translatorsSettings));
        }

        public async Task<string> GetTranslatedText(string text, string from, string to)
        {
            var service = new TranslateService(
                                new BaseClientService.Initializer
                                {
                                    ApiKey = _translatorsSettings.Items.Find( i => i.Vendor.ToLower() == Vendors.Google)?.SubscriptionKey
                                });
            var client = new TranslationClientImpl(service);
            var response = await client.TranslateTextAsync(text, to, from);
            return response.TranslatedText;
        }
    }
}
