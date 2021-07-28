using Newtonsoft.Json;

namespace Configs
{
    public class SettingsProject
    {
        public float StartM { private set; get; }
        public int StartN { private set; get; }
        public float ChangeMOnNextLevel { private set; get; }
        public int ChangeNOnNextLevel { private set; get; }
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
