namespace Wyrd
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        // This event triggers when the Start Game button is clicked
        private async void OnStartGameClicked(object sender, EventArgs e)
        {
        
            System.Diagnostics.Debug.WriteLine("Start game button has been clicked");

            // Navigate to GamePage
            await Navigation.PushAsync(new GamePage());
        }

    }

}
