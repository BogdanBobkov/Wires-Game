using Newtonsoft.Json;
using System.Collections.Generic;

namespace Internals
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

            [JsonProperty("name")] public string name;
            [JsonProperty("score")] public int score;
        }

        [JsonProperty("bestPlayers")] public List<Player> bestPlayers = new List<Player>();
    }

    public class Player
    {

    }
}
