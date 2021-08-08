using Internals;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using Others;
using System;
using Controllers;

namespace UI
{
    public class UiLeaderboard : WidgetBase, ILeaderboardWidget
    {

        [SerializeField] private GameObject _PrefabContent;

        private List<GameObject> _Content = new List<GameObject>();

        private LeaderBoard _leaderBoard;

        private int _maxPlayersInLeaderboard = 10;

        private string PathFile;

        private void Awake() => Register();
        private void OnDestroy() => Unregister();

        #region Interfaces
        public override void Register() => Locator.Register(typeof(ILeaderboardWidget), this);
        public override void Unregister() => Locator.Unregister(typeof(ILeaderboardWidget));
        #endregion

        public void SetLeaderboard(int score, string name)
        {
            PathFile = Path.Combine(Application.temporaryCachePath, PublicConst.LeaderboardFile);

            if (_leaderBoard == null)
            {
                if (File.Exists(PathFile))
                {
                    _leaderBoard = JsonConvert.DeserializeObject<LeaderBoard>(File.ReadAllText(PathFile));
                }
                else
                {
                    _leaderBoard = new LeaderBoard();
                    File.WriteAllText(PathFile, JsonConvert.SerializeObject(_leaderBoard));
                }
            }

            var player = new LeaderBoard.Player(score, name);
            TryInsertPlayerToLeaderBoard(player);
            CreateLeaderboardGOs();

            File.WriteAllText(PathFile, JsonConvert.SerializeObject(_leaderBoard));
        }

        private void TryInsertPlayerToLeaderBoard(LeaderBoard.Player player)
        {
            int oldCount = _leaderBoard.bestPlayers.Count;

            for(int i = 0; i < oldCount; i++)
            {
                if (_leaderBoard.bestPlayers[i].score <= player.score)
                {
                    _leaderBoard.bestPlayers.Insert(i, player);
                    break;
                }
            }

            int newCount = _leaderBoard.bestPlayers.Count;
            if (newCount > _maxPlayersInLeaderboard)
            {
                _leaderBoard.bestPlayers.RemoveAt(_maxPlayersInLeaderboard);
            }
            else if(newCount < _maxPlayersInLeaderboard && oldCount == newCount)
            {
                _leaderBoard.bestPlayers.Insert(newCount, player);
            }
        }

        private void CreateLeaderboardGOs()
        {
            foreach(var player in _leaderBoard.bestPlayers)
            {
                GameObject playerGO = Instantiate(_PrefabContent, _PrefabContent.transform.parent);
                playerGO.GetComponent<TextMeshProUGUI>().text = $"{player.name} (Score: {player.score})";
                playerGO.SetActive(true);
                _Content.Add(playerGO);
            }
        }

        public void ReplayGame() => Hide(() => Locator.GetObject<IGameplayControllable>().StartGame());

        public override void Hide(Action afterShow)
        {
            foreach(var element in _Content)
            {
                Destroy(element);
            }
            _Content.Clear();

            base.Hide(afterShow);
        }
    }
}
