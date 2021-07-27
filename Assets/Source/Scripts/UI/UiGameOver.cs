using Internals;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UiGameOver : UiCanvas
    {
        [SerializeField] private TextMeshProUGUI _CongratsText;
        [SerializeField] private TMP_InputField _InputName;

        private int _score;

        public void Show(int score)
        {
            _score = score;
            _CongratsText.text = $"You got {_score} scores! Enter your name!";
            this.gameObject.SetActive(true);
        }

        public void EnterName() => Locator.UiSwitcher.SetLeaderBoard(_score, _InputName.text);
    }
}
