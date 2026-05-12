using System;
using CrownsGame.Core;

namespace CrownGame
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1. Cream un board de 8x8, unde avem nevoie de 1 coroana per grup (Easy)
            int size = 8;
            int crownsNeeded = 1;
            Board myBoard = new Board(size, crownsNeeded);

            // 2. Initializam celulele (in mod normal asta o va face un BoardGenerator)
            // Pentru test, le punem pe toate in Regiunea 0
            for (int r = 0; r < size; r++)
            {
                for (int c = 0; c < size; c++)
                {
                    myBoard.InitializeCell(r, c, 0);
                }
            }

            // 3. Simulam plasarea unei coroane la pozitia (2, 3)
            myBoard.SetCellState(2, 3, CellState.Crown);
            
            // 4. Simulam un punct de marcare la (0, 0)
            myBoard.SetCellState(0, 0, CellState.Marked);

            // 5. Afisam board-ul in consola
            DisplayBoard(myBoard);

            Console.WriteLine("\nBoard-ul a fost initializat cu succes!");
            Console.ReadLine();
        }

        static void DisplayBoard(Board board)
        {
            for (int r = 0; r < board.Size; r++)
            {
                for (int c = 0; c < board.Size; c++)
                {
                    Cell currentCell = board.GetCell(r, c);
                    
                    // Alegem ce caracter sa afisam in functie de stare
                    if (currentCell.State == CellState.Crown)
                    {
                        Console.Write("[W] "); // W de la Crown
                    }
                    else if (currentCell.State == CellState.Marked)
                    {
                        Console.Write("[X] "); // X pentru marcat
                    }
                    else
                    {
                        Console.Write("[.] "); // Punct pentru gol
                    }
                }
                // Trecem la randul urmator
                Console.WriteLine();
            }
        }
    }
}