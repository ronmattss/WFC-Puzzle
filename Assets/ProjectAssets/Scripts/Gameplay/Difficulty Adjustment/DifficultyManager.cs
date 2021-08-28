using System;
using System.Collections.Generic;
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
        // Monobehavior that helps place end goals better
        // DDA 
        // adjusts the difficulty accordingly
        
        // Needs Player Profile
        // 
        // Check the players current Rating
        // modify necessary level details
        // after checking pass its info to the level generator

        void CheckPlayerProfile()
        {
            // if non exist prompt to create a new profile
            // else load an existing profile
        }
        
        
        void CheckPlayerRating()
        {
            //get the players Initial Rating
            // then process it when generating a new level
        }

        void ProcessDifficulty()
        {
            
        }

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
           currentLevel.seed = "213dasdwe22";
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
    [Serializable]
    public class LevelDetails
    {
        public string seed;
        public int boardSize;
        public int allottedTime;
        public int expectedMoves;
        public int levelScore;
        public double levelRating;    // The Rating of the level Generated
        
        public double playerRating;   // The Current rating of the player playing this level
        [Range(0,200)]
        public int playerMove;
        [Range(0,500)]
        public double playerRemainingTime;
        public double playerScore;
        public double playerEvaluatedScore;
        
        // should the player's performance also be here?
        
            // Set the level Details

            public LevelDetails()
            {
                
            }
            public LevelDetails(int time, int exMoves,int lRating,int pRating,int pMove,int pScore)
            {
                allottedTime = time;
                expectedMoves = exMoves;
                levelRating = lRating;
                playerRating = pRating;
                playerMove = pMove;
                playerScore = pScore;
            }

    }

    [Serializable]
    public class PlayerProfile
    {
        public string userName;
        public int initialRating;
        public int currentRating;
        public int gamesPlayed;
        public int gamesWon;
        public List<LevelDetails> levelsPlayed = new List<LevelDetails>();

        // create blank template
        public PlayerProfile()
        {
            userName = "";
            initialRating = 0;
            currentRating = 0;
            gamesPlayed = 0;
            gamesWon = 0;
        }

        public PlayerProfile(string _userName,int _initialRating,int _currentRating,int _gamePlayed,int _gamesWon)
        {
            userName = _userName;
            initialRating = _initialRating;
            currentRating = _currentRating;
            gamesPlayed = _gamePlayed;
            gamesWon = _gamesWon;

        }

        public PlayerProfile SetBlankTemplate()
        {
            return this;
        }
       // private List<LevelDetails> levelsPlayed;
        
        // Persistence is the Key :>


        void TryLoadPlayerProfile()
        {
            // check if a profile exist
            
        }

        public String SaveToString()
        {
            return JsonUtility.ToJson(this);
        }

        // Load PlayerProfile
        // If does not exist Create a new one
        // 

    }

    public class SaveLoadUtility
    {
        
        // Test Profile
       

        


    }
}