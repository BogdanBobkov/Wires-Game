using UnityEngine;
using UI;
using Controllers;

namespace Internals
{
    public class Locator : MonoBehaviour
    {
        private static Locator _instance;

        [SerializeField] private UiSwitcher _uiSwitcher;
        public static UiSwitcher UiSwitcher => _instance._uiSwitcher;

        [SerializeField] private GameplayController _gameplayController;
        public static GameplayController GameplayController => _instance._gameplayController;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(this);
                return;
            }
            _instance = this;
        }
    }
}