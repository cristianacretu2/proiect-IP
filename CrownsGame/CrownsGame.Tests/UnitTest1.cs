/*
 * Autor: Cristiana Cretu
 * Proiect: CrownsGame.Tests
 * Functionalitate: Testarea unitară adaptată pentru clasa Board
 */
using Xunit;
using CrownsGame.Core;
using CrownsGame.Logic;

namespace CrownsGame.Tests
{
    public class GameLogicTests
    {
        // Helper pentru a crea o tablă inițializată (fără celule null)
        private Board CreateInitializedBoard(int size, int crowns = 1)
        {
            var board = new Board(size, crowns);
            for (int r = 0; r < size; r++)
            {
                for (int c = 0; c < size; c++)
                {
                    // Inițializăm fiecare celulă cu un RegionId (ex: 0)
                    board.InitializeCell(r, c, 0);
                }
            }
            return board;
        }

        [Fact]
        public void Test_BoardInitialization_PropertiesShouldMatch()
        {
            // Arrange & Act
            var board = new Board(8, 1);

            // Assert
            Assert.Equal(8, board.Size);
            Assert.Equal(1, board.CrownsPerGroup);
        }

        [Fact]
        public void Test_SetCellState_ShouldUpdateStateCorrectly()
        {
            // Arrange
            var board = CreateInitializedBoard(8);
            
            // Act
            board.SetCellState(2, 2, CellState.Crown);

            // Assert
            Assert.Equal(CellState.Crown, board.GetCell(2, 2).State);
        }

        [Fact]
        public void Test_Clone_ShouldCreateDeepCopy()
        {
            // Arrange
            var original = CreateInitializedBoard(5);
            original.SetCellState(0, 0, CellState.Crown);

            // Act
            var clone = original.Clone();

            // Assert
            Assert.NotSame(original, clone); // Sunt obiecte diferite în memorie
            Assert.Equal(original.GetCell(0, 0).State, clone.GetCell(0, 0).State);
            
            // Modificăm originalul, clonul nu ar trebui să se schimbe
            original.SetCellState(0, 0, CellState.Empty);
            Assert.Equal(CellState.Crown, clone.GetCell(0, 0).State);
        }

        [Fact]
        public void Test_Validator_ShouldRejectAdjacentCrowns()
        {
            // Arrange
            var board = CreateInitializedBoard(8);
            var validator = new Validator(1);
            board.SetCellState(3, 3, CellState.Crown);

            // Act
            // Verificăm o poziție adiacentă (3, 4)
            bool isValid = validator.IsMoveValid(board, 3, 4);

            // Assert
            Assert.False(isValid, "Validatorul trebuie să blocheze plasarea lângă o altă coroană.");
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(4, 4)]
        public void Test_GetCell_ShouldReturnInitializedCell(int r, int c)
        {
            // Arrange
            var board = CreateInitializedBoard(5);

            // Act
            var cell = board.GetCell(r, c);

            // Assert
            Assert.NotNull(cell);
        }
    }
}