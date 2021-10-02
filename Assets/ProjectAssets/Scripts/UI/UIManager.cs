using System;
using ProjectAssets.Scripts.Util;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UnityTemplateProjects.UI
{
    public class UIManager : Singleton<UIManager>
    {
        // handles UI Objects in the scene


        public GameObject createProfileButton;
        public GameObject mainMenuGroup;
        public GameObject pauseMenuGroup;
        public GameObject inGameUIGroup;
        public GameObject postLevelMenuGroup;

        public TMP_Text moves;
        public TMP_Text keys;
        public TMP_Text time;
        public TMP_Text rating;

        public int expectedMoves;


        private void LateUpdate()
        {
            // Test Key
            if (Input.GetKeyUp(KeyCode.Escape) && mainMenuGroup.activeSelf == false && postLevelMenuGroup.activeSelf == false)
            {
                ShowHidePauseMenu(); // disable when playing
            }
        }

        public void ChangeRatingText(double playerRating)
        {
            rating.text = $"Rating: {playerRating}";
        }

        public void ChangeMoveText(int pm = 0)
        {
            moves.text = $"{expectedMoves}/{pm}";
        }
        public void ChangeKeyText(int key)
        {
            keys.text = key > 1 ? $"keys remaining: {key}" : $"key remaining: {key}";
        }

        public void ChangeTimeText(double remainingTime)
        {
            time.text = $"{remainingTime}";
        }

        public void ShowHidePauseMenu()
        {
            pauseMenuGroup.SetActive(!pauseMenuGroup.activeSelf);
        }
        public void ShowHideMainMenuGroup()
        {
            mainMenuGroup.SetActive(!mainMenuGroup.activeSelf);
        }
        public void ShowHidePostLevelGroup()
        {
            postLevelMenuGroup.SetActive(!postLevelMenuGroup.activeSelf);
        }
        public void ShowHideinGameUIGroup()
        {
            inGameUIGroup.SetActive(!inGameUIGroup.activeSelf);
        }
    }
}