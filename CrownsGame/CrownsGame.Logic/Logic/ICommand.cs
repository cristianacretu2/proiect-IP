namespace CrownsGame.Logic
{
    /// <summary>
    /// Interfata de bazt pentru obiectele de tip comandt.
    /// Defineste metodele necesare pentru executie si anulare (Undo).
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Executa actiunea specifica comenzii.
        /// </summary>
        void Execute();

        /// <summary>
        /// Inverseaza actiunea executata anterior.
        /// </summary>
        void Undo();
    }
}