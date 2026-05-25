/*
 * Scopul fisierului: Testarea unitara extinsa pentru componentele Core si Logic, verificand regulile de validare, clonarea tablei si generarea de board-uri.
 * Autor: Radani Antonia
 */

using Xunit;
using CrownsGame.Core;
using CrownsGame.Logic;

namespace CrownsGame.Tests
{
    /// <summary>
    /// Clasa de testare unitara pentru verificarea corectitudinii structurii Board, 
    /// a regulilor implementate in Validator si a procesului de generare a tablei.
    /// </summary>
    public class GameLogicTests
    {


        /// <summary>
        /// Metoda de tip helper care creeaza o tabla initializata unde toate celulele apartin aceleiasi regiuni (ID 0).
        /// </summary>
        /// <param name="size">Dimensiunea tablei.</param>
        /// <param name="crowns">Numarul de coroane per grup.</param>
        /// <returns>O instanta de Board gata de testare.</returns>
        private Board CreateInitializedBoard(int size, int crowns = 1)
        {
            var board = new Board(size, crowns);
            for (int r = 0; r < size; r++)
                for (int c = 0; c < size; c++)
                    board.InitializeCell(r, c, 0);
            return board;
        }

        /// <summary>
        /// Creeaza un board cu regiuni separate pe fiecare rand pentru a testa limitele regionale.
        /// </summary>
        /// <param name="size">Dimensiunea tablei.</param>
        /// <param name="crowns">Numarul de coroane per grup.</param>
        /// <returns>O instanta de Board cu regiuni mapate pe rânduri.</returns>
        private Board CreateMultiRegionBoard(int size, int crowns = 1)
        {
            var board = new Board(size, crowns);
            for (int r = 0; r < size; r++)
                for (int c = 0; c < size; c++)
                    board.InitializeCell(r, c, r); // regionId = numarul randului
            return board;
        }


        /// <summary>
        /// Verifica daca proprietatile de baza ale tablei sunt setate corect la instantiere.
        /// </summary>
        [Fact]
        public void Test_BoardInitialization_PropertiesShouldMatch()
        {
            var board = new Board(8, 1);
            Assert.Equal(8, board.Size);
            Assert.Equal(1, board.CrownsPerGroup);
        }

        /// <summary>
        /// Testeaza initializarea cu diverse dimensiuni si configuratii de coroane.
        /// </summary>
        [Theory]
        [InlineData(4, 1)]
        [InlineData(8, 1)]
        [InlineData(10, 2)]
        [InlineData(14, 3)]
        public void Test_BoardInitialization_DifferentSizes(int size, int crowns)
        {
            var board = new Board(size, crowns);
            Assert.Equal(size, board.Size);
            Assert.Equal(crowns, board.CrownsPerGroup);
        }

        /// <summary>
        /// Asigura ca toate celulele noi au starea initiala Empty.
        /// </summary>
        [Fact]
        public void Test_BoardInitialization_AllCellsStartEmpty()
        {
            var board = CreateInitializedBoard(6);
            for (int r = 0; r < 6; r++)
                for (int c = 0; c < 6; c++)
                    Assert.Equal(CellState.Empty, board.GetCell(r, c).State);
        }


        /// <summary>
        /// Verifica daca GetCell returneaza o celula valida pentru diverse coordonate.
        /// </summary>
        [Theory]
        [InlineData(0, 0)]
        [InlineData(4, 4)]
        [InlineData(7, 7)]
        public void Test_GetCell_ShouldReturnInitializedCell(int r, int c)
        {
            var board = CreateInitializedBoard(8);
            Assert.NotNull(board.GetCell(r, c));
        }

        /// <summary>
        /// Verifica daca regiunile sunt returnate corect pentru configuratia MultiRegion.
        /// </summary>
        [Fact]
        public void Test_GetCell_ShouldReturnCorrectRegionId()
        {
            var board = CreateMultiRegionBoard(5);
            for (int r = 0; r < 5; r++)
                for (int c = 0; c < 5; c++)
                    Assert.Equal(r, board.GetCell(r, c).RegionId);
        }


        /// <summary>
        /// Testeaza daca plasarea unei coroane actualizeaza corect starea celulei.
        /// </summary>
        [Fact]
        public void Test_SetCellState_ShouldUpdateStateCorrectly()
        {
            var board = CreateInitializedBoard(8);
            board.SetCellState(2, 2, CellState.Crown);
            Assert.Equal(CellState.Crown, board.GetCell(2, 2).State);
        }

