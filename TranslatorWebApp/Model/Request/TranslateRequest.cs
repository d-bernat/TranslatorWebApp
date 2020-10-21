using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TranslatorWebApp.Model.Request
{
    public class TranslateRequest
    {
        [Required]
        public MetaInfo Meta { get; set; }
        [Required]
        public List<Item> Items { get; set; }
    }
}
