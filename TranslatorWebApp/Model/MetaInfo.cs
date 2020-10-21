using System.ComponentModel.DataAnnotations;

namespace TranslatorWebApp.Model
{
    public class MetaInfo
    {
        public string Vendor { get; set; }
        public string From { get; set; }
        [Required]
        public string To { get; set; }
    }
}
