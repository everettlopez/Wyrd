namespace Wyrd
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        // This event triggers when the Start Game button is clicked
        private async void OnStartGameClicked(object sender, EventArgs e)
        {
            // Navigate to GamePage
            await Navigation.PushAsync(new GamePage());
        }

    }

}
