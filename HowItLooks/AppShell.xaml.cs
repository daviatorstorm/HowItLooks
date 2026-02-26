using HowItLooks.Extension;
using System.Globalization;

namespace HowItLooks
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            var savedTheme = Preferences.Get("AppTheme", "Light");
            var lang = Preferences.Get("Lang", "English");
            LanguagePicker.SelectedItem = lang;
            App.Current.UserAppTheme = savedTheme == "Dark" ? AppTheme.Dark : AppTheme.Light;

            this.Loaded += (_, __) =>
            {
                SafeUpdateThemeIcon();
                SafeUpdateFlyoutIcons();
            };

            Routing.RegisterRoute("Groups/details", typeof(GroupDetailsPage));
            Routing.RegisterRoute("Campaigns/main", typeof(MainPage));
        }

        private void OnThemeIconClicked(object sender, EventArgs e)
        {
            bool isDark = App.Current.UserAppTheme == AppTheme.Dark;

            App.Current.UserAppTheme = isDark ? AppTheme.Light : AppTheme.Dark;
            Preferences.Set("AppTheme", isDark ? "Light" : "Dark");

            SafeUpdateThemeIcon();
            SafeUpdateFlyoutIcons();
        }

        private void SafeUpdateThemeIcon()
        {
            ThemeToggleButton.Source = App.Current.UserAppTheme == AppTheme.Dark
                ? "light_mode.png"
                : "dark_mode.png";
        }

        private void SafeUpdateFlyoutIcons()
        {
            bool isDark = App.Current.UserAppTheme == AppTheme.Dark;

            if (MainIcon != null)
                MainIcon.Source = isDark ? "campaign_light.png" : "campaign.png";
        }

        private async void OnMainTapped(object sender, EventArgs e)
        {
            await GoToAsync("//MainPage");
            Shell.Current.FlyoutIsPresented = false;
        }

        private async void OnGroupsTapped(object sender, EventArgs e)
        {
            await GoToAsync("//Groups");
            Shell.Current.FlyoutIsPresented = false;
        }
        private async void OnCampaignsTapped(object sender, EventArgs e)
        {
            await GoToAsync("//Campaigns");
            Shell.Current.FlyoutIsPresented = false;
        }
        private void OnLanguagePickerChanged(object sender, EventArgs e)
        {
            var selectedLanguage = LanguagePicker.SelectedItem as string;

            if (selectedLanguage == "Українська")
            {
                Translator.Instance.SetCulture(new CultureInfo(""));
            }
            else if (selectedLanguage == "English")
            {
                Translator.Instance.SetCulture(new CultureInfo("en"));
            }

            Preferences.Set("Lang", selectedLanguage);
        }
    }
}
