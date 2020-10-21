using System;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.Translate.v2;
using Google.Cloud.Translation.V2;
using TranslatorWebApp.Settings;
using TranslatorWebApp.utilities;

namespace TranslatorWebApp.Clients
{
    public class GoogleLanguageDetectorClient: ILanguageDetectorClient
    {
        private readonly TranslatorsSettings _translatorsSettings;

        public GoogleLanguageDetectorClient(TranslatorsSettings translatorsSettings)
        {
            _translatorsSettings = translatorsSettings ?? throw new ArgumentNullException(nameof(translatorsSettings));
        }

        public async Task<string> GetLanguage(string text)
        {
            var service = new TranslateService(
                new BaseClientService.Initializer
                {
                    ApiKey = _translatorsSettings.Items.Find(i => i.Vendor.ToLower() == Vendors.Google)?.SubscriptionKey
                });

            var client = new TranslationClientImpl(service);
            var detection = await client.DetectLanguageAsync(text);

            return detection.Language;
        }
    }
}
