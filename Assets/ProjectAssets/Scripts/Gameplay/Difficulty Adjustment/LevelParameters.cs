using UnityEngine;

namespace ProjectAssets.Scripts.Gameplay.Difficulty_Adjustment
{
    public class LevelParameters
    {
        
        
        private int boardSize = 4; //BS    // set to 4 as default

        public int expectedMoves = 4; //EM // Computation 1<= EM <= (Board Size – 2)
        
        private double allocatedTime = 20; // AT // (BT * (Expected moves / 10)) + BT
        private double boardTime; //BT Computation boardSize * 5
        
        public double expectedScore; // Computation ((EM/PM) * 100% * 60%) + ((70% of AT * SP(70%)) / (RT * PS)) * 40%
        private double completionScore; // Computation EM/PM *100%
        private double timeCompletionScore; // Computation (RT * SP)/(70% of AT * SP(70%)) 

        private double levelRating; // Computation EM /(70% of AT * 70%)*100 // NOTE subject to change
        // handles the parameters
        
        // increment board size when the allowed EM is achieved??????
        public double SetBoardTime() => boardSize * 5;
        double SetCompletionScore(int playerMovement) => (double) expectedMoves / playerMovement;
       public double SetPuzzleRating() => expectedMoves / ((.7 * SetAllocatedTime() * .7)) * 100;

        public int SetBoardSize(int size)
        {
            boardSize = size;
            return boardSize;
        }
        // should be square n*n // will be used in level generator, EM should be 

        public double SetAllocatedTime()
        {
        allocatedTime = (SetBoardTime() * ((double) expectedMoves / 10)) + boardTime;
        return allocatedTime;

        }
        
        public int SetExpectedMoves(int add = 0)
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

        public double SetTimeCompletionScore(double remainingTime)
        {
            var firstEquation = (remainingTime * .7);
            var secondEquation = ((SetAllocatedTime() * .7) * .7); // outer .7 is Score Percentage (SP) // but this is static I guess
            return firstEquation / secondEquation;
        }
        // used to compute for the Expected Score and player Score
        // used twice, to determine the Level's Score and to Compute the Player's Score
        public double SetExpectedScore(int playerMovement,double remainingTime)
        {
            var firstEquation =  (SetCompletionScore(playerMovement) * .6);
            var secondEquation = (SetTimeCompletionScore(remainingTime) * .4);
            expectedScore = ((firstEquation + secondEquation) * 100) / 10;
            return ((firstEquation + secondEquation) * 100) / 10;
            // var thirdEquation = (remainingTime *)
        }

        

        // will be used in the level generator 
 

        // what to do first
        // determine the board size
        // then The Expected Moves
        // then the Time
        // then the Score
        // then the 2nd Score
        // then Rating
        


        

        
        
        



        
        
        
    }
}