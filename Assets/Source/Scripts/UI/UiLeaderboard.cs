using Internals;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UiLeaderboard : UiCanvas
    {
        public class LeaderBoard
        {
            public class Player
            {
                public Player(int score, string name)
                {
                    this.name = name;
                    this.score = score;
                }

                [JsonProperty("name")]
                public string name;
                [JsonProperty("score")]
                public int    score;
            }

            [JsonProperty("bestPlayers")]
            public List<Player> bestPlayers = new List<Player>();
        }

        private readonly string FileName = "leaderboard.json";

        [SerializeField] private GameObject _PrefabContent;

        private List<GameObject> _Content = new List<GameObject>();

        private LeaderBoard _leaderBoard;

        private int _maxPlayersInLeaderboard = 10;

        public void Show(int score, string name)
        {
            if(_leaderBoard == null)
            {
                var path = Path.Combine(Application.persistentDataPath, FileName);
                if (File.Exists(path))
                {
                    _leaderBoard = JsonConvert.DeserializeObject<LeaderBoard>(File.ReadAllText(path));
                }
                else
                {
                    _leaderBoard = new LeaderBoard();
                    File.WriteAllText(path, JsonConvert.SerializeObject(_leaderBoard));
                }
            }

            var player = new LeaderBoard.Player(score, name);
            TryInsertPlayerToLeaderBoard(player);

            gameObject.SetActive(true);
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

            CreateLeaderboard();
        }

        private void CreateLeaderboard()
        {
            foreach(var player in _leaderBoard.bestPlayers)
            {
                GameObject playerGO = Instantiate(_PrefabContent, _PrefabContent.transform.parent);
                playerGO.GetComponent<TextMeshProUGUI>().text = $"{player.name} (Score: {player.score})";
                playerGO.SetActive(true);
                _Content.Add(playerGO);
            }
        }

        public void ReplayGame() => Locator.GameplayController.StartGame();

        public new void Hide()
        {
            if (_leaderBoard != null)
            {
                File.WriteAllText(Path.Combine(Application.persistentDataPath, FileName), JsonConvert.SerializeObject(_leaderBoard));
            }

            foreach(var element in _Content)
            {
                Destroy(element);
            }
            _Content.Clear();

            this.gameObject.SetActive(false);
        }
    }
}
