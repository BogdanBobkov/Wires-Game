using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Internals;
using UnityEngine.EventSystems;

namespace UI
{
    public class RealtimeWidget : WidgetBase, IRealtimeWidget
    {
        [SerializeField] private TextMeshProUGUI _LevelText;
        [SerializeField] private TextMeshProUGUI _TimerText;

        private EventTrigger trigger;

        private void Awake() => Register();
        private void OnDestroy() => Unregister();

        #region Interfaces
        public override void Register() => Locator.Register(typeof(IRealtimeWidget), this);
        public override void Unregister() => Locator.Unregister(typeof(IRealtimeWidget));
        public void SetLevel(int level) => _LevelText.text = $"Level {level}";
        public void SetTimer(float timer) => _TimerText.text = $"Seconds left {(int)timer}";
        #endregion
    }
}
