/*
 Title: Level Parameters
 Author: Ron Matthew Rivera
 Sub-System: Part of Dynamic Difficulty Adjustment System
 Date Written/Revised: Nov. 15, 2021
 Purpose: Class that holds all the parameter calculation for a level
 Data Structures, algorithms, and control: Class. Lists
 */
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
        public int suggestedPath; // number of cells suggested by the solver

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
            if (expectedMoves <= 20)
            {
                return boardSize * 2.15;
            }
            if (expectedMoves <= 30)
            {
                return boardSize * 2.15;
            }

            if (expectedMoves <= 50)
            {
                return boardSize * 2.25;
            }

            if (expectedMoves <= 60)
            {
                return boardSize * 2.75;
            }

            return boardSize * 2.75;

        }
        
        // puzzle rating scoring
        public double SetPuzzleRating()
        {
            var timePercentage = SetAllocatedTime() * .7;
            Debug.Log($"time percentage: {timePercentage} ");
            Debug.Log($"expected Moves: {expectedMoves} ");

            // y = mx + b
            // where y is the puzzle rating
            // m is the timePercentage
            // x is the expectedMoves
            // b is the boardsize
            
            var y = (timePercentage * boardSize + expectedMoves) * .15; 
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
        public int SetAllocatedTime(double moves,int bSize)
        {
            var boardT = bSize * 3;
            allocatedTime = boardT * ((double) moves / 10) + boardT;
            boardTime = boardT;
            // Debug.Log($"allocated Time: {allocatedTime}");
            return (int) allocatedTime;

        }
        
        public int SetExpectedMoves()
        {

            // if the ExpectedMoves is beyond the size of the 
            // instead of bounded numbers, use rules from a crisp Output
            expectedMoves = Random.Range(MinMove(boardSize), MaxMoves(boardSize)); // this should be random on a range // have a switch statement that handles the max limit of moves per board size
            return expectedMoves;

        }


        public int MaxMoves(int board)
        {
            return board switch
            {
                5 => 18,
                6 => 29,
                7 => 38,
                8 => 52,
                9 => 62,
                10 => 70,
                _ => 10
            };
        }
        private int MinMove(int board)
        {
            return board switch
            {
                4 => 5,
                5 => 12,
                6 => 25,
                7 => 30,
                8 => 45,
                9 => 50,
                10 => 60,
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
            var firstEquation = SetAllocatedTime() * .7; // outer .7 is Score Percentage (SP) // but this is static I guess // I think this will be tweaked
            var secondEquation = allocatedTime * .7; 
            
            timeCompletionScore =  firstEquation / secondEquation;
            return timeCompletionScore;
        }
        // used to compute for the Expected Score and player Score
        // used twice, to determine the Level's Score and to Compute the Player's Score
        public double SetExpectedScore()    // ((EM/PM) * 100% * 60%) + ((70% of AT * SP(70%)) / (RT * PS)) * 40%
        {
            var firstEquation =  completionScore * .6;
        //    Debug.Log($"Completion Score: {firstEquation}");


            var secondEquation = timeCompletionScore * .4;
        //    Debug.Log($"Time Score: {secondEquation}");
            

            var result = (firstEquation + secondEquation) * 100 / 10;
            
            return result;
            // var thirdEquation = (remainingTime *)
        }

        private double SetComputeSuggestedPathScore(int suggestedMovePlayerPath)
        {
            var sPath = suggestedPath;
            var pPath = suggestedMovePlayerPath;
            var extra = 0;
            if (sPath < expectedMoves)
            {
                sPath = expectedMoves;
                extra = expectedMoves - sPath;
                pPath += extra;
            }

            return (double) pPath / sPath;

            // the goal of sPath is to guide the player to the exit

        }
        
        private double SetComputeSuggestedPathScore(int suggestedMovePlayerPath,int eMoves,int sugPath)
        {
            var sPath = sugPath;
            var pPath = suggestedMovePlayerPath;
            var extra = 0;
            if (sPath < eMoves)
            {
                sPath = eMoves;
                extra = eMoves - sPath;
                pPath += extra;
            }

            return (double) pPath / sPath;

            // the goal of sPath is to guide the player to the exit

        }
        
        
        
        public double SetPlayerCompletionScore(int playerMovement)
        {

            if (playerMovement < expectedMoves)
                return 0;
            var result  = (double)-(expectedMoves / playerMovement);
            
            if (playerMovement == expectedMoves) return 1;

            return result;
        }        public double SetPlayerCompletionScore(int playerMovement,int eMoves)
        {

            if (playerMovement < eMoves)
                return 0;
            var result  = (double) eMoves / playerMovement;
            
            if (playerMovement == eMoves) return 1;

            return result;
        }

        
        public double SetPlayerTimeCompletionScore(double remainingTime)
        {
            var playerTime = remainingTime;
            var levelTime = SetAllocatedTime(); // outer .7 is Score Percentage (SP) // but this is static I guess
            
            // these values should be tweaked for board size 6 -10
            if ( playerTime >= levelTime * .60)
            //   Debug.Log(levelTime * 1);
                return  1;
            if (levelTime * .59 >= playerTime && playerTime >= levelTime * .40) return .50;
            if (levelTime * .39 >= playerTime && playerTime >= levelTime * .25) // these values should be tweaked for board size 6 -10
                return .25;

            return remainingTime/levelTime;
        }


        
        
        public double SetPlayerScore(ref int playerMovement,ref double remainingTime, int playerPathMovement)    // ((EM/PM) * 100% * 60%) + ((70% of AT * SP(70%)) / (RT * PS)) * 40%
        {
            var pathMoveScore = SetComputeSuggestedPathScore(playerPathMovement) * .5;
            var pMovement = SetPlayerCompletionScore(playerMovement) * .5;
            var moveRes = pathMoveScore + pMovement;
            
            var firstEquation =  moveRes * .6;



            var secondEquation = SetPlayerTimeCompletionScore(remainingTime) * .4;
          //  Debug.Log( $"Player Completion Score: {firstEquation} Player Time Score: {secondEquation}");
            

            var result = (firstEquation + secondEquation) * 100 / 10;
            return result;
            // var thirdEquation = (remainingTime *)
        }
        public double SetScoreOfPlayer( int playerMovement,int eMoves, double remainingTime, int playerPathMovement)    // ((EM/PM) * 100% * 60%) + ((70% of AT * SP(70%)) / (RT * PS)) * 40%
        {
            var pathMoveScore = SetComputeSuggestedPathScore(playerPathMovement,eMoves,playerPathMovement) * .5;
            var pMovement = SetPlayerCompletionScore(playerMovement) * .5;
            var moveRes = pathMoveScore + pMovement;
            
            var firstEquation =  moveRes * .6;



            var secondEquation = SetPlayerTimeCompletionScore(remainingTime) * .4;
            //  Debug.Log( $"Player Completion Score: {firstEquation} Player Time Score: {secondEquation}");
            

            var result = (firstEquation + secondEquation) * 100 / 10;
            return result;
            // var thirdEquation = (remainingTime *)
        }
        
 
        public double EvaluatePlayerMoves(int playerMoves)
        {
            var baseScore = expectedMoves;
            var pScore = playerMoves;
            
            
            
            if (pScore == baseScore) // perfect moves
                return 1;

            if (pScore < baseScore) return -1;

            // 31 <= playerMoves 32 <= 37

            if (pScore >= baseScore / .99 && pScore <= baseScore / .8) return .25;
            if (pScore >= baseScore / .79 && pScore <= baseScore / .5) // 
                return 0;

            return -.25;
            
        }
        
        public double EvaluatePlayerRemainingTime( double playerRemainingTime)
        {
            var baseTime = allocatedTime;
            var playerTime = playerRemainingTime;
           //  36 >= 32 >= .7
            if (baseTime >= playerTime && playerTime >= baseTime * .60) return 1;
            if (baseTime * .59 >= playerTime && playerTime >= baseTime * .40) return 0.75; // .05
            if (baseTime * .39 >= playerTime && playerTime >= baseTime * .10) return 0.5; // .05

            if (baseTime > 0)
                return -(playerTime/baseTime);

            return 0;
            
        }
        
        
        
        

        
    }
}