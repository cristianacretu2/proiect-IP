namespace CrownsGame.Logic
{
    public class EasyStrategy : IGameStrategy
    {
        public int GetRequiredCrowns()
        {
            return 1;
        }

        public int GetBoardSize()
        {
            return 8; // Un grid de 8x8 pentru Easy
        }

        public string GetDifficultyName()
        {
            return "Easy";
        }
    }
}