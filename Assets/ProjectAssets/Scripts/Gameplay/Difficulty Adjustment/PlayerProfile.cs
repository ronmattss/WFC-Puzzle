/*
 Title: Player Profile
 Author: Ron Matthew Rivera
 Sub-System: Save system
 Date Written/Revised: Sept. 22, 2021
 Purpose: Class that holds the player's profile data
 Data Structures, algorithms, and control: Class. Lists
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectAssets.Scripts.Gameplay.Difficulty_Adjustment
{
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
}