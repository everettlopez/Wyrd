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
    }
}
