using Microsoft.AspNetCore.Mvc;

namespace TranslatorWebApp.Controllers
{
    [Route("monitoring/[controller]")]
    [ApiController]
    public class AliveController : ControllerBase
    {
        [HttpGet]
        public string Alive()
        {
            return ":-)";
        }
    }
}
