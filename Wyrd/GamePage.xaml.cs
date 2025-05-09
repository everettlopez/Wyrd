namespace Wyrd;

public partial class GamePage : ContentPage
{

    private const int Columns = 6;          //Initialize the number of columns in the word grid
    private const int Rows = 8;             //Initialize the number of rows in the word grid
    private readonly Random random = new Random();

    public GamePage()
	{
		InitializeComponent();
        SetupGrid();
    }

    private void SetupGrid()
    {
        WordGrid.ColumnDefinitions.Clear();
        WordGrid.RowDefinitions.Clear();
        WordGrid.Children.Clear();

        for (int col = 0; col < Columns; col++)
        {
            WordGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });
        }

        for (int row = 0; row < Rows; row++)
        {
            WordGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40) });
        }

        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                char randomLetter = (char)('A' + random.Next(0, 26));

                var button = new Button
                {
                    Text = randomLetter.ToString(),
                    FontSize = 20,
                    FontAttributes = FontAttributes.Bold,
                    BackgroundColor = Color.FromArgb("#f0f0f0"),
                    TextColor = Colors.Black,
                    WidthRequest = 48,
                    HeightRequest = 48,
                    Padding = 0,
                    Margin = new Thickness(4),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    CommandParameter = (row, col, randomLetter)
                };


                // Attach the click event
                button.Clicked += OnGridButtonClicked;

                // Add button to grid
                WordGrid.Children.Add(button);
                Grid.SetRow(button, row);
                Grid.SetColumn(button, col);
            }
        }
    }

    private void OnGridButtonClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is ValueTuple<int, int, char> info)
        {
            var (row, col, letter) = info;
            System.Diagnostics.Debug.WriteLine($"Clicked cell [{row}, {col}] with letter '{letter}'");
        }
    }


}