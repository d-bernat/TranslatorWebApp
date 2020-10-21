using System.Threading.Tasks;

namespace TranslatorWebApp.Clients
{
    public interface ILanguageDetectorClient
    {
        Task<string> GetLanguage(string text);
    }
}
