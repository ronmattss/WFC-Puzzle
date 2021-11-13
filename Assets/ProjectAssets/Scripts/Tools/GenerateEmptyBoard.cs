using System;
using UnityEngine;
using TMPro;
using ProjectAssets.Scripts.Gameplay.Difficulty_Adjustment;
using ProjectAssets.Scripts.Util;

namespace ProjectAssets.Scripts.Tools
{
    public class GenerateEmptyBoard : Singleton<GenerateEmptyBoard>
    {
        // This class will Generate an empty board for Tool Demonstration purposes.
        // contains board generation code
        
        
        public Gameplay.Difficulty_Adjustment.Fuzzy fuzzy;
        [Header("Tool  Parameters")]
        [Header("Level Information")]
        public int boardSize; 
        public int expectedMoves; 
        public int allottedTime; 
        public double levelRating; 
        [Header("player Level Performance")]
        public int playerMoves; 
        public float timeLeft; 
        public int playerMovedOnSuggestedPath; 
        public bool playerWon;
        public int playerScore;
        [Header("player Performance")]
        public double playerRating; 
        public int gamesplayed;

 
        
        public DifficultyModifier difficultyModifier;
        [Header("Tool  UI")]
       public TMP_Text expectedMovesText; 
       public TMP_Text timeLeftText; 
       public TMP_Text allottedTimeText; 
       public TMP_Text playerMovesText; 
       public TMP_Text moveIncrementText; 
       public TMP_Text BoardSizeText; 
       public TMP_Text levelScoreText; 
       public TMP_Text levelRatingText;
       public TMP_Text playerRatingText; 






        
        
        private void FuzzyBasedMoves(int _expectedMoves,float _allottedTime)
        {   fuzzy = new Gameplay.Difficulty_Adjustment.Fuzzy();
            fuzzy.SetMoves(_expectedMoves);
            fuzzy.SetTime(_allottedTime);
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

        private void Awake()
        {
            ComputeMoves();
        }


        public void ComputeMoves()
        {
            FuzzyBasedMoves(expectedMoves,allottedTime);
          //  difficultyModifier.SetPlayerScore(playerMoves,timeLeft,playerMovedOnSuggestedPath);
        }
        
        public void LateUpdate()
        {
            allottedTime = difficultyModifier.parameters.SetAllocatedTime(expectedMoves,boardSize);
            moveIncrementText.text = $"Move Increment: {OutputFuzzyBasedMoves(playerMoves, timeLeft)}";
            expectedMovesText.text = $"expected Moves: {expectedMoves}";
            playerMovesText.text = $"player Moves: {playerMoves}";
            allottedTimeText.text = $"Allotted Time: {allottedTime}";
            timeLeftText.text = $"Time Left: {timeLeft}";
            BoardSizeText.text = $"Board Size: {boardSize}";
            levelScoreText.text = $"Level Score: { difficultyModifier.SetPlayerScore(playerMoves,timeLeft,playerMovedOnSuggestedPath,playerWon)}";
            playerRatingText.text = $"Player Rating:{difficultyModifier.AddPlayerRating(gamesplayed, (int)playerRating, difficultyModifier.NextLevelRating(levelRating, playerRating), playerScore, playerWon)} ";
            levelRatingText.text = $"Player Rating:{difficultyModifier.SetLevelRating(expectedMoves,allottedTime)} ";

            Debug.Log(
                $"PlayerRating Increment: {difficultyModifier.AddPlayerRating(gamesplayed, (int)playerRating, difficultyModifier.NextLevelRating(levelRating, playerRating), playerScore, playerWon)}");
        }
        
        
    }
}