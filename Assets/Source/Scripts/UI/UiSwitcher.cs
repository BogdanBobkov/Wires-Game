using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UiSwitcher : MonoBehaviour
    {
        public UiGameMenu UiGameMenu;
        public UiGameOver UiGameOver;
        public UiLeaderboard UiLeaderBoard;
        public UiStartMenu UiStartMenu;

        private void Start()
        {
            UiStartMenu.Show();
            UiGameMenu.Hide();
            UiLeaderBoard.Hide();
            UiGameOver.Hide();
        }

        public void FinishGame(int score)
        {
            UiStartMenu.Hide();
            UiGameMenu.Hide();
            UiLeaderBoard.Hide();
            UiGameOver.Show(score);
        }

        public void GameMenu()
        {
            UiStartMenu.Hide();
            UiGameMenu.Show();
            UiLeaderBoard.Hide();
            UiGameOver.Hide();
        }

        public void SetLeaderBoard(int score, string name)
        {
            UiStartMenu.Hide();
            UiGameMenu.Hide();
            UiLeaderBoard.Show(score, name);
            UiGameOver.Hide();
        }
    }
}