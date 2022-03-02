/*
 Title: Level Details
 Author: Ron Matthew Rivera
 Sub-System: Main Part of Dynamic Difficulty Adjustment System
 Date Written/Revised: Dec. 15, 2021
 Purpose: Holds details about the current level
 Data Structures, algorithms, and control: Class
 */

using System;
using UnityEngine;

namespace ProjectAssets.Scripts.Gameplay.Difficulty_Adjustment
{
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
        public LevelDetails(bool win,int size, int time, int exMoves,int exPath,double lRating,double pRating,int pMove,int pSMove,double pTime,double pScore,int pLevelScore)
        {
            // seed is used to backup the info
            // concatenate all the data with spaces and add to the seed
            seed = $"{win.ToString()} {size} {time} {exMoves} {exPath} {lRating} {pRating} {pMove} {pSMove} {pTime} {pScore} {pLevelScore}";
            // encrypt the seed via Base64
            seed = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(seed));
            won = win;
            boardSize = size;
            allottedTime = time;
            expectedMoves = exMoves;
            suggestedPath = exPath;
            // round to 2 decimal places
            levelRating =  Math.Round(lRating,2);
            playerRating =  Math.Round(pRating,2);
            playerMove = pMove;
            playerMoveOnSuggestedPath = pSMove;
            playerRemainingTime = pTime;
            playerScore =  Math.Round(pScore,2);
            levelScore = pLevelScore;
        }
            
            

        public LevelDetails ReturnDetails()
        {
            return this;
        }

    }
}