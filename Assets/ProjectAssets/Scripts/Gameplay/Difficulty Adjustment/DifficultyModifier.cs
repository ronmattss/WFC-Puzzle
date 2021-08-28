using UnityEngine;

namespace ProjectAssets.Scripts.Gameplay.Difficulty_Adjustment
{
    public class DifficultyModifier : MonoBehaviour
    {
        // Script that handles the difficulty of the levels
        // main Parameters
        // the player's current rating
        // level parameters: width height, expected moves, time, 
        
        [Range(4,15)]
        public int boardSize = 4;
        [Range(0,100)]
        public int moves = 4;

        public double levelRatingDebug = 0;
        public bool debugMode = true;
      
        
        public PlayerProfile currentPlayer;
        public LevelDetails levelGenerated = new LevelDetails();
        public LevelParameters parameters = new LevelParameters();

        void Awake()
        {
            //get currentplayer
        }

        void LateUpdate()
        {
            if (!debugMode) return;
            SetupDifficultyParameters();
        }

        void GetProfile(PlayerProfile profile)
        {
            currentPlayer = profile;
            
        }

       public void SetupDifficultyParameters()
        {
            // Setup Board Size
            parameters.SetBoardSize(boardSize); // Always the initial BoardSize
            
           var levelMoves = parameters.SetExpectedMoves(moves); // add how many moves
          var levelTime = parameters.SetAllocatedTime();
            var timeScore = parameters.SetTimeCompletionScore();
            var completionScore = parameters.SetCompletionScore();
            
            parameters.timeCompletionScore = timeScore;
            parameters.completionScore = completionScore;
            parameters.expectedMoves = levelMoves;
           
            var score = parameters.SetExpectedScore(); // Expected Score and TIME
            var puzzleRating = parameters.SetPuzzleRating();
            parameters.levelRating = puzzleRating;
            parameters.expectedScore = score;
            levelRatingDebug = puzzleRating;


            levelGenerated.expectedMoves = levelMoves;
            levelGenerated.allottedTime = levelTime;
            levelGenerated.levelScore = (int) score;
            levelGenerated.levelRating = puzzleRating;
            levelGenerated.boardSize = parameters.boardSize;

            var playerScore = parameters.SetPlayerScore(ref levelGenerated.playerMove, ref levelGenerated.playerRemainingTime); // ref is a pointer reference to the variables
            levelGenerated.playerScore = playerScore == 0 ? 0 : playerScore;
            
          //  Debug.Log($"Puzzle Rating: {puzzleRating}");


        }


       // 



       
       
       
       
       
       
       
       
       
       
       
       

       public void ModifyDifficulty()
       {
           
       }

        void CalculateLevelRating()
        {
            var currentPlayerRating = currentPlayer.currentRating;
            
            
            // first randomly create a level Rating
            // compare playerRating to the level Rating
            // if is not in range reduce or add level rating 


        }
        
        
        
        
        
    }
}