namespace CrownsGame.Logic
{
    public class HardStrategy : IGameStrategy
    {
        public int GetRequiredCrowns()
        {
            return 3;
        }

        public int GetBoardSize()
        {
            return 14; // Un grid de 14x14 pentru Medium
        }
    }
}