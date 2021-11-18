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
// CalculateLevelRating();
            // fuzzy.SetMoves(debugMoves);
            // fuzzy.SetTime(debugTime);
            // fuzzy.SetIncrementalMoves();
            // fuzzy.AddToDatabase();
        }

        private void FuzzyBasedMoves(int expectedMoves,float allottedTime)
        {   fuzzy = new Fuzzy();
            fuzzy.SetMoves(expectedMoves);
            fuzzy.SetTime(allottedTime);
            fuzzy.SetIncrementalMoves();
            fuzzy.AddToDatabase();
            // Debug Stuff
            debugMoves = expectedMoves;
            debugTime = allottedTime;
        }

        private int OutputFuzzyBasedMoves(float playerMoves, float timeRemaining)
        {

            moveInput = playerMoves;
            timeInput = timeRemaining;
            return Mathf.FloorToInt(fuzzy.AcceptInput(playerMoves, timeRemaining));
        }

        private void LateUpdate()
        {
          //  moveOutput = fuzzy.AcceptInput(moveInput,timeInput);
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
        

      



        void SetupBoardSize(double rating,int moveNumber)
        {
            var randomBoardSize = 0;

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
                    randomBoardSize = FitMovesOnBoard(randomBoardSize,moveNumber);
                    parameters.SetBoardSize(randomBoardSize); // Always the initial BoardSize // Use data from previous levels
                }
                else
                {
                    parameters.SetBoardSize(Random.Range(4, 11));    // Random Board Size
                }
            }
        }

        int FitMovesOnBoard(int board, int eMoves)
        {
            var newSize = board;
            var eM = eMoves;
            
            //check if moves is greater than board

            while (eM >= (parameters.MaxMoves(newSize)))
            {
                newSize++;
                
            }
            return newSize;
        }

        int SetupMoves(int moveOut)
        {   
            if (currentPlayer.gamesPlayed < 3)
                return currentPlayer.gamesPlayed switch
                {
                    0 => 12,
                    1 => 16,
                    2 => 25,
                    _ => moveOut
                };
            
            var previousLevelIndex = SaveManager.Instance.playerProfile.levelsPlayed.Count-1;
            var previousLevel = null ?? SaveManager.Instance.playerProfile.levelsPlayed[previousLevelIndex];
            if (SaveManager.Instance.playerProfile.levelsPlayed.Count == 3)
            {
                var average =  (double)((SaveManager.Instance.playerProfile.levelsPlayed[0].expectedMoves +  SaveManager.Instance.playerProfile.levelsPlayed[1].expectedMoves +  SaveManager.Instance.playerProfile.levelsPlayed[2].expectedMoves) /3.0)*.7;
                return (int) ( average + moveOutput);
            }

            return (int) ((int) previousLevel.expectedMoves + moveOutput);
            // moveOut = parameters.SetExpectedMoves();        // use data from previous levels /// Fuzzy Logic Based

        }
        
        public double SetLevelRating(double rating,double moveIncrement, bool previousLevelWon)
        { // =(EM/((AT*0.7)*0.7))*15
            var newRating = rating;
            if (previousLevelWon)
            {
                newRating+=moveIncrement;
            }
            else
            {
                newRating-= Mathf.Abs((int)moveIncrement);
            }

            return newRating;
        }


       public double SetLevelRating(double em,double at)
       {
           var firstEquation = ((at * 0.7) * .7);
           var secondEquation = em / firstEquation;
            return (em / ((at * 0.7) * 0.7)) * 15;
        }
       public double SetLevelRating(double em,double at,int size)
       {
           var firstEquation = ((at * 0.7) * .7);
           // var secondEquation = em / firstEquation;
           var y = (firstEquation * size + em) * .15; 
           return y;
       }

        
        // Why Does Dis not work sometimes LMAO
        // invoke this after button press and after a level
        public void SetupDifficultyParameters() // we can edit this to for two builds With and wuthout DDA
        {
           // CalculateLevelRating();
           var puzzleRating = 0;

            if (SaveManager.Instance.playerProfile.levelsPlayed.Count > 2)
            {
                
                var previousLevelIndex = SaveManager.Instance.playerProfile.levelsPlayed.Count-1;
                var previousLevel =  SaveManager.Instance.playerProfile.levelsPlayed[previousLevelIndex];


                FuzzyBasedMoves(previousLevel.expectedMoves,previousLevel.allottedTime);
                moveOutput = OutputFuzzyBasedMoves(previousLevel.playerMove, (float)previousLevel.playerRemainingTime);
            }

            // Setup Board Size
            levelGenerated.playerRating = currentPlayer.currentRating; // get current Player's Rating
            var rating = levelGenerated.playerRating;
            var randomBoardSize = 0;

            // 
            var levelMoves = 12;

            if (isDDAActive)
            {
                levelMoves = SetupMoves(levelMoves);
                SetupBoardSize(rating,levelMoves);
            }
            else
            {
                parameters.SetBoardSize(Random.Range(4, 11));
                levelMoves = parameters.SetExpectedMoves();

            }




            var levelTime = parameters.SetAllocatedTime();     // use data from previous levels instead of computing it?
            
            var timeScore = parameters.SetTimeCompletionScore();
            var completionScore = parameters.SetCompletionScore();
            
            parameters.timeCompletionScore = timeScore;
            parameters.completionScore = completionScore;
            parameters.expectedMoves = levelMoves;
           
            var score = parameters.SetExpectedScore(); // Expected Score and TIME
            if (SaveManager.Instance.playerProfile.levelsPlayed.Count > 2)
            {
             //   puzzleRating = (int)SetLevelRating(SaveManager.Instance.playerProfile.currentRating,moveOutput,SaveManager.Instance.playerProfile.levelsPlayed[SaveManager.Instance.playerProfile.levelsPlayed.Count-1].won);  // determines the rating of the puzzle, via move increment
             puzzleRating = (int)SetLevelRating(levelMoves, levelTime);
            }
            else
            {
              //  puzzleRating = (int)parameters.SetPuzzleRating(); // determines the rating of the puzzle, might be a conflict
              puzzleRating = (int)SetLevelRating(levelMoves, levelTime);
            }
            
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
            
            
            var lowerBound = 100;//LowerRatingBound((int) puzzleRating);
            var higherBound = 100; //HigherRatingBound((int) puzzleRating);
            // THIS is where the bounding happens, without this, the generated level will be Random
            // the bounds can be measured depending on the streak?
            
            // if ( puzzleRating >= levelGenerated.playerRating - lowerBound && levelGenerated.playerRating <= puzzleRating &&
            //     puzzleRating <= levelGenerated.playerRating+ higherBound)
            if(puzzleRating != 0)
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

        // To be used in the demonstration
        public double SetPlayerScore(int pMoves, double pTime,int pMoveOnSuggestedPath,bool won )
        {
 
            var playerScore = parameters.SetPlayerScore(ref pMoves, ref pTime, pMoveOnSuggestedPath); // ref is a pointer reference to the variables
            playerScore = won ? playerScore : -(playerScore * 3);
            return playerScore;
        }
        public double SetScore(int pMoves,int eMoves, double pTime,int pMoveOnSuggestedPath,bool won )
        {
 
            var playerScore = parameters.SetScoreOfPlayer( pMoves,  eMoves,pTime, pMoveOnSuggestedPath); // ref is a pointer reference to the variables
            playerScore = won ? playerScore : -(playerScore);
            return playerScore;
        }

       public void ComputeLevelScore() // invoke after finishing the level this will compute the score and the rating for the next level
       {    Debug.Log($"player remaining time: {levelGenerated.playerRemainingTime}");

           if (levelGenerated.playerRemainingTime != 0 && levelGenerated.playerMove>= levelGenerated.expectedMoves)
               playerWon = true;
           else
               playerWon = false;
           
           
           var playerScore = parameters.SetPlayerScore(ref levelGenerated.playerMove, ref levelGenerated.playerRemainingTime, levelGenerated.playerMoveOnSuggestedPath); // ref is a pointer reference to the variables
           levelGenerated.playerScore = playerScore;
           playerScore = playerWon ? playerScore : -(playerScore * 3);
            
           //  Debug.Log($"Puzzle Rating: {puzzleRating}"); 
           nextLevelRating = NextLevelRating();
           if (currentPlayer.gamesPlayed < 3)
           {
               debugPlayerRating =  (levelGenerated.playerRating + (nextLevelRating* .01f) / 1) + (playerScore / 2.5); // This is based from elo rating of chess

           }
           // This is based from elo rating of chess
           else
           {
               if(playerWon)
                debugPlayerRating = (levelGenerated.playerRating + (nextLevelRating * .01f) / currentPlayer.gamesPlayed) + (playerScore / 6); // divided by 6 so player Rating will not go up to much
               else
                   debugPlayerRating = (levelGenerated.playerRating + (nextLevelRating * .01f) / currentPlayer.gamesPlayed) + (playerScore);

           }
           var resultant = levelGenerated.playerRating - debugPlayerRating;
         //  levelGenerated.playerRating = debugPlayerRating;

         
           Debug.Log("Next Level Rating: "+ nextLevelRating);
           Debug.Log($"Current Player Rating: {currentPlayer.currentRating}");
           Debug.Log($"debugPlayerRating ${debugPlayerRating} (pR + NR)");
          currentPlayer.currentRating = debugPlayerRating;
          
          levelGenerated.playerRating = currentPlayer.currentRating;
          var details = new LevelDetails(levelGenerated.seed,playerWon, levelGenerated.boardSize,levelGenerated.allottedTime, levelGenerated.expectedMoves, levelGenerated.suggestedPath, levelGenerated.levelRating,levelGenerated.playerRating,levelGenerated.playerMove,levelGenerated.playerMoveOnSuggestedPath,levelGenerated.playerRemainingTime,levelGenerated.playerScore,levelGenerated.levelScore);
         
          currentPlayer.levelsPlayed.Add(details);
          
          currentPlayer.gamesPlayed = currentPlayer.levelsPlayed.Count;
          currentPlayer.gamesWon += playerWon ? 1 : 0;
           //Debug.Log($"LevelRating: {nextLevelRating} pr + nlr: {debugPlayerRating} pr-dbr= {resultant}  New Player Rating: {levelGenerated.playerRating}");
           SaveManager.Instance.playerProfile = currentPlayer;

           debugMoves = levelGenerated.expectedMoves;
           debugTime = levelGenerated.allottedTime;
           timeInput = (float) levelGenerated.playerRemainingTime;
           moveInput = levelGenerated.playerMove;

           if (currentPlayer.gamesPlayed == 3)
           {
               CalculateLevelRating();
           }

           // fuzzy.SetMoves(levelGenerated.expectedMoves);
           // fuzzy.SetTime(levelGenerated.allottedTime);
           // fuzzy.SetIncrementalMoves();
           // fuzzy.AddToDatabase();

           // moveOutput = fuzzy.AcceptInput(levelGenerated.playerMove,(float)levelGenerated.playerRemainingTime);
       }
       
       
        // This Fuzzy Logic (?)
        // this not fuzzy
       private double EvaluatePlayerPerformance()
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
           var  multiplier = EvaluatePlayerPerformance();
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

       public double NextLevelRating(double lRating,  double pRating)
       {

               // (LR + PS)/GP + CR
               var currentPlayerRating = levelGenerated.playerRating;
               var levelRating = lRating;
               var  multiplier = EvaluatePlayerPerformance();
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

               pRating = currentPlayerRating * multiplier;
               return pRating;


       }

       public double AddPlayerRating(int gamesPlayed,int playerRating,double levelRating, int playerScore,bool playerWon)
       {
           var pRating = playerRating; 
           if (gamesPlayed < 1)
           {
               pRating =  (int)((playerRating + (levelRating* .01f) / 1) + (playerScore / 2.5)); // This is based from elo rating of chess

           }
           // This is based from elo rating of chess
           else
           {
               if(playerWon)
                   pRating = (int)((playerRating + (levelRating * .01f) / gamesPlayed) + (playerScore / 2.5f)); // divided by 6 so player Rating will not go up to much
               else
                   pRating = (int)((playerRating + (levelRating * .01f) / gamesPlayed) + (playerScore));

           }
            Debug.Log($"Player Rating Computation: PlayerRating: {playerRating} levelRating: {levelRating}  playerScore: {playerScore} games played: {gamesPlayed}  pRating: {pRating} ");
           return pRating;
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
            var levelStats = SaveManager.Instance.playerProfile.levelsPlayed;
            if (levelStats.Count < 1) return;

            var games = levelStats.Count;
            
            double pClearTime = 0; // average clearTime dfq
            int pMoves = 0; // average moves
            double pScore =0; // average score

            var allottedTime = 0;
            var expectedMoves = 0;
            var score = 0;
            double lRating = 0;

            foreach (var details in levelStats)
            {
                pMoves += details.playerMove;
                pScore += details.playerScore;
                pClearTime += details.playerRemainingTime;
                currentPlayerRating += details.playerRating;
                
                allottedTime += details.allottedTime;
                expectedMoves += details.expectedMoves;
                score += 10;
                lRating += details.levelRating;

            }

            pClearTime /= games;
            pMoves /= games;
            pScore /= games;
            currentPlayerRating /= games;
            
            allottedTime /= games;
            expectedMoves /= games;
            score /= games;
            lRating /= games;

            var sTime = (pClearTime / allottedTime) *.20;
            var sMoves = (expectedMoves / pMoves) *.25;
            var sScore = (pScore / score) *.5;
            var sRating = (currentPlayerRating / lRating) *.05;

            var suggestedRatingMultiplier = sTime + sMoves + sScore + sRating;

                SaveManager.Instance.playerProfile.initialRating = suggestedRatingMultiplier * lRating;
                SaveManager.Instance.playerProfile.currentRating = suggestedRatingMultiplier * lRating;
            
                SaveManager.Instance.SaveProfile();
                UIManager.Instance.ChangeRatingText( SaveManager.Instance.playerProfile.currentRating);
        }
       
       


    }
}