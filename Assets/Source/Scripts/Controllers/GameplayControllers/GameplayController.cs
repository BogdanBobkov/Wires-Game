using UnityEngine;
using Configs;
using Newtonsoft.Json;
using System.IO;
using System;
using Internals;
using Others;
using UI;

namespace Controllers
{
    public class GameplayController : MonoBehaviour, IGameplayControllable
    {
        private IGameView       _gameWindow;
        private IRealtimeWidget _realtimeWidget;

        private SettingsProject _settingsProject = null;
        private int             _currentLevel    = 0;
        private float           _timerLevel      = 0;
        private bool            _isGame          = false;
        private int             _currentScore    = 0;

        public int CurrentScore => _currentScore;

        public event Action OnStartGame;

        private void Awake() => Register();
        private void OnDestroy() => Unregister();

        private void Start()
        {
            var path = Path.Combine(Application.persistentDataPath, PublicConst.SettingsFile);
            if (File.Exists(path))
            {
                _settingsProject = JsonConvert.DeserializeObject<SettingsProject>(File.ReadAllText(path));
            }
            else
            {
                _settingsProject = new SettingsProject();
                File.WriteAllText(path, JsonConvert.SerializeObject(_settingsProject));
            }
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
                _realtimeWidget.SetTimer(_timerLevel);
            }
        }

        #region Interfaces
        public void NextLevel()
        {
            _currentLevel++;
            _currentScore += _settingsProject.AddScoreOnNextLevel;
            _realtimeWidget.SetLevel(_currentLevel);
            _gameWindow.ClearWindow();
            _gameWindow.CreateWalls(_settingsProject.StartN + _currentLevel * _settingsProject.ChangeNOnNextLevel);
            _timerLevel = _settingsProject.StartM - _currentLevel * _settingsProject.ChangeMOnNextLevel;
        }


        public void StartGame()
        {
            _gameWindow     = Locator.GetObject<IGameView>();
            _realtimeWidget = Locator.GetObject<IRealtimeWidget>();
            _gameWindow.OnWallsConnected += NextLevel;
            _realtimeWidget.Show();
            _currentLevel = 1;
            _currentScore = 0;
            _realtimeWidget.SetLevel(_currentLevel);
            _gameWindow.CreateWalls(_settingsProject.StartN + _currentLevel * _settingsProject.ChangeNOnNextLevel);
            _timerLevel = _settingsProject.StartM - _currentLevel * _settingsProject.ChangeMOnNextLevel;
            _isGame = true;
            OnStartGame?.Invoke();
        }

        public void FinishGame()
        {
            _gameWindow.OnWallsConnected -= NextLevel;
            _gameWindow.ClearWindow();
            Locator.GetObject<IFinishWidget>().Show();
            _isGame = false;
        }

        public bool IsGame() => _isGame;

        public void Register() => Locator.Register(typeof(IGameplayControllable), this);
        public void Unregister() => Locator.Unregister(typeof(IGameplayControllable));
        #endregion
    }
}
