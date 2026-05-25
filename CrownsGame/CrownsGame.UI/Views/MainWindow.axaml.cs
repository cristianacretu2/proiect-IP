/*
 * Scopul fisierului: Implementeaza logica de interfata (Code-behind) pentru fereastra principala, 
 * gestionand interactiunea dintre utilizator si motorul de joc.
 * Autor: 
 */

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Interactivity;
using CrownsGame.Application;
using CrownsGame.Logic;
using CrownsGame.Core;
using CrownsGame.AI;
using System;
using System.Diagnostics;

namespace CrownsGame.UI.Views;

/// <summary>
/// Fereastra principala a aplicatiei care gestioneaza grid-ul de joc, butoanele de control si afisarea progresului.
/// </summary>
public partial class MainWindow : Window
{
    private GameEngine?   _engine;
    private HintEngine?   _hintEngine;
    private MiniSolver?   _miniSolver;
    private IGameStrategy _currentStrategy = new EasyStrategy();

    // Pentru daily challenge 
    private DailyChallengeManager? _dailyManager;
    private bool _isDailyMode = false;

    private readonly Color[] _regionColors =
    {
        Color.FromRgb(185,  65,  65),   // 0  roșu mat
        Color.FromRgb( 52, 115, 180),   // 1  albastru cobalt
        Color.FromRgb( 46, 148,  90),   // 2  verde
        Color.FromRgb(180, 140,  30),   // 3  galben ocru
        Color.FromRgb(140,  60, 175),   // 4  violet
        Color.FromRgb(190, 100,  30),   // 5  portocaliu
        Color.FromRgb( 30, 155, 160),   // 6  teal
        Color.FromRgb(175,  60, 120),   // 7  roz intens
        Color.FromRgb( 90, 130,  50),   // 8  verde oliv
        Color.FromRgb( 60,  90, 170),   // 9  albastru royal
        Color.FromRgb(160,  80,  50),   // 10 cărămiziu
        Color.FromRgb( 80, 160, 140),   // 11 verde-teal
        Color.FromRgb(150,  50, 155),   // 12 mov
        Color.FromRgb(140, 120,  40),   // 13 kaki
        Color.FromRgb( 60, 120, 150),   // 14 albastru-gri
    };

