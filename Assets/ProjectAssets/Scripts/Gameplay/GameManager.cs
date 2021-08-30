using ProjectAssets.Scripts.Gameplay.Difficulty_Adjustment;
using ProjectAssets.Scripts.Gameplay.Pathfinding;
using ProjectAssets.Scripts.Player;
using ProjectAssets.Scripts.Puzzle_Generation;
using ProjectAssets.Scripts.Util;
using TMPro;
using UnityEngine;

namespace ProjectAssets.Scripts.Gameplay
{
    public class GameManager : Singleton<GameManager>

    {
    
        // Script for handling gameplay properties

        [Header("Board Properties")]
        int boardWidth;
        int boardHeight;
       public int solverMoves;

        private Vector3 startPosition;

        public GameObject playerPrefab;
        public GameObject endGoalPrefab; // a prefab for the goal Object 
        

        private GameObject player;
        private GameObject gCellObject;

        public Cell currentCell;
        public Cell endCell;
        Cell[,] activeCells;
     
        public TMP_Text text;
        public TMP_Text cellText;

        [Header("Generator")]
        public DifficultyModifier modifier; // controls the difficulty
        public Solver solver; // controls the path
        public CellGenerator levelGenerator; // controls the board
        public int debugMoves = 9;
     
        /// <summary>
        /// TODO: Movement with direction constrained
        /// TWEAK ALL CELLS AFFECTED BY THE PATH?
        /// Rotation Dilemma: move edge Constraints or just swap it out with other modules?
        /// If player is in goal, create new board
        /// </summary>

        void Awake()
        {
            SaveManager.Instance.TryLoadProfile();
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        
        // TODO: some paths are blocked by a closed block 
        // button to start Generating
        public void GenerateNewLevel()
        {
            modifier.SetupDifficultyParameters();
            solverMoves = modifier.levelGenerated.expectedMoves; // pass the time to the UI and stuff
            solver.expectedMoves = solverMoves;
            levelGenerator.GenerateRandomLevel(modifier.boardSize);
        }

        public void GoalChecker(Cell currentPlayerCell)
        {
            if (endCell.name.Equals(currentPlayerCell.name))
            {
                ObjectSpawner.Instance.generator.GenerateLevel();
            }
        }

        public void SetGoalCell(Cell goalCell)
        {
            endCell = goalCell;

            if (gCellObject == null) // Instantiate a GoalCellObject if not existing
                gCellObject = Instantiate(endGoalPrefab, endCell.transform.position, Quaternion.identity);
            else
                gCellObject.transform.position = endCell.transform.position;

        }
   
        public void SetBoardSize(int x, int y)
        {
            boardHeight = y;
            boardWidth = x;
        }

        public void SetStartPosition(Vector3 sPosition)
        {
            startPosition = sPosition;
            if (player == null)
            {
                player = Instantiate(playerPrefab, startPosition, Quaternion.identity);
            }
        }

        public Cell GetCurrentCell(int x, int y)
        {
            cellText.text = $"Current Cell:{activeCells[x, y].name} ";
            return activeCells[x, y];
        }

        public void SetPlayerPosition()
        {
            
            player.GetComponent<BoardMovement>().SetStartPosition(solver.GetStartCellPosition());
        }

        public void SetActiveCells(Cell[,] cells) => activeCells = cells;
        public int GetBoardHeight() => boardHeight;
        public int GetBoardWidth() => boardWidth;

    }
}
