﻿using System;
using ProjectAssets.Scripts.Gameplay;
using ProjectAssets.Scripts.Gameplay.Difficulty_Adjustment;
using ProjectAssets.Scripts.Util;
using ProjectAssets.SFX;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


namespace UnityTemplateProjects.UI
{
    /// <summary>
    /// This class handles every UI in the game.
    /// Responsible for updating the UI elements.
    /// </summary>
    public class UIManager : Singleton<UIManager>
    {
        // handles UI Objects in the scene


        public GameObject createProfileButton;
        public GameObject mainMenuGroup;
        public GameObject pauseMenuGroup;
        public GameObject inGameUIGroup;
        public GameObject postLevelMenuGroup;
        public GameObject postFailedLevelMenuGroup;
        
        //UI from Main Menu
        public GameObject settingsMenuGroup;
        public GameObject menuCanvas;

        public TMP_Text moves;
        public TMP_Text keys;
        public TMP_Text time;
        public TMP_Text rating;
        public TMP_Text games;
        public TMP_Text wins;
        public TMP_Text currentRating;
        public TMP_Text levelRating;
        public Slider timerBar;
        public Image timerBarFill; 
        
        public Gradient timerBarGradient;

        public int expectedMoves;


        private void LateUpdate()
        {
            // Test Key
            if (Input.GetKeyUp(KeyCode.Escape) && mainMenuGroup.activeSelf == false)
            {
                if(postLevelMenuGroup.activeSelf == false && postFailedLevelMenuGroup.activeSelf == false)
                    PauseUnpause();// disable when playing
            }


        }

        public void PauseUnpause()
        {
            ShowHidePauseMenu(); // disable when playing
            ShowHideinGameUIGroup();
        }

        public void ShowHideSettingsGroup() //settings button (from main menu)
        {
            settingsMenuGroup.SetActive(!settingsMenuGroup.activeSelf);
            menuCanvas.SetActive(!menuCanvas.activeSelf);
        }
        
        

        public void ChangeRatingText(double playerRating)
        {
            rating.text = $"Rating: {(float)playerRating}";
        }

        public void ChangeCurrentRatingText(double playerRating)
        {
            currentRating.text = "Current Rating: " + (float)playerRating;
        }
        public void ChangeLevelRatingText(double lRating)
        {
            levelRating.text = "Level Rating: " + (float)lRating;
        }
        public void ChangeWinText(double win)
        {
            wins.text = win > 1 ? $"wins: {win}" : $"win: {win}";
        }
        public void ChangeGamesText(double gamesPlayed)
        {
            games.text = gamesPlayed > 1 ? $"games played: {gamesPlayed}" : $"game played: {gamesPlayed}";
        }

        

        public void ChangeMoveText(float pm = 0)
        {
            // moves.text = $"{expectedMoves}/{pm}";
            var subtractedExpectedMoves = expectedMoves - 1;
            var percentage = (pm / subtractedExpectedMoves) * 100;
           // moves.text = pm == 0 ? $" Total Tiles: {Mathf.Floor(pm)} \n Explored: {100f}% / {Mathf.Floor(percentage)}% " : $"Total Tiles: {Mathf.Floor(pm)} \n Explored: {100f}% / {Mathf.Floor(percentage)}%";
            moves.text = $" Total Tiles \n Explored: {pm} ";

            
            timerBar.value = pm;

            ChangeFillColorBasedOnProgress( pm);
        }
        public void ChangeKeyText(int key)
        {
            keys.text = key > 1 ? $"keys collected: {key}" : $"key collected: {key}";
        }

        public void ChangeTimeText(double remainingTime)
        {
            time.text = $"{(int)remainingTime}";
        }

        public void Resume()
        {
            ShowHidePauseMenu(); // disable when playing
            ShowHideinGameUIGroup();
        }

        public void ShowHidePauseMenu()
        {
            pauseMenuGroup.SetActive(!pauseMenuGroup.activeSelf);
            if (pauseMenuGroup.activeSelf)
            {
                GameManager.Instance.playerMovement.enabled = false;
            }
            else
            {
                GameManager.Instance.playerMovement.enabled = true;
            }
        }
        public void ShowHideMainMenuGroup()
        {
            mainMenuGroup.SetActive(!mainMenuGroup.activeSelf);
            SoundManager.Instance.PlayMenuMusic();
            
        }
        public void ShowHidePostLevelGroup()
        {
            postLevelMenuGroup.SetActive(!postLevelMenuGroup.activeSelf);
        }
        public void ShowHidePostFailedLevelGroup()
        {
            postFailedLevelMenuGroup.SetActive(!postFailedLevelMenuGroup.activeSelf);
        }
        public void ShowHideinGameUIGroup()
        {
            inGameUIGroup.SetActive(!inGameUIGroup.activeSelf);
            // Pause 

            
            ChangeCurrentRatingText(SaveManager.Instance.playerProfile.currentRating);
            ChangeLevelRatingText(GameManager.Instance.modifier.levelGenerated.levelRating);

            ChangeRatingText(SaveManager.Instance.playerProfile.currentRating);
            ChangeGamesText(SaveManager.Instance.playerProfile.gamesPlayed);
            ChangeWinText(SaveManager.Instance.playerProfile.gamesWon);
        }
        public void ShowHideMainMenuGroupFromButton()
        {
            for (int i = 0; i < GameManager.Instance.cellGameObjects.Count; i++)
            {
                Destroy(GameManager.Instance.cellGameObjects[i]);
            }
            GameManager.Instance.cellGameObjects.Clear();
            GameManager.Instance.RemoveObjects();

            ShowHidePostLevelGroup();
            mainMenuGroup.SetActive(!mainMenuGroup.activeSelf);
            SoundManager.Instance.PlayMenuMusic();

        }
        public void ShowHideMainMenuGroupFromFailedButton()
        {
            for (int i = 0; i < GameManager.Instance.cellGameObjects.Count; i++)
            {
                Destroy(GameManager.Instance.cellGameObjects[i]);
            }
            GameManager.Instance.cellGameObjects.Clear();
            GameManager.Instance.RemoveObjects();

            ShowHidePostFailedLevelGroup();
            mainMenuGroup.SetActive(!mainMenuGroup.activeSelf);
            SoundManager.Instance.PlayMenuMusic();

        }

        public void ChangeFillColorBasedOnProgress(float playerMoves)
        {
            var currentMove = playerMoves;
            float gradientMap =Mathf.InverseLerp(timerBar.minValue,timerBar.maxValue, currentMove);
            timerBarFill.color = timerBarGradient.Evaluate(gradientMap);
        }
        
        public void ShowHideMainMenuGroupFromPause()
        {
            for (int i = 0; i < GameManager.Instance.cellGameObjects.Count; i++)
            {
                Destroy(GameManager.Instance.cellGameObjects[i]);
            }
            GameManager.Instance.cellGameObjects.Clear();
            GameManager.Instance.RemoveObjects();
            ShowHidePauseMenu();
            mainMenuGroup.SetActive(!mainMenuGroup.activeSelf);
            SoundManager.Instance.PlayMenuMusic();

        }
    }
}