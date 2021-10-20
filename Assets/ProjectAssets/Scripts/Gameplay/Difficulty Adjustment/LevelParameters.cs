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
        
        public int SetExpectedMoves()
        {

            // if the ExpectedMoves is beyond the size of the 
            expectedMoves = Random.Range(MinMove(boardSize), (MaxMoves(boardSize))); // this should be random on a range // have a switch statement that handles the max limit of moves per board size
            return expectedMoves;

        }


        private int MaxMoves(int board)
        {
            return board switch
            {
                5 => 21,
                6 => 29,
                7 => 35,
                8 => 50,
                9 => 64,
                10 => 80,
                _ => 12
            };
        }
        private int MinMove(int board)
        {
            return board switch
            {
                4 => 6,
                5 => 14,
                6 => 16,
                7 => 20,
                8 => 30,
                9 => 45,
                10 => 50,
                _ => 12
            };
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
            if (( expectedMoves * .69) >= playerMovement && playerMovement >= (expectedMoves * .50))
            {
                return expectedMoves * .5;
            }
            return result;
        }

        // FUZZY Logic ?
        public double SetPlayerTimeCompletionScore(double remainingTime)
        {
            var playerTime = (remainingTime);
            var levelTime = ((SetAllocatedTime())); // outer .7 is Score Percentage (SP) // but this is static I guess
            
            // these values should be tweaked for board size 6 -10
            if ( playerTime >= (levelTime * .70))
            {
             //   Debug.Log(levelTime * 1);
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
          //  Debug.Log( $"Player Completion Score: {firstEquation} Player Time Score: {secondEquation}");
            

            var result = ((firstEquation + secondEquation) * 100) / 10;
            return result;
            // var thirdEquation = (remainingTime *)
        }
        
        // fuzzy membership functions

        // fuzzy evaluator 
        public double EvaluatePlayerMoves(int playerMoves)
        {
            var baseScore = expectedMoves;
            var pScore = playerMoves;
            
            
            
            if (pScore == baseScore) // perfect moves
            {
                return 1;
            }

            if (pScore < baseScore)
            {
                return -1;
            }

            // 31 <= playerMoves 32 <= 37

            if (pScore >= (baseScore / .99) && pScore <= (baseScore / .8))
            {
                return .25;
                
            }
            if ((pScore >= (baseScore / .79) && pScore <= (baseScore / .5))) // 
            {
                return 0;
            }

            return -.25;
            
        }
        
        public double EvaluatePlayerRemainingTime( double playerRemainingTime)
        {
            var baseTime = allocatedTime;
            var playerTime = playerRemainingTime;
           //  36 >= 32 >= .7
            if (baseTime >= playerTime && playerTime >= (baseTime * .70))
            {
                return 1;
            }
            if (( baseTime * .69) >= playerTime && playerTime >= (baseTime * .50))
            {
                return 0.05;
            }
        
            if (baseTime > 0)
                return -(playerTime/baseTime);

            return 0;
            
        }
        
        

        
    }
}