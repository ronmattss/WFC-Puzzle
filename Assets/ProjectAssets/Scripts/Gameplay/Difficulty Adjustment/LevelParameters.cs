namespace ProjectAssets.Scripts.Gameplay.Difficulty_Adjustment
{
    public class LevelParameters
    {
        
        
        private int boardSize = 4; //BS    // set to 4 as default

        private int expectedMoves = 4; //EM // Computation 1<= EM <= (Board Size – 2)
        
        private double allocatedTime = 20; // AT // (BT * (Expected moves / 10)) + BT
        private double boardTime; //BT Computation boardSize * 5
        
        private double ExpectedScore; // Computation ((EM/PM) * 100% * 60%) + ((70% of AT * SP(70%)) / (RT * PS)) * 40%
        private double completionScore; // Computation EM/PM *100%
        private double timeCompletionScore; // Computation (RT * SP)/(70% of AT * SP(70%)) 

        private double levelRating; // Computation EM /(70% of AT * 70%)*100 // NOTE subject to change
        // handles the parameters
        
        // increment board size when the allowed EM is achieved??????
        int BoardSize() => boardSize; // should be square n*n
        double SetBoardTime() => boardSize * 5;
    }
}