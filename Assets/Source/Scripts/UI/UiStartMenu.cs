using Internals;

namespace UI
{
    public class UiStartMenu : UiCanvas
    {
        public void StartGame() => Locator.GameplayController.StartGame();
    }
}
