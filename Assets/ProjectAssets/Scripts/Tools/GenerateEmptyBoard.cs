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
       public TMP_InputField boardSizeInput; 
       public TMP_InputField movesInput;
       public TMP_InputField playerMovesInput;
       public TMP_InputField playerRemainingTimeInput;
       public TMP_InputField playerRatingInput;  
       public TMP_InputField gamesPlayedInput;  










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

        public void ComputePlayerRating()
        {
            playerRating = difficultyModifier.AddPlayerRating(gamesplayed, (int)playerRating,
                levelRating, playerScore, playerWon);
        }
        
        public void LateUpdate()
        {
            allottedTime = difficultyModifier.parameters.SetAllocatedTime(expectedMoves,boardSize);
            levelRating = Math.Round(difficultyModifier.SetLevelRating(expectedMoves, allottedTime, boardSize), 2);
            playerScore =
                (int)difficultyModifier.SetScore(playerMoves, expectedMoves,timeLeft ,playerMovedOnSuggestedPath, playerWon);
            moveIncrementText.text = $"Move Increment: {OutputFuzzyBasedMoves(playerMoves, timeLeft)} Next Level Moves: {OutputFuzzyBasedMoves(playerMoves, timeLeft) + expectedMoves}";
            expectedMovesText.text = $"expected Moves: {expectedMoves}";
            playerMovesText.text = $"player Moves: {playerMoves}";
            allottedTimeText.text = $"Allotted Time: {allottedTime}";
            timeLeftText.text = $"Time Left: {timeLeft}";
            BoardSizeText.text = $"Board Size: {boardSize}";
            levelScoreText.text = $"Level Score: { playerScore}";
            playerRatingText.text = $"Player Rating:{playerRating} ";
            levelRatingText.text = $"level Rating:{Math.Round(difficultyModifier.SetLevelRating(expectedMoves,allottedTime,boardSize),2)} ";
            
        }

        public void ChangeBoardSize()
        {
            boardSize = int.Parse(boardSizeInput.text);
        }
        public void ChangeMoves()
        {
            expectedMoves = int.Parse(movesInput.text);
        }
        public void ChangePlayerMoves()
        {
            playerMoves = int.Parse(playerMovesInput.text);
        }
        public void ChangeRemainingTime()
        {
            timeLeft = float.Parse(playerRemainingTimeInput.text);
        }
        public void ChangePlayerRating()
        {
            playerRating = float.Parse(playerRatingInput.text);
        }
        public void ChangeGamesPlayed()
        {
            gamesplayed = int.Parse(gamesPlayedInput.text);
        }
        
    }
}