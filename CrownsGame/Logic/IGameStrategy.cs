namespace CrownsGame.Logic
{
    /// <summary>
    ///  Interfata pentru nivelurile de dificultate ale jocului: Easy, Medium, Hard.
    ///  Design pattern folosit: Strategy
    /// </summary>
    public interface IGameStrategy
    {
        /// <summary>
        /// Determina numarul de coroane necesare pentru configurația curenta.
        /// </summary>
        int GetRequiredCrowns();

        /// <summary>
        /// Determina dimensiunea gridului: 8 - Easy, 10 - Medium, 14 - Hard.
        /// </summary>
        int GetBoardSize();
    }
    
}