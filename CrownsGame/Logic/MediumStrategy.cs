namespace CrownsGame.Logic
{
    public class MediumStrategy : IGameStrategy
    {
        public int GetRequiredCrowns()
        {
            return 2;
        }

        public int GetBoardSize()
        {
            return 10; // Un grid de 10x10 pentru Medium
        }
    }
}