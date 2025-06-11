using System.ComponentModel;
using System.Globalization;

namespace HowItLooks.Extension
{
    public class Translator : INotifyPropertyChanged
    {
        private const string LanguagePreferenceKey = "AppLanguage";
        private CultureInfo _cultureInfo;

        public static Translator Instance { get; } = new Translator();

        public CultureInfo CultureInfo
        {
            get => _cultureInfo;
            set
            {
                if (_cultureInfo != value)
                {
                    _cultureInfo = value;
                    SaveLanguage(value);
                    UpdateCulture(value);
                }
            }
        }

        private Translator()
        {
            _cultureInfo = LoadLanguage() ?? Thread.CurrentThread.CurrentUICulture;
            UpdateCulture(_cultureInfo);
        }

        private void SaveLanguage(CultureInfo culture)
        {
            Preferences.Set(LanguagePreferenceKey, culture.TwoLetterISOLanguageName);
        }

        private CultureInfo LoadLanguage()
        {
            var langCode = Preferences.Get(LanguagePreferenceKey, null);
            return langCode != null ? new CultureInfo(langCode) : null;
        }

        private void UpdateCulture(CultureInfo culture)
        {
            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
            OnPropertyChanged();
        }

        public string this[string text] =>
            Resources.Languages.Resource.ResourceManager.GetString(text, CultureInfo) ?? text;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        public void SetCulture(CultureInfo culture)
        {
            CultureInfo = culture;
        }
    }
}