    /// <summary>
    /// Constructorul ferestrei: initializeaza componentele vizuale si porneste o partida noua pe dificultate Easy.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        StartNewGame(new EasyStrategy());
    }


    /// <summary>
    /// Initializeaza o sesiune de joc noua, asigurandu-se prin incercari repetate ca tabla generata are o solutie valida.
    /// </summary>
    /// <param name="strategy">Strategia de dificultate aleasa.</param>
    private void StartNewGame(IGameStrategy strategy)
    {
        _currentStrategy = strategy;
        HighlightActiveButton(strategy);

        int attempts = 0;
        bool isSolvable = false;

        // Se incearca generarea unui board pana cand solver-ul confirma ca acesta poate fi rezolvat.
        while (!isSolvable && attempts < 10)
        {
            _engine    = new GameEngine(strategy);
            var validator = new Validator(strategy);
            _miniSolver   = new MiniSolver(validator, strategy);

            if (_miniSolver.IsSolvable(_engine.State.Board))
            {
                isSolvable  = true;
                _hintEngine = new HintEngine(validator, strategy);
            }
            attempts++;
        }

        if (!isSolvable)
        {
            var lbl = this.FindControl<TextBlock>("StatusLabel")!;
            lbl.Text      = "Eroare: nu s-a putut genera un board rezolvabil.";
            lbl.Foreground = new SolidColorBrush(Colors.OrangeRed);
            return;
        }

        CreateGrid();
        UpdateUI();
    } 

   /* private void StartNewGame(IGameStrategy strategy)
{
    _currentStrategy = strategy;
    HighlightActiveButton(strategy);

    // TEST: generăm fără verificare solver
    _engine = new GameEngine(strategy);
    var validator = new Validator(strategy);
    _miniSolver = new MiniSolver(validator, strategy);
    _hintEngine = new HintEngine(validator, strategy);

    CreateGrid();
    UpdateUI();
} */

    
    /// <summary>
    /// Construieste dinamic matricea de butoane in interfata grafica, adaptand dimensiunea celulelor la marimea tablei.
    /// </summary>
    private void CreateGrid()
    {
        var grid  = this.FindControl<Grid>("GameGrid")!;
        var board = _engine!.State.Board;
        int size  = board.Size;

        grid.RowDefinitions.Clear();
        grid.ColumnDefinitions.Clear();
        grid.Children.Clear();

        // Calculam dimensiunea celulei pentru a incapea in containerul de 460px.
        double cellSize = Math.Min(68, 460.0 / size);

        for (int i = 0; i < size; i++)
        {
            grid.RowDefinitions.Add(new RowDefinition(cellSize, GridUnitType.Pixel));
            grid.ColumnDefinitions.Add(new ColumnDefinition(cellSize, GridUnitType.Pixel));
        }

        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                var cell   = board.GetCell(r, c);
                var baseColor = _regionColors[cell.RegionId % _regionColors.Length];

                var btn = new Button
                {
                    Background       = MakeBrush(baseColor, 1.0),
                    BorderBrush      = new SolidColorBrush(Color.FromArgb(120, 0, 0, 0)),
                    BorderThickness  = new Thickness(1),
                    Margin           = new Thickness(0),
                    Padding          = new Thickness(0),
                    FontSize         = size <= 8 ? 22 : size <= 10 ? 18 : 15,
                    HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalContentAlignment   = Avalonia.Layout.VerticalAlignment.Center,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                    VerticalAlignment   = Avalonia.Layout.VerticalAlignment.Stretch,
                    CornerRadius = new CornerRadius(0),
                    // Tag pentru a putea regăsi culoarea de bază la UpdateUI
                    Tag = cell.RegionId,
                };

                Grid.SetRow(btn, r);
                Grid.SetColumn(btn, c);

                int row = r, col = c;
                btn.Click += (_, _) =>
                {
                    _engine!.HandleCellClick(row, col);
                    UpdateUI();
                };

                grid.Children.Add(btn);
            }
        }
    }


    /// <summary>
    /// Actualizeaza aspectul vizual al tablei (culori, simboluri) si textele de stare pe baza schimbarilor din GameState.
    /// </summary>
    private void UpdateUI()
    {
        if (_engine == null) return;

        var grid  = this.FindControl<Grid>("GameGrid")!;
        var board = _engine.State.Board;
        var label = this.FindControl<TextBlock>("StatusLabel")!;

        label.Foreground = new SolidColorBrush(Color.FromRgb(208, 208, 224));
        label.Text = $"Scor: {_engine.State.Score} | Greșeli: {_engine.State.Mistakes}";

        for (int i = 0; i < grid.Children.Count; i++)
        {
            if (grid.Children[i] is not Button btn) continue;

            int r    = i / board.Size;
            int c    = i % board.Size;
            var cell = board.GetCell(r, c);

            var baseColor = _regionColors[cell.RegionId % _regionColors.Length];

            // Feedback vizual: intunecam celulele marcate cu X si luminam cele cu coroana.
            btn.Background = cell.State switch
            {
                CellState.Crown  => MakeBrush(Lighten(baseColor, 0.25), 1.0),
                CellState.Marked => MakeBrush(Darken(baseColor,  0.30), 1.0),
                _                => MakeBrush(baseColor, 1.0),
            };

            btn.BorderBrush     = new SolidColorBrush(Color.FromArgb(140, 0, 0, 0));
            btn.BorderThickness = new Thickness(1);

            btn.Content = cell.State switch
            {
                CellState.Crown  => "👑",
                CellState.Marked => "✕",
                _                => "",
            };
        }

        if (_engine.State.IsVictory)
        {
            if (_isDailyMode && _dailyManager != null)
            {
                _dailyManager.IncrementScore();
            
                // Aici folosim label-ul pentru feedback rapid
                label.Text = $"Bravo! Următorul! (Total: {_dailyManager.GamesSolved})";
                label.Foreground = new SolidColorBrush(Color.FromRgb(255, 215, 0)); // Auriu pentru Daily

                // Generăm imediat următorul board
                StartNewGame(new DailyChallengeStrategy());
            }
            else
            {
                label.Text = "BRAVO! Ai câștigat! 🎉";
                label.Foreground = new SolidColorBrush(Color.FromRgb(100, 220, 120));
            }
        }
    }

    /// <summary>
    /// Schimba aspectul butoanelor de dificultate pentru a indica vizual setarea activa.
    /// </summary>
    /// <param name="strategy">Strategia curenta de joc.</param>
    private void HighlightActiveButton(IGameStrategy strategy)
    {
        var easy   = this.FindControl<Button>("BtnEasy")!;
        var medium = this.FindControl<Button>("BtnMedium")!;
        var hard   = this.FindControl<Button>("BtnHard")!;

        var inactive = new SolidColorBrush(Color.FromRgb(55, 55, 80));
        var active   = new SolidColorBrush(Color.FromRgb(224, 201, 127));

        easy.Background   = inactive;
        medium.Background = inactive;
        hard.Background   = inactive;

        easy.Foreground   = Brushes.White;
        medium.Foreground = Brushes.White;
        hard.Foreground   = Brushes.White;

        var target = strategy switch
        {
            EasyStrategy   => easy,
            MediumStrategy => medium,
            HardStrategy   => hard,
            _              => easy,
        };

        target.Background = active;
        target.Foreground = new SolidColorBrush(Color.FromRgb(30, 30, 50));
    }

    /// <summary> Handler pentru setarea dificultatii Easy. </summary>
    private void OnEasyClick  (object? s, RoutedEventArgs e) => StartNewGame(new EasyStrategy());

    /// <summary> Handler pentru setarea dificultatii Medium. </summary>
    private void OnMediumClick(object? s, RoutedEventArgs e) => StartNewGame(new MediumStrategy());

    /// <summary> Handler pentru setarea dificultatii Hard. </summary>
    private void OnHardClick  (object? s, RoutedEventArgs e) => StartNewGame(new HardStrategy());


    /// <summary> Solicita motorului de joc anularea ultimei mutari si actualizeaza UI-ul. </summary>
    private void OnUndoClick(object? s, RoutedEventArgs e) { _engine?.Undo(); UpdateUI(); }

    /// <summary> Reporneste jocul curent mentinand aceeasi strategie. </summary>
    private void OnNewGameClick(object? s, RoutedEventArgs e) => StartNewGame(_currentStrategy);

    /// <summary>
    /// Deschide fisierul de ajutor (CHM) prin intermediul proceselor de sistem.
    /// </summary>
    private void OnHelpClick(object? s, RoutedEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "Manual de utilizare - CrownsGame.chm",
                UseShellExecute = true
            });
        }
        catch (Exception)
        {
            var label = this.FindControl<TextBlock>("StatusLabel")!;
            label.Text = "Eroare: Nu s-a putut deschide fișierul de ajutor.";
            label.Foreground = new SolidColorBrush(Colors.OrangeRed);
        }
    }

    /// <summary>
    /// Solicita motorului de sugestii cea mai buna mutare si o aplica automat pe tabla.
    /// </summary>
    private void OnHintClick(object? s, RoutedEventArgs e)
    {
        if (_engine == null || _hintEngine == null) return;

        var hint = _hintEngine.GetBestHint(_engine.State.Board);
        if (!hint.HasValue) return;

        var cell = _engine.State.Board.GetCell(hint.Value.Row, hint.Value.Col);
        // Simulam click-uri pana cand starea celulei devine Crown.
        int safety = 0;
        while (cell.State != CellState.Crown && safety++ < 3)
        {
            _engine.HandleCellClick(hint.Value.Row, hint.Value.Col);
            cell = _engine.State.Board.GetCell(hint.Value.Row, hint.Value.Col);
        }

        // Highlight vizual pe celula hint
        var grid = this.FindControl<Grid>("GameGrid")!;
        int idx  = hint.Value.Row * _engine.State.Board.Size + hint.Value.Col;
        if (idx < grid.Children.Count && grid.Children[idx] is Button btn)
        {
            btn.BorderBrush     = new SolidColorBrush(Color.FromRgb(255, 230, 80));
            btn.BorderThickness = new Thickness(3);
        }

        UpdateUI();
    }

    /// <summary>
    /// Verifica daca starea curenta a tablei mai poate conduce catre o solutie valida.
    /// </summary>
    private void OnSolveCheckClick(object? s, RoutedEventArgs e)
    {
        if (_engine == null || _miniSolver == null) return;

        var label    = this.FindControl<TextBlock>("StatusLabel")!;
        bool solvable = _miniSolver.IsSolvable(_engine.State.Board);

        label.Text = solvable
            ? "Configurația e validă — mergi înainte! "
            : "Imposibil! Ai o greșeală undeva. ";
        label.Foreground = new SolidColorBrush(
            solvable ? Color.FromRgb(80, 200, 120) : Color.FromRgb(220, 80, 80));
    }

    /// <summary>
    /// Activeaza modul Daily Challenge, pornind cronometrul si gestionand evenimentele de timp.
    /// </summary>
    private void OnDailyChallengeClick(object? s, RoutedEventArgs e)
    {
        // Oprim orice sesiune anterioara
        _dailyManager?.Stop();

        _isDailyMode = true;
        _dailyManager = new DailyChallengeManager(60); // 1 minut

        
        _dailyManager.OnTick += (secondsRemaining) =>
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                var timerLabel = this.FindControl<TextBlock>("TimerLabel")!;
                timerLabel.Text = $"⏱️ {_dailyManager.GetFormattedTime()}";
                
                // Efect vizual când timpul e aproape gata
                if (secondsRemaining <= 10)
                    timerLabel.Foreground = Brushes.OrangeRed;
                else
                    timerLabel.Foreground = Brushes.White;
            });
        };

        _dailyManager.OnTimeUp += () =>
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                _isDailyMode = false;
                var label = this.FindControl<TextBlock>("StatusLabel")!;
                label.Text = $"TIMP EXPIRAT! Scor: {_dailyManager.GamesSolved} jocuri";
                label.Foreground = Brushes.Gold;
                
                this.FindControl<Grid>("GameGrid")!.IsEnabled = false;
            });
        };

        _dailyManager.Start();
        StartNewGame(new DailyChallengeStrategy());
        
        // Asiguram că grid ul este activ
        this.FindControl<Grid>("GameGrid")!.IsEnabled = true;
    }

    // curatare resurse 
    /// <summary>
    /// Asigura oprirea cronometrelor active la inchiderea ferestrei pentru a preveni scurgerile de memorie.
    /// </summary>
    protected override void OnClosing(WindowClosingEventArgs e)
    {
        _dailyManager?.Stop();
        base.OnClosing(e);
    }

    // help culori 

    private static SolidColorBrush MakeBrush(Color c, double alpha) =>
        new(Color.FromArgb((byte)(alpha * 255), c.R, c.G, c.B));

    private static Color Lighten(Color c, double amount) => Color.FromRgb(
        (byte)Math.Min(255, c.R + (255 - c.R) * amount),
        (byte)Math.Min(255, c.G + (255 - c.G) * amount),
        (byte)Math.Min(255, c.B + (255 - c.B) * amount));

    private static Color Darken(Color c, double amount) => Color.FromRgb(
        (byte)(c.R * (1 - amount)),
        (byte)(c.G * (1 - amount)),
        (byte)(c.B * (1 - amount))
    );
}