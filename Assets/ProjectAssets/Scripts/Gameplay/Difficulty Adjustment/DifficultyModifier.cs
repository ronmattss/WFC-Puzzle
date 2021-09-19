using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

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
        public double nextLevelRating = 0;
        public double debugMoveScore;
        public double debugTimeScore;
        public double debugPlayerRating;

        public AnimationCurve valueCurve;
        public float2 curveValues;
      
        
        public PlayerProfile currentPlayer;
        public LevelDetails levelGenerated = new LevelDetails();    // this will be saved
        public LevelParameters parameters = new LevelParameters(); // this is the computation

        void Awake()
        {
            GetProfile(SaveManager.Instance.playerProfile);
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

        public void GetPlayerMovement(int movement)
        {
            levelGenerated.playerMove = movement;
        }
        // invoke this after button press and after a level
        public void SetupDifficultyParameters()
        {
            // Setup Board Size

            var randomBoardSize = Random.Range(4, 11);  
            levelGenerated.playerRating = currentPlayer.currentRating; // get current Player's Rating
            parameters.SetBoardSize(randomBoardSize); // Always the initial BoardSize
           var levelMoves = parameters.SetExpectedMoves(); // add how many moves
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

            if (puzzleRating >= (levelGenerated.playerRating - 5) && levelGenerated.playerRating <= puzzleRating &&
                puzzleRating <= levelGenerated.playerRating+ 5)
            {
                levelGenerated.playerMove = 0;
                levelGenerated.expectedMoves = levelMoves;
                levelGenerated.allottedTime = levelTime;
                levelGenerated.levelScore = (int) score;
                levelGenerated.levelRating = puzzleRating;
                levelGenerated.boardSize = parameters.boardSize;
                return;
            }
            // if no level is generated, REPEAT
            SetupDifficultyParameters();

        }

       public void ComputeLevelScore() // invoke after finishing the level this will compute the score and the rating for the next level
       {
           var playerScore = parameters.SetPlayerScore(ref levelGenerated.playerMove, ref levelGenerated.playerRemainingTime); // ref is a pointer reference to the variables
           levelGenerated.playerScore = playerScore;
           
            
           //  Debug.Log($"Puzzle Rating: {puzzleRating}");
           nextLevelRating = NextLevelRating();
           debugPlayerRating = levelGenerated.playerRating + nextLevelRating / currentPlayer.gamesPlayed;
           var resultant = levelGenerated.playerRating - debugPlayerRating;
         //  levelGenerated.playerRating = debugPlayerRating;

         
           Debug.Log("Next Level Rating: "+ nextLevelRating);
           Debug.Log($"Current Player Rating: {currentPlayer.currentRating}");
           Debug.Log($"debugPlayerRating ${debugPlayerRating} (pR + NR)");
          currentPlayer.currentRating = debugPlayerRating;
          levelGenerated.playerRating = currentPlayer.currentRating;
          var details = new LevelDetails(levelGenerated.seed,levelGenerated.boardSize,levelGenerated.allottedTime,levelGenerated.expectedMoves,levelGenerated.levelRating,levelGenerated.playerRating,levelGenerated.playerMove,levelGenerated.playerScore,levelGenerated.playerEvaluatedScore,levelGenerated.levelScore);
          currentPlayer.levelsPlayed.Add(details);
           //Debug.Log($"LevelRating: {nextLevelRating} pr + nlr: {debugPlayerRating} pr-dbr= {resultant}  New Player Rating: {levelGenerated.playerRating}");
           SaveManager.Instance.playerProfile = currentPlayer;
       }

       private double EvaluateMemberFunctions()
       {

           var moveValue= parameters.EvaluatePlayerMoves(levelGenerated.playerMove);
           var timeValue = parameters.EvaluatePlayerRemainingTime(levelGenerated.playerRemainingTime);

           var absoluteWin = false;
           var levelMoveClear = true;
           var tooMuchMoveClear = false;

           var perfectTimeClear = true;
           var halfTimeClear = false;
           var belowfHalfTimeClear = false;
           var levelFailed = false;

  
           if ((int)moveValue == 1 &&(int) timeValue == 1)
           {
               absoluteWin = true;
           }
           else if (moveValue >= .25)
           {
               absoluteWin = false;
               levelMoveClear = true;
               tooMuchMoveClear = false;
           }
           else
           {
               absoluteWin = false;
               levelMoveClear = false;
               tooMuchMoveClear = true;
           }

           if ((int) timeValue == 1)
           {
               perfectTimeClear = true;
               halfTimeClear = false;
               levelFailed = false;
               belowfHalfTimeClear = false;
           }
           else if (timeValue >= 0.05)
           {
               perfectTimeClear = false;
               halfTimeClear = true;
               belowfHalfTimeClear = false;
               levelFailed = false;
           }
           else if (timeValue < 0.049 && timeValue != 0)
           {
               belowfHalfTimeClear = true;
               perfectTimeClear = false;
               halfTimeClear = false;
               levelFailed = false;
           }
           else
           {
               belowfHalfTimeClear = false;
               perfectTimeClear = false;
               halfTimeClear = false;
               levelFailed = true;
           }



           if (levelFailed)
               return -1;
           if (perfectTimeClear && absoluteWin)
           {
               return 1;
           }

           if (levelMoveClear && perfectTimeClear)
           {
               return .5;
           }
           if (tooMuchMoveClear && perfectTimeClear)
           {
               return .05;
           }
           if (absoluteWin && halfTimeClear)
           {
               return 0.25;
           }

           if (absoluteWin)
           {
               return .01;
           }

           if (tooMuchMoveClear && belowfHalfTimeClear)
           {
               return -0.05;
           }

           if (levelMoveClear && halfTimeClear)
           {
               return .5;
           }

           if (tooMuchMoveClear)
           {
               return 0.05;
           }

           return 0;
       }

       public double NextLevelRating()
       {
           // (LR + PS)/GP + CR
           var currentPlayerRating = levelGenerated.playerRating;
           var levelRating = levelGenerated.levelRating;
           var  multiplier = EvaluateMemberFunctions();
           var playerScore =levelGenerated.playerScore;
           var ratingResult = 0.0;

           if ((int) multiplier == 1)
           {
               
               ratingResult = levelRating - currentPlayerRating;
               currentPlayerRating += ratingResult;
               currentPlayerRating /= 2;

           }
           else if (multiplier >= .5 && multiplier < 1)
           {
               currentPlayerRating /= 1.75;
           }

           var result = ((currentPlayerRating) * multiplier);


           Debug.Log($"Next Level Rating/Player Rating Computation: CurrentPR: {currentPlayerRating} levelRating: {levelRating}  multiplier: {multiplier} ratingResult: {ratingResult} playerScore: {playerScore}  result: {result} ");
           return result;
       }

        void CalculateLevelRating()
        {
            var currentPlayerRating = currentPlayer.currentRating;
            
            
            // first randomly create a level Rating
            // compare playerRating to the level Rating
            // if is not in range reduce or add level rating 
            // next is to apply the DDA to the level generator
            // save new profile 


        }


    }
}