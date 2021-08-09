using System.IO;
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
        private string path = "/save/profile.json";
        private PlayerProfile _playerProfile;
        
        // Load if one exist
        public void TryLoadProfile()
        {
            var filePath = Application.dataPath + path;
            var exists = File.Exists(Application.dataPath + path);
            Debug.Log(exists ? "Profile Found" : "create a Profile?");
            if (!exists)
            {
                File.Create(Application.dataPath + @"\save\profile.json");
                UIManager.Instance.createProfileButton.SetActive(true);
                // Prompt Profile Creation text/ button

            }
            else
            {
                _playerProfile = new PlayerProfile();
            var file = File.ReadAllText(filePath);
            Debug.Log(file.Length);
            Debug.Log($"Loaded Profile of: {_playerProfile.userName}");

            _playerProfile =  JsonUtility.FromJson<PlayerProfile>(file);
            Debug.Log($"Loaded Profile of: {_playerProfile.userName}");
            }
        }

        // prompted when there is no profile available
        public void CreateProfile()
        {
            //create a new file Test
            _playerProfile = new PlayerProfile(); // create blank template
            var jsonString = JsonUtility.ToJson(_playerProfile);
            File.WriteAllText(Application.dataPath + path,jsonString);


        }

        public void SaveProfile()
        {
            //comment
        }
        
        
        
        
    }
}