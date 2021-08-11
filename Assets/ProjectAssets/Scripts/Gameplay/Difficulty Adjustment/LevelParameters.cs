using UnityEngine;

namespace ProjectAssets.Scripts.Gameplay.Difficulty_Adjustment
{
    public class LevelParameters
    {
        
        
        private int boardSize = 4; //BS    // set to 4 as default

        private int expectedMoves = 4; //EM // Computation 1<= EM <= (Board Size – 2)
        
        private double allocatedTime = 20; // AT // (BT * (Expected moves / 10)) + BT
        private double boardTime; //BT Computation boardSize * 5
        
        private double expectedScore; // Computation ((EM/PM) * 100% * 60%) + ((70% of AT * SP(70%)) / (RT * PS)) * 40%
        private double completionScore; // Computation EM/PM *100%
        private double timeCompletionScore; // Computation (RT * SP)/(70% of AT * SP(70%)) 

        private double levelRating; // Computation EM /(70% of AT * 70%)*100 // NOTE subject to change
        // handles the parameters
        
        // increment board size when the allowed EM is achieved??????
        int BoardSize() => boardSize; // should be square n*n
        double SetBoardTime() => boardSize * 5;
        double SetAllocatedTime() => (SetBoardTime() * ((double) expectedMoves / 10)) + boardTime;
        
        double SetCompletionScore(int playerMovement) => (double) expectedMoves / playerMovement;

        double SetTimeCompletionScore(double remainingTime)
        {
            var firstEquation = (remainingTime * .7);
            var secondEquation = ((SetAllocatedTime() * .7) * .7); // outer .7 is Score Percentage (SP)
            return firstEquation / secondEquation;
        }
        double SetExpectedScore(int playerMovement,double remainingTime)
        {
            var firstEquation =  (SetCompletionScore(playerMovement) * .6);
            var secondEquation = (SetTimeCompletionScore(remainingTime) * .4);
            expectedScore = ((firstEquation + secondEquation) * 100) / 10;
            return ((firstEquation + secondEquation) * 100) / 10;
            // var thirdEquation = (remainingTime *)
        }


        void SetScorePercentage()
        {
            var scorePercentageRaw = expectedScore;
            // compute the players score to the expected score
            // check if the score falls DO i eed this???
            

        }
        

        int SetExpectedMoves(int add)
        {
            int EM = boardSize; // set the EM to the board size
            int maxRange = boardSize * boardSize;
            
            
            // check the current EM
            if(boardSize <= EM && EM < (maxRange - 2))
            {
                expectedMoves = EM + add;
                return EM + add;
            }
            EM = Random.Range(boardSize, (maxRange-2)); // this should be random on a range // reconfigure in the modifier
            expectedMoves = EM;
            return EM;

        }
        
        
        



        
        
        
    }
}