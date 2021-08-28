using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ProjectAssets.Scripts.Gameplay.Difficulty_Adjustment
{
    [Serializable]
    public class LevelParameters
    {
        
        
        public int boardSize = 4; //BS    // set to 4 as default

        public int expectedMoves = 4; //EM // Computation 1<= EM <= (Board Size – 2)
        
        public double allocatedTime = 20; // AT // (BT * (Expected moves / 10)) + BT
        public double boardTime; //BT Computation boardSize * 5
        
        public double expectedScore; // Computation ((EM/PM) * 100% * 60%) + ((70% of AT * SP(70%)) / (RT * PS)) * 40%
        public double completionScore; // Computation EM/PM *100%
        public double timeCompletionScore; // Computation (RT * SP)/(70% of AT * SP(70%)) 

        public double levelRating; // Computation EM /(70% of AT * 70%)*100 // NOTE subject to change
        // handles the parameters
        
        // increment board size when the allowed EM is achieved??????
        public double SetBoardTime()
        {
            return boardSize * 5;
        }



        public double SetPuzzleRating()
        {
            var timePercentage = SetAllocatedTime() * .7;
            Debug.Log($"time percentage: {timePercentage} ");
            Debug.Log($"expected Moves: {expectedMoves} ");

            // y = mx + b
            // where y is the puzzle rating
            // m is the timePercentage
            // x is the expecteMoves
            // b is the boardsize
            var y = (timePercentage * boardSize + expectedMoves) * .1; 
           return /*((timePercentage * boardSize) / (expectedMoves * .7)) * 100*/ y;
        }

        public int SetBoardSize(int size)
        {
            boardSize = size;
            return boardSize;
        }
        // should be square n*n // will be used in level generator, EM should be 

        public int SetAllocatedTime()
        {
            var boardT = SetBoardTime();
        allocatedTime = (boardT * ((double) expectedMoves / 10)) + boardT;
        boardTime = boardT;
       // Debug.Log($"allocated Time: {allocatedTime}");
        return (int) allocatedTime;

        }
        
        public int SetExpectedMoves(int add = 0)
        {
            int minBoard = 4;
            int EM = minBoard; // set the EM to the board size
            int maxRange = boardSize * boardSize;
            // check the current EM
            if(EM < (maxRange - 2))
            {
                expectedMoves = EM + add;
                return EM + add;
            }
            // if the ExpectedMoves is beyond the size of the 
            EM = Random.Range(boardSize, (maxRange-2)); // this should be random on a range // reconfigure in the modifier
            expectedMoves = EM;
            return EM;

        }
        
        public double SetCompletionScore()
        {
            var pMovement = expectedMoves;
            completionScore = (double) expectedMoves / pMovement;
            return completionScore;
        }

        public double SetTimeCompletionScore()
        {
            var firstEquation = ((SetAllocatedTime() * .7)); // outer .7 is Score Percentage (SP) // but this is static I guess // I think this will be tweaked
            var secondEquation = (allocatedTime * .7); 
            
            timeCompletionScore =  firstEquation / secondEquation;
            return timeCompletionScore;
        }
        // used to compute for the Expected Score and player Score
        // used twice, to determine the Level's Score and to Compute the Player's Score
        public double SetExpectedScore()    // ((EM/PM) * 100% * 60%) + ((70% of AT * SP(70%)) / (RT * PS)) * 40%
        {
            var firstEquation =  (completionScore * .6);
        //    Debug.Log($"Completion Score: {firstEquation}");


            var secondEquation = (timeCompletionScore * .4);
        //    Debug.Log($"Time Score: {secondEquation}");
            

            var result = ((firstEquation + secondEquation) * 100) / 10;
            
            return result;
            // var thirdEquation = (remainingTime *)
        }
        
        public double SetPlayerCompletionScore(int playerMovement)
        {

            if (playerMovement < expectedMoves)
                return 0;
            var result  = (double) expectedMoves / playerMovement;
            
            if (playerMovement == expectedMoves)
            {
                return 1;
            }

            if (( expectedMoves * .99) >= playerMovement && playerMovement >= (expectedMoves * .70))
            {
                return expectedMoves * .7;
            }
            if (( expectedMoves * .79) >= playerMovement && playerMovement >= (expectedMoves * .50))
            {
                return expectedMoves * .5;
            }
            return result;
        }

        public double SetPlayerTimeCompletionScore(double remainingTime)
        {
            var playerTime = (remainingTime);
            var levelTime = ((SetAllocatedTime())); // outer .7 is Score Percentage (SP) // but this is static I guess
            
            // these values should be tweaked for board size 6 -10
            if ( playerTime >= (levelTime * .70))
            {
                Debug.Log(levelTime * 1);
                return  1;
            }
            if (( levelTime * .69) >= playerTime && playerTime >= (levelTime * .50))
            {
                return .50;
            }
            if (( levelTime * .49) >= playerTime && playerTime >= (levelTime * .25)) // these values should be tweaked for board size 6 -10
            {
                return .25;
            }

            return remainingTime/levelTime;
        }
        
        public double SetPlayerScore(ref int playerMovement,ref double remainingTime)    // ((EM/PM) * 100% * 60%) + ((70% of AT * SP(70%)) / (RT * PS)) * 40%
        {
            var firstEquation =  (SetPlayerCompletionScore(playerMovement) * .6);



            var secondEquation = (SetPlayerTimeCompletionScore(remainingTime) * .4);
            Debug.Log( $"Player Completion Score: {firstEquation} Player Time Score: {secondEquation}");
            

            var result = ((firstEquation + secondEquation) * 100) / 10;
            return result;
            // var thirdEquation = (remainingTime *)
        }
        
        // fuzzy membership functions

        // fuzzy evaluator 
        public double EvaluatePlayerMoves(int levelMoves,int playerMoves)
        {
            var baseScore = levelMoves;
            var pScore = playerMoves;
            if (pScore == baseScore)        // perfect moves
            {
                return 1;
            }

            // 31 <= playerMoves 32 <= 37
            if (( baseScore / .99) <= pScore && pScore <= (baseScore / .80)) // prolly some detours
            {
                return .25;
            }
            if (( baseScore / .79) <= pScore && pScore <= (baseScore / .50))   // 
            {
                return 0;
            }

            return -1 *(baseScore/pScore);
            
        }
        
        public double EvaluatePlayerRemainingTime(double allottedTime, double playerRemainingTime)
        {
            var baseTime = allottedTime;
            var playerTime = playerRemainingTime;
           //  36 >= 32 >= .7
            if (baseTime >= playerTime && playerTime >= (baseTime * .70))
            {
                return 1;
            }
            if (( baseTime * .69) >= playerTime && playerTime >= (baseTime * .50))
            {
                return 0;
            }
        
            if (baseTime > 0)
                return -1 * (playerTime/baseTime);

            return -.75;
            
        }
        
        
        //create conditions for larger map size moveset time allocated

        

        // will be used in the level generator 
 

        // what to do first
        // determine the board size
        // then The Expected Moves
        // then the Time
        // then the Score
        // then the 2nd Score
        // then Rating    ``    
        


        

        
        
        



        
        
        
    }
}