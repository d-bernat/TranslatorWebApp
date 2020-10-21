using System.Threading.Tasks;

namespace TranslatorWebApp.Clients
{
    public interface ITranslatorClient
    {
        Task<string> GetTranslatedText(string text, string from, string to);
    }
}
