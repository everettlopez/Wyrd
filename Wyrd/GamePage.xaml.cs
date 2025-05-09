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
        // Define columns
        for (int col = 0; col < Columns; col++)
        {
            WordGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });
        }

        // Define rows
        for (int row = 0; row < Rows; row++)
        {
            WordGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40) });
        }

        // Populate the grid with random letters
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                char randomLetter = (char)('A' + random.Next(0, 26));

                Label cell = new Label
                {
                    Text = randomLetter.ToString(),
                    FontSize = 20,
                    FontAttributes = FontAttributes.Bold,
                    BackgroundColor = Color.FromArgb("#f0f0f0"),
                    TextColor = Colors.Black,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    WidthRequest = 40,
                    HeightRequest = 40,
                    Margin = new Thickness(0)
                };

                // Position in grid
                WordGrid.Children.Add(cell);
                Grid.SetRow(cell, row);
                Grid.SetColumn(cell, col);
            }
        }
    }
}