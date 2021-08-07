using Internals;

namespace Controllers
{
    public interface IGameplayControllable : IRegistrable
    {
        bool IsGame();
        void StartGame();
        void FinishGame();
        void NextLevel();
    }
}