        /// <summary>
        /// Verifica functionalitatea de marcare (X) a unei celule.
        /// </summary>
        [Fact]
        public void Test_SetCellState_MarkedState()
        {
            var board = CreateInitializedBoard(8);
            board.SetCellState(1, 1, CellState.Marked);
            Assert.Equal(CellState.Marked, board.GetCell(1, 1).State);
        }

        /// <summary>
        /// Verifica daca o celula poate fi resetata la starea Empty.
        /// </summary>
        [Fact]
        public void Test_SetCellState_ResetToEmpty()
        {
            var board = CreateInitializedBoard(8);
            board.SetCellState(0, 0, CellState.Crown);
            board.SetCellState(0, 0, CellState.Empty);
            Assert.Equal(CellState.Empty, board.GetCell(0, 0).State);
        }

        /// <summary>
        /// Asigura ca modificarea unei celule nu afecteaza starea celorlalte celule din jur.
        /// </summary>
        [Fact]
        public void Test_SetCellState_DoesNotAffectOtherCells()
        {
            var board = CreateInitializedBoard(5);
            board.SetCellState(2, 2, CellState.Crown);

            for (int r = 0; r < 5; r++)
                for (int c = 0; c < 5; c++)
                    if (r != 2 || c != 2)
                        Assert.Equal(CellState.Empty, board.GetCell(r, c).State);
        }


        /// <summary>
        /// Verifica daca metoda Clone realizeaza un Deep Copy (copie in profunzime).
        /// </summary>
        [Fact]
        public void Test_Clone_ShouldCreateDeepCopy()
        {
            var original = CreateInitializedBoard(5);
            original.SetCellState(0, 0, CellState.Crown);
            var clone = original.Clone();

            Assert.NotSame(original, clone);
            Assert.Equal(original.GetCell(0, 0).State, clone.GetCell(0, 0).State);

            original.SetCellState(0, 0, CellState.Empty);
            Assert.Equal(CellState.Crown, clone.GetCell(0, 0).State);
        }

        /// <summary>
        /// Confirma ca schimbarile efectuate pe clona nu se reflecta in instanta originala.
        /// </summary>
        [Fact]
        public void Test_Clone_ModifyingCloneDoesNotAffectOriginal()
        {
            var original = CreateInitializedBoard(5);
            var clone = original.Clone();

            clone.SetCellState(3, 3, CellState.Crown);
            Assert.Equal(CellState.Empty, original.GetCell(3, 3).State);
        }

        /// <summary>
        /// Asigura ca ID-urile de regiune sunt conservate dupa clonare.
        /// </summary>
        [Fact]
        public void Test_Clone_PreservesRegionIds()
        {
            var original = CreateMultiRegionBoard(5);
            var clone = original.Clone();

            for (int r = 0; r < 5; r++)
                for (int c = 0; c < 5; c++)
                    Assert.Equal(original.GetCell(r, c).RegionId, clone.GetCell(r, c).RegionId);
        }


        /// <summary>
        /// Testeaza regula care interzice plasarea coroanelor in celule adiacente.
        /// </summary>
        [Fact]
        public void Test_Validator_ShouldRejectAdjacentCrowns()
        {
            var board = CreateInitializedBoard(8);
            var validator = new Validator(1);
            board.SetCellState(3, 3, CellState.Crown);

            Assert.False(validator.IsMoveValid(board, 3, 4), "Trebuie blocat pe orizontala.");
        }

        /// <summary>
        /// Verifica daca toti cei 8 vecini ai unei coroane sunt blocati pentru mutari noi.
        /// </summary>
        [Theory]
        [InlineData(2, 2)] [InlineData(2, 3)] [InlineData(2, 4)]
        [InlineData(3, 2)] [InlineData(3, 4)]
        [InlineData(4, 2)] [InlineData(4, 3)] [InlineData(4, 4)]
        public void Test_Validator_ShouldRejectAllEightNeighbors(int nr, int nc)
        {
            var board = CreateInitializedBoard(8);
            var validator = new Validator(1);
            board.SetCellState(3, 3, CellState.Crown);

            Assert.False(validator.IsMoveValid(board, nr, nc), $"Vecinul ({nr},{nc}) trebuie blocat.");
        }

