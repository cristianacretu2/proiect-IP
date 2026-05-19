using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Interactivity;
using CrownsGame.Application;
using CrownsGame.Logic;
using CrownsGame.Core;
using CrownsGame.AI;
using System;

namespace CrownsGame.UI.Views;

public partial class MainWindow : Window
{
    private GameEngine? _engine;
    private HintEngine? _hintEngine;
    private MiniSolver? _miniSolver;

    // Culori pentru regiuni (asigură-te că ID-urile regiunilor nu depășesc lungimea array-ului)
    private readonly IBrush[] _regionColors = {
        Brushes.LightCoral, Brushes.LightBlue, Brushes.LightGreen,
        Brushes.PaleGoldenrod, Brushes.Plum, Brushes.LightSalmon,
        Brushes.MediumTurquoise, Brushes.Thistle, Brushes.SandyBrown,
        Brushes.DarkSeaGreen, Brushes.SkyBlue
    };

    public MainWindow()
    {
        InitializeComponent();
        // Pornim implicit pe Easy la deschiderea aplicației
        StartNewGame(new EasyStrategy());
    }

    private void StartNewGame(IGameStrategy strategy)
    {
        bool isSolvable = false;
        int attempts = 0;

        // Loop de generare: nu ne oprim până nu avem un board cu soluție
        while (!isSolvable && attempts < 100)
        {
            _engine = new GameEngine(strategy);
            var validator = new Validator(strategy);
            _miniSolver = new MiniSolver(validator, strategy);

            if (_miniSolver.IsSolvable(_engine.State.Board))
            {
                isSolvable = true;
                _hintEngine = new HintEngine(validator, strategy);
            }
            attempts++;
        }

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

        // Definim rândurile și coloanele în grid-ul Avalonia
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
                    FontSize = size > 8 ? 18 : 22, // Ajustăm fontul pentru table mari
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
        if (_engine == null) return;

        var grid = this.FindControl<Grid>("GameGrid")!;
        var board = _engine.State.Board;
        var label = this.FindControl<TextBlock>("StatusLabel")!;

        // Resetăm stilul label-ului
        label.Foreground = Brushes.Black;
        label.Text = $"Scor: {_engine.State.Score} | Greșeli: {_engine.State.Mistakes}";

        // Actualizăm fiecare buton din grid
        for (int i = 0; i < grid.Children.Count; i++)
        {
            if (grid.Children[i] is Button btn)
            {
                int r = i / board.Size;
                int c = i % board.Size;
                var cell = board.GetCell(r, c);

                // Resetăm eventualele highlight-uri de la Hint
                btn.BorderBrush = Brushes.Black;
                btn.BorderThickness = new Thickness(1);

                btn.Content = cell.State switch {
                    CellState.Crown => "👑",
                    CellState.Marked => "X",
                    _ => ""
                };
            }
        }

        if (_engine.State.IsVictory)
        {
            label.Text = "BRAVO! Ai câștigat! 🎉";
            label.Foreground = Brushes.Green;
        }
    }

    // --- EVENIMENTE BUTOANE DIFICULTATE ---
    private void OnEasyClick(object? sender, RoutedEventArgs e) => StartNewGame(new EasyStrategy());
    private void OnMediumClick(object? sender, RoutedEventArgs e) => StartNewGame(new MediumStrategy());
    private void OnHardClick(object? sender, RoutedEventArgs e) => StartNewGame(new HardStrategy());

    // --- EVENIMENTE CONTROL ---
    private void OnUndoClick(object? sender, RoutedEventArgs e)
    {
        _engine?.Undo();
        UpdateUI();
    }

    private void OnNewGameClick(object? sender, RoutedEventArgs e) 
    {
        // Reîncepe jocul cu strategia curentă
        if (_engine != null)
        {
            // Aici poți decide dacă vrei să păstrezi strategia curentă
            // sau să pui una default.
            StartNewGame(new EasyStrategy()); 
        }
    }

    // --- EVENIMENTE AI ---
    private void OnHintClick(object? sender, RoutedEventArgs e)
    {
        if (_engine == null || _hintEngine == null) return;

        var hint = _hintEngine.GetBestHint(_engine.State.Board);
        if (hint.HasValue)
        {
            // Forțăm plasarea coroanei prin Engine
            _engine.HandleCellClick(hint.Value.Row, hint.Value.Col); // Devine Marked sau Crown
            // Ne asigurăm că ajunge în starea Crown
            while(_engine.State.Board.GetCell(hint.Value.Row, hint.Value.Col).State != CellState.Crown)
            {
                _engine.HandleCellClick(hint.Value.Row, hint.Value.Col);
            }
            UpdateUI();
        }
    }


    private void OnSolveCheckClick(object? sender, RoutedEventArgs e)
    {
        if (_engine == null || _miniSolver == null) return;

        bool solvable = _miniSolver.IsSolvable(_engine.State.Board);
        var label = this.FindControl<TextBlock>("StatusLabel")!;
        
        if (solvable) {
            label.Text = "Configurația este validă. Mergi înainte! ✅";
            label.Foreground = Brushes.DeepSkyBlue;
        } else {
            label.Text = "Imposibil! Ai făcut o greșeală anterior. ❌";
            label.Foreground = Brushes.Red;
        }
    }
}