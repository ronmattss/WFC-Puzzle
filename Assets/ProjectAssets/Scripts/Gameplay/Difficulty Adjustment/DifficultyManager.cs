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
        public bool won;// The Rating of the level Generated
        public int boardSize;
        public int allottedTime;
        public int expectedMoves;
        public int suggestedPath;
        public int levelScore;
        public double levelRating;
        
        public double playerRating;   // The Current rating of the player playing this level
        [Range(0,200)]
        public int playerMove;

        public int playerMoveOnSuggestedPath;
        [Range(0,500)]
        public double playerRemainingTime;
        public double playerScore;

        // should the player's performance also be here?
        
            // Set the level Details

            public LevelDetails()
            {
                
            }
            public LevelDetails(string boardSeed,bool win,int size, int time, int exMoves,int exPath,double lRating,double pRating,int pMove,int pSMove,double pTime,double pScore,int pLevelScore)
            {
                seed = boardSeed;
                won = win;
                boardSize = size;
                allottedTime = time;
                expectedMoves = exMoves;
                suggestedPath = exPath;
                levelRating = lRating;
                playerRating = pRating;
                playerMove = pMove;
                playerMoveOnSuggestedPath = pSMove;
                playerRemainingTime = pTime;
                playerScore = pScore;
                levelScore = pLevelScore;
            }
            
            

            public LevelDetails ReturnDetails()
            {
                return this;
            }

    }

    [Serializable]
    public class PlayerProfile
    {
        public string userName;
        public double initialRating;
        public double currentRating;
        public int gamesPlayed;
        public int gamesWon;
        public List<LevelDetails> levelsPlayed = new List<LevelDetails>();

        // create blank template
        public PlayerProfile()
        {
            userName = "";
            initialRating = 10;
            currentRating = 10;
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