        /// <summary>
        /// Verifica daca plasarea unei coroane la o distanta sigura este permisa.
        /// </summary>
        [Fact]
        public void Test_Validator_ShouldAllowNonAdjacentCrown()
        {
            var board = CreateMultiRegionBoard(8);
            var validator = new Validator(1);
            board.SetCellState(0, 0, CellState.Crown);

            Assert.True(validator.IsMoveValid(board, 2, 2), "Celula la distanta > 1 trebuie sa fie valida.");
        }


        /// <summary>
        /// Testeaza respingerea unei a doua coroane pe acelasi rand cand limita este 1.
        /// </summary>
        [Fact]
        public void Test_Validator_ShouldRejectSecondCrownOnSameRow_WhenLimitIsOne()
        {
            var board = CreateMultiRegionBoard(8);
            var validator = new Validator(1);
            board.SetCellState(0, 0, CellState.Crown);

            Assert.False(validator.IsMoveValid(board, 0, 5), "A doua coroana pe rand trebuie blocata cand limita e 1.");
        }

        /// <summary>
        /// Testeaza respingerea unei a doua coroane pe aceeasi coloana cand limita este 1.
        /// </summary>
        [Fact]
        public void Test_Validator_ShouldRejectSecondCrownOnSameCol_WhenLimitIsOne()
        {
            var board = CreateMultiRegionBoard(8);
            var validator = new Validator(1);
            board.SetCellState(0, 0, CellState.Crown);

            Assert.False(validator.IsMoveValid(board, 5, 0), "A doua coroana pe coloana trebuie blocata cand limita e 1.");
        }

        /// <summary>
        /// Verifica daca validatorul permite corect 2 coroane pe rand cand strategia o cere.
        /// </summary>
        [Fact]
        public void Test_Validator_ShouldAllowSecondCrownOnSameRow_WhenLimitIsTwo()
        {
            var board = new Board(10, 2);
            for (int r = 0; r < 10; r++)
                for (int c = 0; c < 10; c++)
                    board.InitializeCell(r, c, r);

            var validator = new Validator(2);
            board.SetCellState(0, 0, CellState.Crown);

            Assert.True(validator.IsMoveValid(board, 0, 5), "A doua coroana trebuie permisa cand limita e 2.");
        }


        /// <summary>
        /// Verifica daca validatorul blocheaza plasarea in regiuni care au atins deja cota de coroane.
        /// </summary>
        [Fact]
        public void Test_Validator_ShouldRejectCrownInFullRegion()
        {
            var board = CreateInitializedBoard(8); // Toate celulele sunt regiunea 0
            var validator = new Validator(1);
            board.SetCellState(0, 0, CellState.Crown);

            Assert.False(validator.IsMoveValid(board, 5, 5), "Regiunea 0 este plina.");
        }

        /// <summary>
        /// Asigura ca nu se poate suprascrie o coroana existenta.
        /// </summary>
        [Fact]
        public void Test_Validator_ShouldRejectPlacingOnExistingCrown()
        {
            var board = CreateInitializedBoard(8);
            var validator = new Validator(1);
            board.SetCellState(4, 4, CellState.Crown);

            Assert.False(validator.IsMoveValid(board, 4, 4), "Nu se poate plasa pe o coroana deja existenta.");
        }


        /// <summary>
        /// Verifica validitatea plasarii in colturi pe o tabla goala.
        /// </summary>
        [Fact]
        public void Test_Validator_CornerCell_ShouldBeValidOnEmptyBoard()
        {
            var board = CreateMultiRegionBoard(8);
            var validator = new Validator(1);

            Assert.True(validator.IsMoveValid(board, 0, 0));
            Assert.True(validator.IsMoveValid(board, 7, 7));
        }

        /// <summary>
        /// Metoda utilitara pentru a crea rapid o strategie de testare.
        /// </summary>
        private IGameStrategy CreateStrategy(int size, int k) => new InlineStrategy(size, k);

        /// <summary>
        /// Implementare privata a IGameStrategy pentru utilizare exclusiva in interiorul testelor.
        /// </summary>
        private class InlineStrategy : IGameStrategy
        {
            private readonly int _size, _k;
            public InlineStrategy(int size, int k) { _size = size; _k = k; }
            public int GetBoardSize()       => _size;
            public int GetRequiredCrowns()  => _k;
            public string GetDifficultyName() => "Test";
        }
    }
}