using System.Text;

namespace Wyrd;

public partial class GamePage : ContentPage
{

    private const int Columns = 6;          //Initialize the number of columns in the word grid
    private const int Rows = 8;             //Initialize the number of rows in the word grid

    private readonly Random random = new Random();
    private StringBuilder selectedLetters = new StringBuilder();


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

        // Define each column for the grid
        for (int col = 0; col < Columns; col++)
        {
            WordGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });
        }

        // Define each row for the grid
        for (int row = 0; row < Rows; row++)
        {
            WordGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40) });
        }

        // Iterate through each cell in the grid
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                // Generate a random letter for eahc cell
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

            selectedLetters.Append(letter);
            LetterBox.Text = selectedLetters.ToString();
        }
    }

    private void ConfirmButtonClicked(object sender, EventArgs e)
    {
        // TODO: Must be implemented when words are populated into the grid.
        // Start a new branch in Git.
        System.Diagnostics.Debug.WriteLine("Confirm button has been clicked");
    }

    private void ClearButtonClicked(object sender, EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("Clear button has been clicked");

        selectedLetters.Clear();
        LetterBox.Text = "";
    }



}