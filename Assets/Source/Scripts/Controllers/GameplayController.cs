using UnityEngine;
using Configs;
using Newtonsoft.Json;
using System.IO;
using System;
using Internals;

namespace Controllers
{
    public class GameplayController : MonoBehaviour
    {
        [SerializeField] private GameWindow _gameWindow;

        private SettingsProject _settingsProject = null;
        private int             _currentLevel    = 0;
        private float           _timerLevel      = 0;
        private bool            _isGame          = false;
        private int             _currentScore    = 0;

        public bool IsGame => _isGame;

        public event Action OnStartGame;

        private void Start()
        {
            var path = Path.Combine(Application.persistentDataPath, "settings.json");
            if (File.Exists(path))
            {
                _settingsProject = JsonConvert.DeserializeObject<SettingsProject>(File.ReadAllText(path));
            }
            else
            {
                _settingsProject = new SettingsProject();
                File.WriteAllText(path, JsonConvert.SerializeObject(_settingsProject));
            }

            _gameWindow.OnWallsConnected += NextLevel;
        }

        private void Update()
        {
            if (_isGame)
            {
                if (_timerLevel <= 0)
                {
                    FinishGame();
                }
                _timerLevel -= Time.deltaTime;
                Locator.UiSwitcher.UiGameMenu.SetTimer(_timerLevel);
            }
        }

        public void NextLevel()
        {
            _currentLevel++;
            _currentScore += _settingsProject.AddScoreOnNextLevel;
            Locator.UiSwitcher.UiGameMenu.SetLevel(_currentLevel);
            _gameWindow.ClearWindow();
            _gameWindow.CreateWalls(_settingsProject.StartN + _currentLevel * _settingsProject.ChangeNOnNextLevel);
            _timerLevel = _settingsProject.StartM - _currentLevel * _settingsProject.ChangeMOnNextLevel;
        }


        public void StartGame()
        {
            Locator.UiSwitcher.GameMenu();
            _currentLevel = 1;
            _currentScore = 0;
            Locator.UiSwitcher.UiGameMenu.SetLevel(_currentLevel);
            _gameWindow.ClearWindow();
            _gameWindow.CreateWalls(_settingsProject.StartN + _currentLevel * _settingsProject.ChangeNOnNextLevel);
            _timerLevel = _settingsProject.StartM - _currentLevel * _settingsProject.ChangeMOnNextLevel;
            _isGame = true;
            OnStartGame?.Invoke();
        }

        public void FinishGame()
        {
            _gameWindow.ClearWindow();
            Locator.UiSwitcher.FinishGame(_currentScore);
            _isGame = false;
        }
    }
}
