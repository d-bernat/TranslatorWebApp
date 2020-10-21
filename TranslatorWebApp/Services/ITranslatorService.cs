using System.Threading.Tasks;
using TranslatorWebApp.Model.Request;
using TranslatorWebApp.Model.Response;

namespace TranslatorWebApp.Services
{
    public interface ITranslatorService
    {
        Task<TranslateResponse> Translate(TranslateRequest translateRequest);
    }
}
