using System.IO;
using ProjectAssets.Scripts.Util;
using UnityEngine;

namespace ProjectAssets.Scripts.Gameplay.Difficulty_Adjustment
{
    public class DifficultyManager : Singleton<DifficultyManager>
    {
        private PlayerProfile playerProfile = new PlayerProfile();
        private PlayerProfile testProfile = new PlayerProfile("userTest",25,25,5,5);

        private LevelDetails currentLevel = new LevelDetails();

        // create a Button to test this
       public void TestSaveJson()
       {
           // add data not here but on run
           playerProfile.userName = "test";
           playerProfile.initialRating = 25;
           playerProfile.currentRating =  26;
           playerProfile.gamesPlayed = 25;
           playerProfile.gamesWon = 25;
           currentLevel = new LevelDetails();
           currentLevel.allottedTime = 20;
           currentLevel.expectedMoves = 6;
           currentLevel.levelRating = 5;
           currentLevel.playerRating = 25;
           playerProfile.levelsPlayed.Add(currentLevel);
           playerProfile.levelsPlayed.Add(currentLevel);

           var jsonString = playerProfile.SaveToString();
           if (File.Exists(Application.dataPath + @"\save\profile.json"))
               File.WriteAllText(Application.dataPath + @"\save\profile.json", jsonString);
           else
           {
               File.WriteAllText(Application.dataPath + @"\save\profile.json", jsonString);
           }
           Debug.Log(Application.dataPath);

            Debug.Log(jsonString);
        }
    }

    // holds level details

    public class SaveLoadUtility
    {
        
        // Test Profile
       

        


    }
}