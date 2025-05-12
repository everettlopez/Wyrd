namespace Wyrd
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            
            // Wrap the AppShell in a NavigationPage to support navigation features
            MainPage = new NavigationPage(new AppShell());
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);

            const int newWidth = 450;
            const int newHeight = 650;

            window.Width = newWidth;
            window.Height = newHeight;

            return window;
        }
    }
}
