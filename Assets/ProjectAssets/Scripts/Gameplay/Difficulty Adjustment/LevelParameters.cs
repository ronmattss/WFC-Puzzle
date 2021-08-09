namespace ProjectAssets.Scripts.Gameplay.Difficulty_Adjustment
{
    public class LevelParameters
    {
        
        
        private int boardSize = 4;    // set to 4 as default

        private int expectedMoves = 4; // Computation 1<= EM <= (Board Size – 2)
        private double allocatedTime = 20;
        private double boardTime; // Computation
        private double ExpectedScore; // Computation
        private double completionScore; // Computation EM/PM *100%
        private double timeCompletionScore; // Computation (RT * SP)/(70% of AT * SP(70%)) 

        private double levelRating; // Computation
        // handles the parameters
        
        // increment board size when the allowed EM is achieved??????
        int BoardSize() => boardSize; // should be square n*n
        double SetBoardTime() => boardSize * 5;
    }
}