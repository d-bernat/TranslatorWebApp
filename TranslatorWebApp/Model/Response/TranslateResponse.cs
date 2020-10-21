using System.Collections.Generic;
using TranslatorWebApp.Model.Request;

namespace TranslatorWebApp.Model.Response
{
    public class TranslateResponse
    {
        public MetaInfo Meta { get; set; }
        public List<Item> RequestedItems { get; set; }
        public List<Item> TranslatedItems { get; set; }
        public List<Error> Errors { get; set; }
    }
}
