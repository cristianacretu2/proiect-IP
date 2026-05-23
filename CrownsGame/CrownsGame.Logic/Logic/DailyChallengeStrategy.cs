namespace CrownsGame.Logic
{
    public class DailyChallengeStrategy : IGameStrategy
    {
        public int GetBoardSize() => 5;
        public int GetRequiredCrowns() => 1;
        public string GetDifficultyName() => "Daily Challenge";
    }
}