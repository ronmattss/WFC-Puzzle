using System;
using UnityEngine;
using TMPro;
using ProjectAssets.Scripts.Gameplay.Difficulty_Adjustment;
namespace ProjectAssets.Scripts.Tools
{
    public class GenerateEmptyBoard : MonoBehaviour
    {
        // This class will Generate an empty board for Tool Demonstration purposes.
        // contains board generation code
        
        
        public Gameplay.Difficulty_Adjustment.Fuzzy fuzzy;
        [Header("Tool Simulation Parameters")]

        public int boardSize; 
        public int expectedMoves; 
        public int playerMoves; 
        public int allottedTime; 
        public float timeLeft; 
        [Header("Tool Simulation Parameters")]
       public TMP_Text movesLeftText; 
       public TMP_Text timeLeftText; 
       public TMP_Text allottedTimeText; 
       public TMP_Text playerMovesText; 


        
        
        private void FuzzyBasedMoves(int expectedMoves,float allottedTime)
        {   fuzzy = new Gameplay.Difficulty_Adjustment.Fuzzy();
            fuzzy.SetMoves(expectedMoves);
            fuzzy.SetTime(allottedTime);
            fuzzy.SetIncrementalMoves();
            fuzzy.AddToDatabase();
            // Debug Stuff
            // debugMoves = expectedMoves;
            // debugTime = allottedTime;
        }
        
        private int OutputFuzzyBasedMoves(int pMoves, float pTimeRemaining)
        {

            playerMoves = pMoves;
            timeLeft = pTimeRemaining;
            return Mathf.FloorToInt(fuzzy.AcceptInput(pMoves, pTimeRemaining));
        }

        public void LateUpdate()
        {
            
        }
    }
}