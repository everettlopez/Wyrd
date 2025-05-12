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

    public List<Button> selectedButtons = new List<Button>();       // Helpful to visualize which buttons to change
    private Dictionary<string, List<(int row, int col)>> wordPositions = new();
    private Dictionary<(int row, int col), List<string>> sharedCellMap = new();





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

    private async void BackBtnClicked(object sender, EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("Back button has been clicked");

        // Navigate to Main page
        await Navigation.PopAsync(); // Goes back to the previous page in the navigation stack
    }

    private async void NewGameClicked(object sender, EventArgs e)
    {
        // Recreate the current page to ensure full reset
        var newPage = new GamePage(); // or whatever your page is called
        await Navigation.PushAsync(newPage);
        Navigation.RemovePage(this); // remove old page from stack

        System.Diagnostics.Debug.WriteLine("New game initialized.");
    }


    private void OnGridButtonClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is ValueTuple<int, int, char> info)
        {
            var (row, col, letter) = info;

            if (!selectedButtons.Contains(button))
            {
                selectedButtons.Add(button);
                selectedLetters.Append(letter);
                LetterBox.Text = selectedLetters.ToString();
            }

            System.Diagnostics.Debug.WriteLine($"Clicked cell [{row}, {col}] with letter '{letter}'");
        }
    }


    private void ConfirmButtonClicked(object sender, EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("Confirm button has been clicked");

        String result = selectedLetters.ToString().ToUpper();

        if (totalWords.Contains(result) && !foundWords.Contains(result))
        {
            System.Diagnostics.Debug.WriteLine("Correct");

            foundWords.Add(result);
            UpdateProgressBar();

            if(foundWords.Count == totalWords.Count)
            {
                NewGameBtn.IsEnabled = true;
            }

            ProgressText.IsVisible = true;
            ProgressText.Text = $"{foundWords.Count}/{totalWords.Count} Words Found";

            foreach (var button in selectedButtons)
            {
                button.BackgroundColor = Colors.Gray;
                button.IsEnabled = false;

                // Extract row and col from the CommandParameter
                if (button.CommandParameter is ValueTuple<int, int, char> tuple)
                {
                    int row = tuple.Item1;
                    int col = tuple.Item2;

                    if (sharedCellMap.TryGetValue((row, col), out var words))
                    {
                        var sharedWith = words.Where(w => w != result && !foundWords.Contains(w)).ToList();
                        if (sharedWith.Any())
                        {
                            
                            System.Diagnostics.Debug.WriteLine($"Letter at ({row},{col}) is also used in: {string.Join(", ", sharedWith)}");

                            button.IsEnabled = true;
                            button.BackgroundColor = Colors.Orange;

                        }
                    }
                }
            }

            selectedButtons.Clear();
        }
        else if (totalWords.Contains(result) && foundWords.Contains(result))
        {
            System.Diagnostics.Debug.WriteLine("Correct, but you've already found this word.");

            ProgressText.IsVisible = true;
            ProgressText.Text = $"{foundWords.Count}/{totalWords.Count} Words Found";

            DisplayAlert("Word Already Found", "You have already used this word in an attempt. Don'r give up and try again.", "Continue");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("Incorrect");

            ProgressText.IsVisible = true;
            ProgressText.Text = $"{foundWords.Count}/{totalWords.Count} Words Found";
        }
       
        System.Diagnostics.Debug.WriteLine($"The result is {result}");

    }

    private void ClearButtonClicked(object sender, EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("Clear button has been clicked");

        selectedButtons.Clear();
        selectedLetters.Clear();
        LetterBox.Text = "";
    }

    private void HelpButtonClicked(object sender, EventArgs e)
    {
        string helpMessage = "Welcome to Wyrd Search!\n\n" +
                             "Objective:\n" +
                             "Find all the hidden words in the letter grid. Words can be placed horizontally, vertically, or diagonally and may go forwards or backwards.\n\n" +
                             "How to Play:\n" +
                             "1. Look at the list of words you need to find.\n" +
                             "2. Scan the grid carefully to locate each word.\n" +
                             "3. When you spot a word, tap and drag across the letters to highlight it.\n" +
                             "4. If the selection is correct, the word will be marked as found and removed from the list.\n\n" +
                             "Tips:\n" +
                             "- Look for unique letter combinations.\n" +
                             "- Start with longer words — they’re easier to spot.\n" +
                             "- Use hints if you're stuck (if available).\n\n" +
                             "Enjoy the challenge and have fun sharpening your mind!";

        DisplayAlert("How to Play Wyrd Search", helpMessage, "Got it!");
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
        try
        {
            // Defensive: Ensure Rows and Columns are valid
            if (Rows <= 0 || Columns <= 0)
                throw new InvalidOperationException("Grid dimensions must be greater than zero.");

            // Defensive: Ensure wordList is loaded
            if (wordList == null || wordList.Count == 0)
                throw new InvalidOperationException("Word list is empty or not loaded.");

            char[,] grid = new char[Rows, Columns];
            bool[,] occupied = new bool[Rows, Columns];

            // Filter usable words
            usableWords = wordList
                .Where(w => w.Length <= Math.Max(Columns, Rows))
                .OrderBy(_ => random.Next())
                .Take(10)
                .ToList();

            foreach (var word in usableWords)
            {
                bool placed = false;

                for (int attempt = 0; attempt < 100 && !placed; attempt++)
                {
                    bool horizontal = random.Next(2) == 0;

                    int maxRow = horizontal ? Rows : Rows - word.Length + 1;
                    int maxCol = horizontal ? Columns - word.Length + 1 : Columns;

                    // Defensive: Ensure maxRow and maxCol are valid
                    if (maxRow <= 0 || maxCol <= 0)
                        continue;

                    int startRow = random.Next(maxRow);
                    int startCol = random.Next(maxCol);

                    // Check if the word can be placed
                    bool canPlace = true;
                    for (int i = 0; i < word.Length; i++)
                    {
                        int r = startRow + (horizontal ? 0 : i);
                        int c = startCol + (horizontal ? i : 0);

                        // Defensive: Bounds check
                        if (r >= Rows || c >= Columns || (occupied[r, c] && grid[r, c] != word[i]))
                        {
                            canPlace = false;
                            break;
                        }
                    }

                    if (canPlace)
                    {
                        var positions = new List<(int, int)>();

                        for (int i = 0; i < word.Length; i++)
                        {
                            int r = startRow + (horizontal ? 0 : i);
                            int c = startCol + (horizontal ? i : 0);
                            grid[r, c] = word[i];
                            occupied[r, c] = true;
                            positions.Add((r, c));
                        }

                        wordPositions[word] = positions;
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

            System.Diagnostics.Debug.WriteLine($"Total number of words in the grid: {totalWords.Count}");


            sharedCellMap = new Dictionary<(int, int), List<string>>();

            foreach (var kvp in wordPositions)
            {
                string word = kvp.Key;
                foreach (var pos in kvp.Value)
                {
                    if (!sharedCellMap.ContainsKey(pos))
                        sharedCellMap[pos] = new List<string>();
                    sharedCellMap[pos].Add(word);
                }
            }


            return grid;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Exception in GenerateWordSearchGrid: {ex.Message}\n{ex.StackTrace}");
            throw; // Optionally rethrow to let the caller handle it
        }
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
                    WidthRequest = 50,
                    HeightRequest = 50,
                    Padding = 0,
                    Margin = new Thickness(0),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    CommandParameter = (row, col, letter),
                    BorderWidth = 6
                
                };

                button.Clicked += OnGridButtonClicked;

                WordGrid.Children.Add(button);
                Grid.SetRow(button, row);
                Grid.SetColumn(button, col);
            }
        }
    }



}