namespace UI
{
    public interface ILeaderboardWidget : IWidget
    {
        void SetLeaderboard(int score, string name);
    }
}
