using Unity.Mathematics;
using UnityEngine;
using UnityTemplateProjects.UI;
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

        public Fuzzy fuzzy;
        
        
        [Header("DDA")] public bool isDDAActive = true;
        [Header("DEBUG FUZZY")] public float moveInput = 16;
        public int debugMoves = 0 ;
        public float debugTime = 0 ;

        public float timeInput = 0 ;
        public float moveOutput = 0;
        [Header(" ")]
        public double levelRatingDebug = 0;
        public bool debugMode = true;
        public double nextLevelRating = 0;
        public double debugMoveScore;
        public double debugTimeScore;
        public double debugPlayerRating;
        public int giveUp = 4; // used to force a generation
        [Header("PostGame Analysis")] public bool playerWon = false;
      
        
        public PlayerProfile currentPlayer;
        public LevelDetails levelGenerated = new LevelDetails();    // this will be saved
        public LevelParameters parameters = new LevelParameters(); // this is the computation

        private void Awake()
        {
            GetProfile(SaveManager.Instance.playerProfile);
            fuzzy.SetMoves(debugMoves);
            fuzzy.SetTime(debugTime);
            fuzzy.SetIncrementalMoves();
            fuzzy.AddToDatabase();
        }

        private void LateUpdate()
        {
            moveOutput = fuzzy.AcceptInput(moveInput,timeInput);
            if (!debugMode) return;
            SetupDifficultyParameters();

        }

        private void GetProfile(PlayerProfile profile)
        {
            currentPlayer = profile;
            
        }

        public void GetPlayerMovement(int movement,int pathMovement)
        {
                
            levelGenerated.playerMove = movement;
            levelGenerated.playerMoveOnSuggestedPath = pathMovement;
            UIManager.Instance.ChangeMoveText(movement);
        }
        

        void GetInitialRating()
        {
            if (currentPlayer.gamesPlayed == 3)
            {
                double average = 0;
                for (int i = 0; i < 3; i++)
                {
                    average += currentPlayer.levelsPlayed[i].playerRating;
                }

                average /= 3;
                currentPlayer.currentRating = average;
            }
        }
        
        // invoke this after button press and after a level
        public void SetupDifficultyParameters() // we can edit this to for two builds With and wuthout DDA
        {
            // Setup Board Size
            levelGenerated.playerRating = currentPlayer.currentRating; // get current Player's Rating
            var rating = levelGenerated.playerRating;
            var randomBoardSize = 0;

            // 
            if (currentPlayer.gamesPlayed < 3)
            {
                randomBoardSize = currentPlayer.gamesPlayed switch
                {
                    0 => 4,
                    1 => 5,
                    2 => 6,
                    _ => 4
                };
                parameters.SetBoardSize(randomBoardSize);
            }
            else
            {
                if (GameManager.Instance.hasDDA)
                {
                    randomBoardSize = rating < 11 ? 4 : BoardSizeRatingRange(rating); // Disable this if you want a full random board everytime (NO DDA)
                    parameters.SetBoardSize(randomBoardSize); // Always the initial BoardSize // Use data from previous levels
                }
                else
                {
                    parameters.SetBoardSize(Random.Range(4, 11));    // Random Board Size
                }
            }

            var levelMoves = 12;
          // gamesplayed Conditions
          if (currentPlayer.gamesPlayed < 3)
              levelMoves = currentPlayer.gamesPlayed switch
              {
                  0 => 12,
                  1 => 16,
                  2 => 25,
                  _ => levelMoves
              };
          else
          {
              levelMoves = parameters.SetExpectedMoves();        // use data from previous levels
          } // add how many moves


          var levelTime = parameters.SetAllocatedTime();     // use data from previous levels instead of computing it?
            
            var timeScore = parameters.SetTimeCompletionScore();
            var completionScore = parameters.SetCompletionScore();
            
            parameters.timeCompletionScore = timeScore;
            parameters.completionScore = completionScore;
            parameters.expectedMoves = levelMoves;
           
            var score = parameters.SetExpectedScore(); // Expected Score and TIME
            var puzzleRating = parameters.SetPuzzleRating();  // determines the rating of the puzzle, might be a conflict
            
            parameters.levelRating = puzzleRating;
            parameters.expectedScore = score;
            levelRatingDebug = puzzleRating;
            
            
            // if DDA is not enabled
            if (!GameManager.Instance.hasDDA || currentPlayer.gamesPlayed < 3)
            {
                levelGenerated.playerMove = 0;
                levelGenerated.expectedMoves = levelMoves;
                levelGenerated.allottedTime = levelTime;
                levelGenerated.levelScore = (int) score;
                levelGenerated.levelRating = puzzleRating;
                levelGenerated.boardSize = parameters.boardSize;
                levelGenerated.playerRemainingTime = levelTime;
                
                UIManager.Instance.expectedMoves = levelMoves;
                UIManager.Instance.ChangeTimeText(levelGenerated.playerRemainingTime);

                return;
            }
            // set upper bounds and lower bounds
            //
            var lowerBound = LowerRatingBound((int) puzzleRating);
            var higherBound = HigherRatingBound((int) puzzleRating);
            // THIS is where the bounding happens, without this, the generated level will be Random
            // the bounds can be measured depending on the streak?
            if ( puzzleRating >= levelGenerated.playerRating - lowerBound && levelGenerated.playerRating <= puzzleRating &&
                puzzleRating <= levelGenerated.playerRating+ higherBound)
            {
                levelGenerated.playerMove = 0;
                levelGenerated.expectedMoves = levelMoves;
                levelGenerated.allottedTime = levelTime;
                levelGenerated.levelScore = (int) score;
                levelGenerated.levelRating = puzzleRating;
                levelGenerated.boardSize = parameters.boardSize;
                levelGenerated.playerRemainingTime = levelTime;
                
                UIManager.Instance.expectedMoves = levelMoves;
                UIManager.Instance.ChangeTimeText(levelGenerated.playerRemainingTime);

                return;
            }
            // if no level is generated, REPEAT
            SetupDifficultyParameters();

        }

        public void CountDownTimer()
        {
            levelGenerated.playerRemainingTime -= Time.deltaTime;
            UIManager.Instance.ChangeTimeText(levelGenerated.playerRemainingTime);
        }

       public void ComputeLevelScore() // invoke after finishing the level this will compute the score and the rating for the next level
       {    Debug.Log($"player remaining time: {levelGenerated.playerRemainingTime}");

           if (levelGenerated.playerRemainingTime != 0 && levelGenerated.playerMove>= levelGenerated.expectedMoves)
               playerWon = true;
           else
               playerWon = false;
           
           
           var playerScore = parameters.SetPlayerScore(ref levelGenerated.playerMove, ref levelGenerated.playerRemainingTime, levelGenerated.playerMoveOnSuggestedPath); // ref is a pointer reference to the variables
           levelGenerated.playerScore = playerScore;
           playerScore = playerWon ? playerScore : -playerScore;
            
           //  Debug.Log($"Puzzle Rating: {puzzleRating}");
           nextLevelRating = NextLevelRating();
           if (currentPlayer.gamesPlayed < 1)
           {
               debugPlayerRating = (playerScore / 2.5) + levelGenerated.playerRating + nextLevelRating / 1;

           }
           else
           {
               debugPlayerRating = playerScore + levelGenerated.playerRating + nextLevelRating / currentPlayer.gamesPlayed;

           }
           var resultant = levelGenerated.playerRating - debugPlayerRating;
         //  levelGenerated.playerRating = debugPlayerRating;

         
           Debug.Log("Next Level Rating: "+ nextLevelRating);
           Debug.Log($"Current Player Rating: {currentPlayer.currentRating}");
           Debug.Log($"debugPlayerRating ${debugPlayerRating} (pR + NR)");
          currentPlayer.currentRating = debugPlayerRating;
          
          levelGenerated.playerRating = currentPlayer.currentRating;
          var details = new LevelDetails(levelGenerated.seed, levelGenerated.boardSize,levelGenerated.allottedTime, levelGenerated.expectedMoves, levelGenerated.suggestedPath, levelGenerated.levelRating,levelGenerated.playerRating,levelGenerated.playerMove,levelGenerated.playerMoveOnSuggestedPath,levelGenerated.playerRemainingTime,levelGenerated.playerScore,levelGenerated.playerEvaluatedScore,levelGenerated.levelScore);
         
          currentPlayer.levelsPlayed.Add(details);
          
          currentPlayer.gamesPlayed = currentPlayer.levelsPlayed.Count;
          currentPlayer.gamesWon += playerWon ? 1 : 0;
           //Debug.Log($"LevelRating: {nextLevelRating} pr + nlr: {debugPlayerRating} pr-dbr= {resultant}  New Player Rating: {levelGenerated.playerRating}");
           SaveManager.Instance.playerProfile = currentPlayer;

           debugMoves = levelGenerated.expectedMoves;
           debugTime = levelGenerated.allottedTime;
           timeInput = (float) levelGenerated.playerRemainingTime;
           moveInput = levelGenerated.playerMove;

           // fuzzy.SetMoves(levelGenerated.expectedMoves);
           // fuzzy.SetTime(levelGenerated.allottedTime);
           // fuzzy.SetIncrementalMoves();
           // fuzzy.AddToDatabase();
           // moveOutput = fuzzy.AcceptInput(levelGenerated.playerMove,(float)levelGenerated.playerRemainingTime);
       }
       
       
        // This Fuzzy Logic (?)
       private double EvaluateMemberFunctions()
       {

           var moveValue= parameters.EvaluatePlayerMoves(levelGenerated.playerMove);
           var timeValue = parameters.EvaluatePlayerRemainingTime(levelGenerated.playerRemainingTime);

           bool absoluteWin = false;
           bool levelMoveClear = true;
           bool tooMuchMoveClear = false;

           bool perfectTimeClear = true;
           bool halfTimeClear = false;
           bool belowfHalfTimeClear = false;
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


            // TWEAK THE RETURN VALUES TO ADJUST THE SCORING 
           if (levelFailed)
               return -1;
           if (perfectTimeClear && absoluteWin) return 1;

           if (levelMoveClear && perfectTimeClear) return .5;
           if (tooMuchMoveClear && perfectTimeClear) return .05;
           if (absoluteWin && halfTimeClear) return 0.25;

           if (absoluteWin) return .01;

           if (tooMuchMoveClear && belowfHalfTimeClear) return -0.05;

           if (levelMoveClear && halfTimeClear) return .5;

           if (tooMuchMoveClear) return 0.05;

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

           var result = currentPlayerRating * multiplier;


           Debug.Log($"Next Level Rating/Player Rating Computation: CurrentPR: {currentPlayerRating} levelRating: {levelRating}  multiplier: {multiplier} ratingResult: {ratingResult} playerScore: {playerScore}  result: {result} ");
           return result;
       }

        // Rules
        private int BoardSizeRatingRange(double playerRating)
       {
           if (playerRating < 20) return Random.Range(4, 6);
           if (playerRating > 20 && playerRating < 30) return Random.Range(4, 6);

           if (playerRating > 30 && playerRating < 40) return Random.Range(4, 7);
           if (playerRating > 40 && playerRating < 50) return Random.Range(5, 7);
           if (playerRating > 50 && playerRating < 60) return Random.Range(5, 8);
           if (playerRating > 70 && playerRating < 80) return Random.Range(6, 9);
           if (playerRating > 80 && playerRating < 95) return Random.Range(7, 10);
           return Random.Range(8, 11);
           
       }

       
       // if (puzzleRating >= (levelGenerated.playerRating - 5) && levelGenerated.playerRating <= puzzleRating &&
       // puzzleRating <= levelGenerated.playerRating+ 5)
       // Rules
       private int LowerRatingBound(int puzzleRating)
       {
           if (puzzleRating < 20)        // its hard to generate puzzles lower than 20
               return 6;
           if (puzzleRating > 20 && puzzleRating < 30) return 4;
           if (puzzleRating > 30 && puzzleRating < 40) return 5;
           if (puzzleRating > 40 && puzzleRating < 50) return 8;
           if (puzzleRating > 50 && puzzleRating < 60) return 10;
           if (puzzleRating > 60 && puzzleRating < 70) return 15;
           if (puzzleRating > 70 && puzzleRating < 80) return 20;
           if (puzzleRating > 80 && puzzleRating < 95) return 25;
           // this just means that your Rating is over one hundred
           return 50;
       }

       private int HigherRatingBound(int puzzleRating)
       {
           if (puzzleRating < 20) return 10;
           if (puzzleRating > 20 && puzzleRating < 30) return 10;
           if (puzzleRating > 30 && puzzleRating < 40) return 14;
           if (puzzleRating > 40 && puzzleRating < 50) return 16;
           if (puzzleRating > 50 && puzzleRating < 60) return 18;
           if (puzzleRating > 60 && puzzleRating < 70) return 22;
           if (puzzleRating > 70 && puzzleRating < 80) return 25;
           if (puzzleRating > 80 && puzzleRating < 95) return 35;
           // this just means that your Rating is over one hundred
           return 50;
       }

       private void CalculateLevelRating()
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