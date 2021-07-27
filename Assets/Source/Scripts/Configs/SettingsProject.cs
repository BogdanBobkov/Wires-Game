using Newtonsoft.Json;

namespace Configs
{
    public class SettingsProject
    {
        [JsonProperty("StartM")]
        public float StartM { private set; get; }
        [JsonProperty("StartN")]
        public int StartN { private set; get; }
        [JsonProperty("ÑhangeMOnNextLevel")]
        public float ChangeMOnNextLevel { private set; get; }
        [JsonProperty("ÑhangeNOnNextLevel")]
        public int ChangeNOnNextLevel { private set; get; }
        [JsonProperty("AddScoreOnNextLevel")]
        public int AddScoreOnNextLevel { private set; get; }

        public SettingsProject()
        {
            StartM              = 3;
            StartN              = 2;
            ChangeMOnNextLevel  = 0.5f;
            ChangeNOnNextLevel  = 2;
            AddScoreOnNextLevel = 50;
        }
    }
}
