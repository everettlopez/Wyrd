namespace Wyrd
{
    public partial class AppShell : Shell
    {

        public AppShell()
        {
            InitializeComponent();

            // Register the GamePage with a route so Shell can navigate to it
            // nameof(GamePage) makes your code safer by avoiding hard-coded route strings like "GamePage".
            Routing.RegisterRoute(nameof(GamePage), typeof(GamePage));
        }
    }
}
