using UnityEngine;

namespace ProjectAssets.Scripts.Gameplay.Difficulty_Adjustment
{
    public class DifficultyModifier : MonoBehaviour
    {
        // Script that handles the difficulty of the levels
        // main Parameters
        // the player's current rating
        // level parameters: width height, expected moves, time, 
        public PlayerProfile currentPlayer;
        public LevelDetails levelGenerated;
        public LevelParameters parameters = new LevelParameters();

        void Awake()
        {
            //get currentplayer
        }

        void GetProfile(PlayerProfile profile)
        {
            currentPlayer = profile;
            
        }

       public void SetupDifficultyParameters()
        {
            // Setup Board Size
            parameters.SetBoardSize(4); // Always the initial BoardSize
          var time = parameters.SetAllocatedTime();
            parameters.SetExpectedMoves();
            parameters.SetExpectedScore(parameters.expectedMoves,time); // Expected Score and TIME
            var puzzleRating = parameters.SetPuzzleRating();
            
            Debug.Log($"Puzzle Rating: {puzzleRating}");
            // now create the alloted Time




        }

        void CalculateLevelRating()
        {
            var currentPlayerRating = currentPlayer.currentRating;
            
            
            // first randomly create a level Rating
            // compare playerRating to the level Rating
            // if is not in range reduce or add level rating 


        }
        
        
        
        
        
    }
}