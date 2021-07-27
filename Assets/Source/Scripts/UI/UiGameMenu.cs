using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UiGameMenu : UiCanvas
    {
        [SerializeField] private TextMeshProUGUI _LevelText;
        [SerializeField] private TextMeshProUGUI _TimerText;

        public void SetLevel(int level) => _LevelText.text = $"Level {level}";
        public void SetTimer(float timer) => _TimerText.text = $"Seconds left {(int)timer}";
    }
}
