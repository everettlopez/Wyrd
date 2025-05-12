using System.Numerics;
using System.Text;
using System.Xml.Serialization;


namespace Wyrd;

public partial class GamePage : ContentPage
{

    private const int Columns = 6;          //Initialize the number of columns in the word grid
    private const int Rows = 8;             //Initialize the number of rows in the word grid

    private readonly Random random = new Random();
    private StringBuilder selectedLetters = new StringBuilder();
    private List<string> wordList = new List<string>();             // To store the words for the grid
    private List<string> usableWords = new List<string>();             // To store the words for the grid
    private List<string> foundWords = new List<string>();
    private List<string> totalWords = new List<string>();


    public GamePage()
    {
        InitializeComponent();
        _ = InitializeGameAsync(); // Fire-and-forget async call
    }

    private async Task InitializeGameAsync()
    {
        await LoadWordListAsync();

        if (wordList.Count == 0)
        {
            await DisplayAlert("Error", "No words could be loaded. Check your file setup.", "OK");
            return;
        }

        SetupGrid();
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
        System.Diagnostics.Debug.WriteLine("Confirm button has been clicked");

        String result = selectedLetters.ToString().ToUpper();
        
        if(totalWords.Contains(result) && !foundWords.Contains(result))
        {
            System.Diagnostics.Debug.WriteLine("Correct");

            foundWords.Add(result);

            UpdateProgressBar();


        }
        else if(totalWords.Contains(result) && foundWords.Contains(result))
        {
            System.Diagnostics.Debug.WriteLine("Correct, but you've already found this word.");
            DisplayAlert("Word Already Found", "You have already used this word in an attempt. Don'r give up and try again.", "Continue");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("Incorrect");
        }
       
        System.Diagnostics.Debug.WriteLine($"The result is {result}");

    }

    private void ClearButtonClicked(object sender, EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("Clear button has been clicked");

        selectedLetters.Clear();
        LetterBox.Text = "";
    }

    private void UpdateProgressBar()
    {

        double normalizedProgress = (double) foundWords.Count / totalWords.Count;
        ProgressBar.Progress = normalizedProgress;

        LetterBox.Text = "";
        selectedLetters.Clear();
    }

    private async Task LoadWordListAsync()
    {
        try
        {
            // Try to open the wordlist.txt from app package
            using var stream = await FileSystem.OpenAppPackageFileAsync("wordlist.txt");

            // Check stream state
            if (stream == null)
            {
                System.Diagnostics.Debug.WriteLine("Stream is null.");
                return;
            }

            using var reader = new StreamReader(stream);

            wordList = new List<string>();
            int index = 0;

            while (reader.Peek() >= 0)
            {
                string? line = await reader.ReadLineAsync();

                if (!string.IsNullOrWhiteSpace(line))
                {
                    string word = line.Trim().ToUpper();

                    if (word.Length <= Columns || word.Length <= Rows)
                    {
                        wordList.Add(word);
                        System.Diagnostics.Debug.WriteLine($"Loaded word: {word}");
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"Total words loaded: {wordList.Count}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Exception in LoadWordListAsync: {ex.Message}");
        }
    }

    private char[,] GenerateWordSearchGrid()
    {
        char[,] grid = new char[Rows, Columns];
        bool[,] occupied = new bool[Rows, Columns];

        // Filter usable words (length must fit within grid)
        usableWords = wordList
            .Where(w => w.Length <= Math.Max(Columns, Rows))
            .OrderBy(_ => random.Next())
            .Take(10) // adjust number of words you want to place
            .ToList();

  



        foreach (var word in usableWords)
        {
            bool placed = false;
            for (int attempt = 0; attempt < 100 && !placed; attempt++)
            {
                bool horizontal = random.Next(2) == 0;
                int maxRow = horizontal ? Rows : Rows - word.Length;
                int maxCol = horizontal ? Columns - word.Length : Columns;

                int startRow = random.Next(maxRow);
                int startCol = random.Next(maxCol);

                // Check for space and compatibility
                bool canPlace = true;
                for (int i = 0; i < word.Length; i++)
                {
                    int r = startRow + (horizontal ? 0 : i);
                    int c = startCol + (horizontal ? i : 0);

                    if (occupied[r, c] && grid[r, c] != word[i])
                    {
                        canPlace = false;
                        break;
                    }
                }

                if (canPlace)
                {
                    for (int i = 0; i < word.Length; i++)
                    {
                        int r = startRow + (horizontal ? 0 : i);
                        int c = startCol + (horizontal ? i : 0);
                        grid[r, c] = word[i];
                        occupied[r, c] = true;
                    }

                    totalWords.Add(word);
                    System.Diagnostics.Debug.WriteLine($"Placed word '{word}' at ({startRow}, {startCol}) {(horizontal ? "horizontally" : "vertically")}");
                    placed = true;
                }
            }
        }

        // Fill remaining spaces with random letters
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                if (!occupied[r, c])
                    grid[r, c] = (char)('A' + random.Next(26));
            }
        }


        System.Diagnostics.Debug.WriteLine($"Total number of words in the grid are: {totalWords.Count}");

        return grid;
    }

    private void SetupGrid()
    {
        WordGrid.ColumnDefinitions.Clear();
        WordGrid.RowDefinitions.Clear();
        WordGrid.Children.Clear();
        

        for (int col = 0; col < Columns; col++)
            WordGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });

        for (int row = 0; row < Rows; row++)
            WordGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40) });

        char[,] gridLetters = GenerateWordSearchGrid();

        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                char letter = gridLetters[row, col];
                var button = new Button
                {
                    Text = letter.ToString(),
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
                    CommandParameter = (row, col, letter)
                };

                button.Clicked += OnGridButtonClicked;

                WordGrid.Children.Add(button);
                Grid.SetRow(button, row);
                Grid.SetColumn(button, col);
            }
        }
    }



}