using System.Collections.Generic;

namespace TranslatorWebApp.Model
{
    public class DetectedItem
    {
        public string Language { get; set; }
        public double Score { get; set; }
        public bool IsTranslationSupported { get; set; }
        public bool IsTransliterationSupported { get; set; }
        public List<Alternative> Alternatives { get; set; }
    }
}
