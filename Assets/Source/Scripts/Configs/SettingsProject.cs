using Newtonsoft.Json;

namespace Configs
{
    public class SettingsProject
    {
        [JsonProperty("StartM")]              public float StartM { private set; get; }
        [JsonProperty("StartN")]              public int StartN { private set; get; }
        [JsonProperty("ChangeMOnNextLevel")]  public float ChangeMOnNextLevel { private set; get; }
        [JsonProperty("ChangeNOnNextLevel")]  public int ChangeNOnNextLevel { private set; get; }
        [JsonProperty("AddScoreOnNextLevel")] public int AddScoreOnNextLevel { private set; get; }

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
