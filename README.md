# Crowns — Logic Puzzle Game

A desktop logic puzzle game built in C# with Avalonia, in the style of grid placement puzzles like "Queens": place crowns on an N×N board so that no two crowns share a row, column, region, or are adjacent to each other.


## My Contribution

I owned the **game logic layer** (`CrownsGame.Logic`) and contributed to the UI:

- **`Validator`** — enforces all placement rules: row/column/region limits and adjacency restrictions.
- **`BoardGenerator`** — procedurally generates valid puzzle boards.
- **Difficulty strategies** (`EasyStrategy`, `MediumStrategy`, `HardStrategy`, `DailyChallengeStrategy`) implementing a shared `IGameStrategy` interface — a Strategy pattern controlling win conditions and board parameters per mode.
- **Undo/redo system** (`ICommand`, `MoveCommand`, `CommandManager`) — a Command pattern wrapping each move so it can be reversed.
- Contributed to the **Avalonia/MVVM UI** (`MainWindow.axaml.cs`).

Other contributors built the `Core` layer (board/cell/position primitives) and the `AI` layer (a backtracking solver used for hints and daily-challenge generation).

## Overview

The game presents a grid divided into colored regions. The player places crowns so that:
- Each row, column, and region contains exactly the required number of crowns
- No two crowns are adjacent (including diagonally)

<p align="center">
  <img src="/Users/cristianacretu/Desktop/proiect-IP/screen.png" width="300" alt="alt text">
</p>

## Features

- Daily Challenge mode with a fixed seed per day
- Three difficulty levels (Easy / Medium / Hard)
- Undo/redo for every move
- An AI hint system that checks whether the current board state is still solvable
- Unit tests covering the rule-validation logic

## Tech Stack

- **Language:** C#
- **UI Framework:** Avalonia (cross-platform, MVVM)
- **Testing:** xUnit
- **Patterns used:** Command (undo/redo), Strategy (difficulty modes)

## Project Structure

```
CrownsGame/
├── CrownsGame.Core/         # Board, Cell, Move, Position primitives
├── CrownsGame.Logic/        # Validator, BoardGenerator, difficulty strategies, Command pattern
├── CrownsGame.Application/  # GameEngine, GameState, InputHandler, DailyChallengeManager
├── CrownsGame.AI/           # Backtracking solver (HintEngine, MiniSolver)
├── CrownsGame.UI/           # Avalonia UI (Views, ViewModels — MVVM)
└── CrownsGame.Tests/        # Unit tests
```

## How to Run

```bash
cd CrownsGame.UI
dotnet run 
```

