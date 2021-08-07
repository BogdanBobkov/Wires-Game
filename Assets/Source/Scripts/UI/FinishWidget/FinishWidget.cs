using Internals;
using TMPro;
using UnityEngine;

namespace UI
{
    public class FinishWidget : WidgetBase, IFinishWidget
    {
        [SerializeField] private TextMeshProUGUI _CongratsText;
        [SerializeField] private TMP_InputField _InputName;

        private int _score;

        private void Awake() => Register();
        private void OnDestroy() => Unregister();

        #region Interfaces
        public override void Register() => Locator.Register(typeof(IFinishWidget), this);
        public override void Unregister() => Locator.Unregister(typeof(IFinishWidget));
        #endregion

        public void SetInformationTable(int score)
        {
            _score = score;
            _CongratsText.text = $"You got {_score} scores! Enter your name!";
            this.gameObject.SetActive(true);
        }

        public void EnterName()
        {
            var leaderboardUI = Locator.GetObject<ILeaderboardWidget>();
            leaderboardUI.SetLeaderboard(_score, _InputName.text);
            leaderboardUI.Show();

            base.Hide();
        }
    }
}
