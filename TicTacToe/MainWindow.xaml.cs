using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace TicTacToe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _totalMoves;
        private char[] _fields;
        private bool _gameWon;
        private Button[] _fieldButtons;

        public MainWindow()
        {
            InitializeComponent();

            // Initialize empty char-array
            _fields = new char[9];

            // Reference collection of all field buttons
            _fieldButtons = [field_btn_0, field_btn_1, field_btn_2, field_btn_3, field_btn_4, field_btn_5, field_btn_6, field_btn_7, field_btn_8];

            // Setting basic startup values
            game_status.Text = "Make your move";
            reset_btn.IsEnabled = false;
        }

        // Event/method triggered when pushing a game field button
        private void GameTurn(object sender, RoutedEventArgs e)
        {
            if (!_gameWon && _totalMoves < 9)
            {
                if (!ValidPlayerMove(sender))
                {
                    return;
                }
            }
            if (!_gameWon && _totalMoves < 9)
            {
                ComputerMove();
            }
            if (!_gameWon && _totalMoves == 9)
            {
                game_status.Text = "Its a tie!";
            }

            if (_totalMoves > 0)
            {
                reset_btn.IsEnabled = true;
            }
        }

        // Method for the player (human move).
        // Returns a bool indicating if a non-occupied field was selected.
        private bool ValidPlayerMove(object sender)
        {
            if (sender is Button button)
            {
                // Extract the int field position.
                var pos = Convert.ToInt32(button.Tag);

                // Check if field position is available before occupying.
                
                if (_fields[pos] == 0)
                {
                    button.Content = "X";
                    button.FontSize = 36;
                    button.FontWeight = FontWeights.ExtraBold;
                    button.Foreground = Brushes.Blue;

                    _fields[pos] = 'p';
                    _totalMoves++;
                    game_status.Text = $"Moves: {_totalMoves.ToString()}";

                    CheckWinningMove();

                    if (_gameWon)
                    {
                        game_status.Text = $"You won the game in {_fields.Count(f => f == 'p')} moves!";
                    }

                    return true;
                }
            }

            game_status.Text = "Field already occupied!";
            return false;
        }

        private async void ComputerMove()
        {
            await ComputerMoveDelay();

            var pos = Random.Shared.Next(8);

            while (_fields[pos] != 0)
            {
                pos = Random.Shared.Next(8);
            }

            Button button = _fieldButtons[pos];

            button.Content = "O";
            button.FontSize = 36;
            button.FontWeight = FontWeights.ExtraBold;
            button.Foreground = Brushes.Red;

            _fields[pos] = 'c';
            _totalMoves++;
            game_status.Text = $"Moves: {_totalMoves.ToString()}";

            CheckWinningMove();

            if (_gameWon)
            {
                game_status.Text = $"Computer won the game in {_fields.Count(f => f == 'c')} moves!";
            }
        }

        // Method for comparing moves vs winning patterns, ie. 3 in a row.
        private void CheckWinningMove()
        {
            int[][] winPatterns =
            [
                [0, 1, 2],
                [3, 4, 5],
                [6, 7, 8],
                [0, 3, 6],
                [1, 4, 7],
                [2, 5, 8],
                [0, 4, 8],
                [2, 4, 6]
            ];

            foreach (var pattern in winPatterns)
            {
                if (_fields[pattern[0]] != 0)
                {
                    if (_fields[pattern[0]] == _fields[pattern[1]] &&
                        _fields[pattern[1]] == _fields[pattern[2]])
                    {
                        _gameWon = true;
                        reset_btn.Content = "Restart";
                        ToggleButtons();

                        int[] winningFieldNumbers = { pattern[0], pattern[1], pattern[2] };
                        Button[] winningButtons = _fieldButtons.Where(button => winningFieldNumbers.Contains(Convert.ToInt32(button.Tag))).ToArray();
                        Button[] otherButtons = _fieldButtons.Except(winningButtons).ToArray();

                        foreach (var button in otherButtons)
                        {
                            button.Opacity = 0.35;
                        }
                    }
                }
            }
        }

        // Delay method for increased user experience
        private async Task ComputerMoveDelay()
        {
            ToggleButtons();
            await Task.Delay(750);
            ToggleButtons();
        }

        // Reset method that clears game memory and restores initial setup.
        private void ResetBtnClick(object sender, RoutedEventArgs e)
        {
            _fields = new char[9];
            _totalMoves = 0;
            _gameWon = false;
            game_status.Text = "Make your move";
            reset_btn.IsEnabled = false;
            reset_btn.Content = "Reset";

            foreach (var button in _fieldButtons)
            {
                button.IsEnabled = true;
                button.Opacity = 1;
                button.Content = string.Empty;
                button.BorderBrush = Brushes.Transparent;
            }
        }

        // Method for toggling all game-field buttons IsEnabled property
        public void ToggleButtons()
        {
            foreach (var button in _fieldButtons)
            {
                if (button.IsEnabled)
                {
                    button.IsEnabled = false;
                }
                else
                {
                    button.IsEnabled = true;
                }
            }
        }
    }
}