using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CrownsGame.Application;
using CrownsGame.Logic;
using CrownsGame.Core;

namespace CrownsGame.UI.Views;

public partial class MainWindow : Window
{
    private GameEngine? _engine;

    private readonly IBrush[] _regionColors = {
        Brushes.LightCoral, Brushes.LightBlue, Brushes.LightGreen,
        Brushes.PaleGoldenrod, Brushes.Plum, Brushes.LightSalmon,
        Brushes.MediumTurquoise, Brushes.Thistle
    };

    public MainWindow()
    {
        InitializeComponent();
        StartNewGame();
    }

    private void StartNewGame()
    {
        _engine = new GameEngine(new EasyStrategy());
        CreateGrid();
        UpdateUI();
    }

    private void CreateGrid()
    {
        var grid = this.FindControl<Grid>("GameGrid")!;
        var board = _engine!.State.Board;
        int size = board.Size;

        grid.RowDefinitions.Clear();
        grid.ColumnDefinitions.Clear();
        grid.Children.Clear();

        for (int i = 0; i < size; i++)
        {
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        }

        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                var cell = board.GetCell(r, c);
                var btn = new Button
                {
                    Margin = new Thickness(1),
                    Background = _regionColors[cell.RegionId % _regionColors.Length],
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1),
                    HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
                    FontSize = 22,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch,
                };

                Grid.SetRow(btn, r);
                Grid.SetColumn(btn, c);

                int row = r, col = c;
                btn.Click += (s, e) => {
                    _engine!.HandleCellClick(row, col);
                    UpdateUI();
                };

                grid.Children.Add(btn);
            }
        }
    }

    private void UpdateUI()
    {
        var grid = this.FindControl<Grid>("GameGrid")!;
        var board = _engine!.State.Board;
        var label = this.FindControl<TextBlock>("StatusLabel")!;

        label.Text = $"Scor: {_engine.State.Score} | Greșeli: {_engine.State.Mistakes}";

        for (int i = 0; i < grid.Children.Count; i++)
        {
            var btn = (Button)grid.Children[i];
            int r = i / board.Size;
            int c = i % board.Size;
            var cell = board.GetCell(r, c);

            btn.Content = cell.State switch {
                CellState.Crown => "👑",
                CellState.Marked => "X",
                _ => ""
            };
        }

        if (_engine.State.IsVictory)
        {
            label.Text = "BRAVO! Ai câștigat! 🎉";
            label.Foreground = Brushes.Green;
        }
    }

    private void OnUndoClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _engine!.Undo();
        UpdateUI();
    }

    private void OnNewGameClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) 
        => StartNewGame();
}