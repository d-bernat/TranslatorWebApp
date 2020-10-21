namespace TranslatorWebApp.Model
{
    public class Alternative
    {
        public string Language { get; set; }
        public double Score { get; set; }
        public bool IsTranslationSupported { get; set; }
        public bool IsTransliterationSupported { get; set; }
    }
}
