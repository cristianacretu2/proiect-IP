/*
 * Scopul fisierului: Testarea unitara a componentelor logice si a structurii de date Board pentru asigurarea integritatii sistemului.
 * Autor: Radani Antonia
 */
using Xunit;
using CrownsGame.Core;
using CrownsGame.Logic;

namespace CrownsGame.Tests
{
    /// <summary>
    /// Clasa care contine suita de teste unitare pentru verificarea corectitudinii operatiilor pe tabla de joc si a regulilor de validare.
    /// </summary>
    public class GameLogicTests
    {
        /// <summary>
        /// Metoda de tip helper utilizata pentru a genera o instanta de Board gata de utilizare, 
        /// prevenind erorile de tip NullReference prin initializarea tuturor celulelor.
        /// </summary>
        /// <param name="size">Dimensiunea laturii tablei patrate.</param>
        /// <param name="crowns">Numarul de coroane necesare per grup.</param>
        /// <returns>O instanta de Board cu toate celulele setate pe aceeasi regiune implicita.</returns>
        private Board CreateInitializedBoard(int size, int crowns = 1)
        {
            var board = new Board(size, crowns);
            for (int r = 0; r < size; r++)
            {
                for (int c = 0; c < size; c++)
                {
                    // Fiecare celula este legata de o regiune cu ID-ul 0 pentru simplitatea testarii.
                    board.InitializeCell(r, c, 0);
                }
            }
            return board;
        }


        /// <summary>
        /// Verifica daca proprietatile de baza ale tablei (dimensiune si coroane) sunt setate corect prin constructor.
        /// </summary>
        [Fact]
        public void Test_BoardInitialization_PropertiesShouldMatch()
        {
            // Arrange & Act: Se creeaza o tabla de 8x8 cu 1 coroana per grup.
            var board = new Board(8, 1);

            // Assert: Se verifica corespondenta valorilor stocate.
            Assert.Equal(8, board.Size);
            Assert.Equal(1, board.CrownsPerGroup);
        }

        /// <summary>
        /// Testeaza functionalitatea de modificare a starii unei celule, asigurandu-se ca datele sunt persistate in grila.
        /// </summary>
        [Fact]
        public void Test_SetCellState_ShouldUpdateStateCorrectly()
        {
            // Arrange: Initializam o tabla de test.
            var board = CreateInitializedBoard(8);
            
            // Act: Plasasăm o coroană la coordonatele specificate.
            board.SetCellState(2, 2, CellState.Crown);

            // Assert: Verificam daca starea celulei s-a schimbat conform comenzii.
            Assert.Equal(CellState.Crown, board.GetCell(2, 2).State);
        }

        /// <summary>
        /// Verifica mecanismul de clonare (Deep Copy) pentru a asigura izolarea datelor intre instanta originala si cea clonata.
        /// </summary>
        [Fact]
        public void Test_Clone_ShouldCreateDeepCopy()
        {
            // Arrange: Pregatim o tabla cu o coroana plasata intr-un colt.
            var original = CreateInitializedBoard(5);
            original.SetCellState(0, 0, CellState.Crown);

            // Act: Cream o copie independenta.
            var clone = original.Clone();

            // Assert: Verificam izolarea memoriei prin modificarea originalului si controlul clonei.
            Assert.NotSame(original, clone); // Sunt obiecte diferite în memorie
            Assert.Equal(original.GetCell(0, 0).State, clone.GetCell(0, 0).State);
            
            // Modificam originalul; dca este deep copy, clona trebuie sa ramana neschimbata.
            original.SetCellState(0, 0, CellState.Empty);
            Assert.Equal(CellState.Crown, clone.GetCell(0, 0).State);
        }


        /// <summary>
        /// Testeaza regula de non-proximitate: Validatorul trebuie sa respinga plasarea unei coroane langa alta deja existenta.
        /// </summary>
        [Fact]
        public void Test_Validator_ShouldRejectAdjacentCrowns()
        {
            // Arrange: Plasăm o coroană "ancoră" pe tabla.
            var board = CreateInitializedBoard(8);
            var validator = new Validator(1);
            board.SetCellState(3, 3, CellState.Crown);

            // Act: Incercam sa plasam o alta coroana imediat langa ea (pe orizontala).
            bool isValid = validator.IsMoveValid(board, 3, 4);

            // Assert: Rezultatul trebuie sa fie invalid conform regulamentului.
            Assert.False(isValid, "Validatorul trebuie să blocheze plasarea lângă o altă coroană.");
        }


        /// <summary>
        /// Verifica daca metoda de acces GetCell returneaza corect referinta catre obiectul Cell si nu null.
        /// </summary>
        /// <param name="r">Indexul randului.</param>
        /// <param name="c">Indexul coloanei.</param>
        [Theory]
        [InlineData(0, 0)]
        [InlineData(4, 4)]
        public void Test_GetCell_ShouldReturnInitializedCell(int r, int c)
        {
            // Arrange
            var board = CreateInitializedBoard(5);

            // Act: Accesam celula la coordonatele furnizate de InlineData.
            var cell = board.GetCell(r, c);

            // Assert
            Assert.NotNull(cell);
        }
    }
}