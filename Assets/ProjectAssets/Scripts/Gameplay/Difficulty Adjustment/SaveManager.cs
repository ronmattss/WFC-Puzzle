/*
 Title: Save Manager
 Author: Ron Matthew Rivera
 Sub-System: Save system
 Date Written/Revised: Sept. 22, 2021
 Purpose: Class that handles saving and loading of data on a JSON File
 Data Structures, algorithms, and control: Class. Lists
 */


using System.IO;
using ProjectAssets.Scripts.UI;
using ProjectAssets.Scripts.Util;
using UnityEngine;
using UnityTemplateProjects.UI;

namespace ProjectAssets.Scripts.Gameplay.Difficulty_Adjustment
{

    public class SaveManager : Singleton<SaveManager>
    {
        // pass the profile to the difficulty Manager
        // have one single Instance of the player Profile
        // if non exist create one
        private string path = "/profile.json";
        public PlayerProfile playerProfile;
        
        // Load if one exist
        public void TryLoadProfile()
        {
            var filePath = Application.dataPath + path;
            var exists = File.Exists(Application.dataPath + path);
            Debug.Log(exists ? "Profile Found" : "create a Profile?");
            if (!exists)
            {
              var file =  File.Create(Application.dataPath + @"\profile.json");
                file.Close();
                CreateProfile();
              //  UIManager.Instance.createProfileButton.SetActive(true);
                // Prompt Profile Creation text/ button

            }
            else
            {
                playerProfile = new PlayerProfile();
            var file = File.ReadAllText(filePath);
//            Debug.Log(file.Length);
            Debug.Log($"Loaded Profile of: {playerProfile.userName}");

            playerProfile =  JsonUtility.FromJson<PlayerProfile>(file);
            Debug.Log($"Loaded Profile of: {playerProfile.userName}");
            UIManager.Instance.ChangeRatingText(playerProfile.currentRating);
            UIManager.Instance.ChangeGamesText(playerProfile.gamesPlayed);
            UIManager.Instance.ChangeWinText(playerProfile.gamesWon);


            }
        }

        // prompted when there is no profile available
        public void CreateProfile()
        {
            //create a new file Test
            playerProfile = new PlayerProfile(); // create blank template
            var jsonString = JsonUtility.ToJson(playerProfile);
            File.WriteAllText(Application.dataPath + path,jsonString);
            


        }

        public void SaveProfile()
        {
            var jsonString = JsonUtility.ToJson(playerProfile);
            Debug.Log($"SaveString: {jsonString}");
            File.WriteAllText(Application.dataPath + path,jsonString);
            //comment
        }
        
        
        
        
    }
}