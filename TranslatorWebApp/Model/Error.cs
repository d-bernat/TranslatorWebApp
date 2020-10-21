using System.ComponentModel.DataAnnotations;

namespace TranslatorWebApp.Model
{
    public class Error
    {
        [Required]
        public string Message { get; set; }
    }
}
