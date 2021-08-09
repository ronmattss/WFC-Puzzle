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

        void Awake()
        {
            //get currentplayer
        }

        void GetProfile(PlayerProfile profile)
        {
            currentPlayer = profile;
            
        }

        void CalculateLevelRating()
        {
            var currentPlayerRating = currentPlayer.currentRating;
            


        }
        
        
        
    }
}