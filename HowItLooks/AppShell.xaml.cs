namespace HowItLooks
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            var savedTheme = Preferences.Get("AppTheme", "Light");
            App.Current.UserAppTheme = savedTheme == "Dark" ? AppTheme.Dark : AppTheme.Light;

            this.Loaded += (_, __) =>
            {
                SafeUpdateThemeIcon();
                SafeUpdateFlyoutIcons();
            };
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

            if (GlobalsIcon != null)
                GlobalsIcon.Source = isDark ? "earth_light.png" : "earth.png";
        }

        private async void OnMainTapped(object sender, EventArgs e)
        {
            await GoToAsync("//MainPage");
            Shell.Current.FlyoutIsPresented = false;
        }

        private async void OnGlobalsTapped(object sender, EventArgs e)
        {
            await GoToAsync("//Globals");
            Shell.Current.FlyoutIsPresented = false;
        }
    }
}
