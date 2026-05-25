/*
 * Scopul fisierului: Defineste interfata contractuala pentru implementarea sablonului Command, 
 * asigurand suportul pentru operatii reversibile (Undo/Redo).
 * Autor: 
 */

namespace CrownsGame.Logic
{
    /// <summary>
    /// Interfata de baza pentru obiectele de tip comanda.
    /// Defineste metodele necesare pentru executie si anulare (Undo), facilitand gestionarea istoricului de mutari.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Executa actiunea specifica comenzii pe starea curenta a jocului.
        /// </summary>
        void Execute();

        /// <summary>
        /// Inverseaza actiunea executata anterior, restabilind starea precedenta a obiectelor afectate.
        /// </summary>
        void Undo();
    }
}