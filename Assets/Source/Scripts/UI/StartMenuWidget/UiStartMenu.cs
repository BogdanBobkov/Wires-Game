using Internals;
using Controllers;

namespace UI
{
    public class UiStartMenu : WidgetBase, IStartableWidget
    {
        private void Awake() => Register();
        private void OnDestroy() => Unregister();

        #region Interfaces
        public override void Register() => Locator.Register(typeof(IStartableWidget), this);
        public override void Unregister() => Locator.Unregister(typeof(IStartableWidget));
        #endregion

        public void StartGame() => Locator.GetObject<IGameplayControllable>().StartGame();
    }
}
