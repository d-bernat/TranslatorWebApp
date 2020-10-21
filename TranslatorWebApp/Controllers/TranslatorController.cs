using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TranslatorWebApp.Model.Request;
using TranslatorWebApp.Model.Response;
using TranslatorWebApp.Services;

namespace TranslatorWebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TranslatorController : ControllerBase
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly ILogger<TranslatorController> _logger;
        private readonly ITranslatorService _translatorService;

        public TranslatorController(ITranslatorService translatorService, ILogger<TranslatorController> logger)
        {
            _translatorService = translatorService ?? throw new ArgumentNullException(nameof(translatorService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        public async Task<TranslateResponse> Get([FromBody] TranslateRequest translateRequest)
        {
            return await _translatorService.Translate(translateRequest);
        }
    }
}
