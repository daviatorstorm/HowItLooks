using HowItLooks.Services;

namespace HowItLooks
{
    public partial class App : Application
    {
        public App(StartupService startupService)
        {
            startupService.Run();
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}