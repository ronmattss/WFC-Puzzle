using System.Collections.Generic;
using ProjectAssets.Scripts.Gameplay.Difficulty_Adjustment;
using ProjectAssets.Scripts.Gameplay.Pathfinding;
using ProjectAssets.Scripts.Player;
using ProjectAssets.Scripts.Puzzle_Generation;
using ProjectAssets.Scripts.Util;
using ProjectAssets.SFX;
using TMPro;
using UnityEngine;
using UnityTemplateProjects;
using UnityTemplateProjects.UI;
using Random = Unity.Mathematics.Random;

namespace ProjectAssets.Scripts.Gameplay
{
    /// <summary>
    /// Handles the game logic for the game.
    /// 
    /// </summary>
    public class GameManager : Singleton<GameManager>

    {
        // Script for handling gameplay properties

        private int boardWidth;
        private int boardHeight;
        public int solverMoves;


        [Header("Properties")] 
        public Vector3 startPosition;

        public GameObject playerPrefab;
        public GameObject endGoalPrefab; // a prefab for the goal Object 
        public GameObject keyObjectPrefab; // this will be placed in the level which will give access to the goal
        private int initialKeys = 3;
        private int currentKeys;
        private List<GameObject> keysList = new List<GameObject>();

        public GameObject player;
        public GameObject gCellObject;
        public BoardMovement playerMovement;

        public Cell currentCell;
        public Cell endCell;
        private Cell[,] activeCells;

        public TMP_Text text;
        public TMP_Text cellText;

        [Header("Generator")] public DifficultyModifier modifier; // controls the difficulty
        public Solver solver; // controls the path
        public CellGenerator levelGenerator; // controls the board
        public int debugMoves = 9;

        [Header("in-game cell Objects")] public List<GameObject> cellGameObjects = new List<GameObject>();
        // bruteforce tries 
        
        [Header("Build Type")] public bool hasDDA = true;

        /// <summary>
        /// TODO: Movement with direction constrained
        /// TWEAK ALL CELLS AFFECTED BY THE PATH?
        /// Rotation Dilemma: move edge Constraints or just swap it out with other modules?
        /// If player is in goal, create new board
        /// </summary>
        private void Awake()
        {
            SaveManager.Instance.TryLoadProfile();
        }

        public void RemoveObjects()
        {
            Destroy(player.gameObject);
            Destroy(gCellObject.gameObject);

            solver.debugRenderer.enabled = false;
            for (var i = 0; i < keysList.Count; i++) Destroy(keysList[i]);

            keysList.Clear();
        }

        // Update is called once per frame
        private void LateUpdate()
        {
            if (!UIManager.Instance.mainMenuGroup.activeSelf && !UIManager.Instance.postLevelMenuGroup.activeSelf &&
                playerMovement.totalMoves > 0 &&
                UIManager.Instance.inGameUIGroup.activeSelf) // start timer once player moves
            {
                if (modifier.levelGenerated.playerRemainingTime <= 0)
                {
                    modifier.ComputeLevelScore();
                    SaveManager.Instance.SaveProfile();
                    UIManager.Instance.ShowHideinGameUIGroup();
                    UIManager.Instance.ShowHidePostFailedLevelGroup();

                    // ObjectSpawner.Instance.generator.GenerateLevel();
                    playerMovement.totalMoves = 0;
                    RemoveObjectsInPlay();
                }
                else
                {
                    modifier.CountDownTimer();
                }
                if (/*currentKeys == 0 &&*/ playerMovement.totalMoves >= modifier.levelGenerated.expectedMoves-1)
                {
                    LowerGates();
                    ModifyPathOfEndGoal(true);
                }
            }

            if (endCell != null && endCell.cellOnPosition) SetGoalPosition();
        }

        // TODO: some paths are blocked by a closed block 
        // button to start Generating
        public void GenerateNewLevel()
        {
            //DEBUG
            
            modifier.SetupDifficultyParameters();
            solverMoves = modifier.levelGenerated.expectedMoves; // pass the time to the UI and stuff
            solver.expectedMoves = solverMoves;
            levelGenerator.GenerateRandomLevel(modifier.levelGenerated.boardSize);
            UIManager.Instance.ShowHideMainMenuGroup();
            UIManager.Instance.ShowHideinGameUIGroup();
            SoundManager.Instance.PlayGameMusic();
            playerMovement.totalMoves = 0;
        }

        public void PostLevelGenerateNewLevel()
        {
            modifier.SetupDifficultyParameters();
            solverMoves = modifier.levelGenerated.expectedMoves; // pass the time to the UI and stuff
            solver.expectedMoves = solverMoves;
            levelGenerator.GenerateRandomLevel(modifier.boardSize);
            UIManager.Instance.ShowHidePostLevelGroup();
            UIManager.Instance.ShowHideinGameUIGroup();
            playerMovement.totalMoves = 0;
        }
        public void PostLevelFailedGenerateNewLevel()
        {
            modifier.SetupDifficultyParameters();
            solverMoves = modifier.levelGenerated.expectedMoves; // pass the time to the UI and stuff
            solver.expectedMoves = solverMoves;
            levelGenerator.GenerateRandomLevel(modifier.boardSize);
            UIManager.Instance.ShowHidePostFailedLevelGroup();
            UIManager.Instance.ShowHideinGameUIGroup();
            playerMovement.totalMoves = 0;
        }
        public void RegenerateLevelFedGenerateNewLevel()
        {
            //modifier.ComputeLevelScore();
            modifier.SetupDifficultyParameters();
            solverMoves = modifier.levelGenerated.expectedMoves; // pass the time to the UI and stuff
            solver.expectedMoves = solverMoves;
            levelGenerator.GenerateRandomLevel(modifier.boardSize);
            UIManager.Instance.ShowHidePauseMenu();
            UIManager.Instance.ShowHideinGameUIGroup();
            playerMovement.totalMoves = 0;
        }

        // This checks if the current Cell is the end cell
        public void GoalChecker(Cell currentPlayerCell)
        {
            if (endCell.name.Equals(currentPlayerCell.name)) EndGoalAnimation();
        }

        public void EndGoalAnimation()
        {
            playerMovement.gameObject.transform.LeanMoveY(50, 1).setEaseInCirc().setOnComplete(PostEndGoal);
        }

        private void PostEndGoal()
        {
            // Compute the Level
            if (modifier.levelGenerated.playerMove == modifier.levelGenerated.expectedMoves -1) modifier.levelGenerated.playerMove = modifier.levelGenerated.expectedMoves;
            modifier.ComputeLevelScore();
            SaveManager.Instance.SaveProfile();
            UIManager.Instance.ShowHideinGameUIGroup();
            UIManager.Instance.ShowHidePostLevelGroup();
                
            //Stuff Remover
            for (var i = 0; i < cellGameObjects.Count; i++) Destroy(cellGameObjects[i]);
            cellGameObjects.Clear();
            RemoveObjects();
               
                
                
            playerMovement.totalMoves = 0;
        }

        private void RemoveObjectsInPlay()
        {
            for (var i = 0; i < cellGameObjects.Count; i++) Destroy(cellGameObjects[i]);
            cellGameObjects.Clear();
            RemoveObjects();
               
                
                
            playerMovement.totalMoves = 0;
        }

        public void SetGoalCell(Cell goalCell)
        {
            endCell = goalCell;
            endCell.EaseToPosition(false);
            endCell.cellOnPosition = false;
            SetGoalPosition();
            endCell.isRotatable = false;
            // set end goal to the first path
        }

        public void SetGoalPosition()
        {
            endCell.EaseToPosition(true);
            endCell.cellOnPosition = false;
        }

        public void SetBoardSize(int x, int y)
        {
            boardHeight = y;
            boardWidth = x;
        }

        public void SetStartPosition(Vector3 sPosition)
        {
            startPosition = sPosition;
            if (player == null) player = Instantiate(playerPrefab, startPosition, Quaternion.identity);
        }

        public void SetGoalPosition(Vector3 ePosition)
        {
            if (gCellObject != null)
                gCellObject.transform.position = ePosition;
            else
                gCellObject = Instantiate(endGoalPrefab, ePosition, Quaternion.identity);
        }

        public Cell GetCurrentCell(int x, int y)
        {
            cellText.text = $"Current Cell:{activeCells[x, y].name} ";
            return activeCells[x, y];
        }

        // This sets all position lmao
        public void SetPlayerPosition()
        {
            player.GetComponent<BoardMovement>().gameObject.SetActive(true);
            player.GetComponent<BoardMovement>().SetStartPosition(solver.GetStartCellPosition());
            ChangeGridColorOfSolvedPath();
            playerMovement = player.GetComponent<BoardMovement>();
            playerMovement.enabled = true;
            CameraManager.Instance.vCam.transform.gameObject.SetActive(true);
            CameraManager.Instance.vCam.Follow = player.transform;
            CameraManager.Instance.vCam.LookAt = player.transform;
            
            SetGoalPosition(new Vector3(solver.cellPath[0].transform.position.x, .25f,
                solver.cellPath[0].transform.position.z));
            CheckIfKeysPersist();
            RandomlyPlaceKeys();
            SetGoalCell(solver.cellPath[0]); // set goal cell to the first cell in the path
            ModifyPathOfEndGoal(false);
            
            CellVisuals.Instance.GetCellMesh(playerMovement.currentCell);
        }

        private void ChangeGridColorOfSolvedPath()
        {
            CellVisuals.Instance.GradientPath(solver.cellPath);
            // foreach (var cell in solver.cellPath)
            // {
            //     CellVisuals.Instance.ChangeGridColor(cell,Color.yellow);
            // }
        }

        private void ModifyPathOfEndGoal(bool isOpen)
        {
           var pathCount =  endCell.direction.path.Count;
           for (var i = 0; i < pathCount; i++)
               if (isOpen)
                   endCell.direction.path[i] = ConnectionType.Open;
               else
                   endCell.direction.path[i] = ConnectionType.Blocked;
        }

        private void RandomlyPlaceKeys()
        {
            // three 
            var numOfKeys = initialKeys;
            //currentKeys = numOfKeys;
            var cells = new List<Cell>(solver.cellPath);
            cells.Remove(cells[cells.Count - 1]);
            cells.Remove(cells[0]);

            if (cells.Count < 4) return;
            int[] keyIndex = {
                UnityEngine.Random.Range(cells.Count-2,cells.Count-4),
                cells.Count/2,
                UnityEngine.Random.Range(2,4)

            };
            for (var i = 0; i < numOfKeys; i++)
            {
                var index = UnityEngine.Random.Range(0, cells.Count);
                var keyPosition =
                    new Vector3(cells[keyIndex[i]].transform.position.x, .25f, cells[keyIndex[i]].transform.position.z);
                var key = Instantiate(keyObjectPrefab, keyPosition, Quaternion.identity);
                cells.Remove(cells[keyIndex[i]]);
                keysList.Add(key);
            }
            UIManager.Instance.ChangeKeyText(0);
            currentKeys = 0;
        }

       public void DecrementKeys()
        {
            if (currentKeys > 0)
            {
                currentKeys--;
                UIManager.Instance.ChangeKeyText(currentKeys);
            }
        }
        public void IncrementKeys()
        {
           
                currentKeys++;
                playerMovement.forceRotation++;
                UIManager.Instance.ChangeKeyText(currentKeys);
            
        }

        public void RemoveKeys(GameObject key)
        {
            keysList.Remove(key);
            IncrementKeys();
        }

        private void CheckIfKeysPersist()
        {
            if (keysList.Count == 0) return;
            for (var i = 0; i < keysList.Count; i++) Destroy(keysList[i].gameObject);

            keysList.Clear();
            UIManager.Instance.ChangeKeyText(0);
        }

        public void LowerGates()
        {
            var endCellObject = endCell.transform.GetChild(0).GetChild(4);
            endCellObject.LeanMoveY(-100, 0.5f);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

 

        public void SetActiveCells(Cell[,] cells)
        {
            activeCells = cells;
        }

        public int GetBoardHeight()
        {
            return boardHeight;
        }

        public int GetBoardWidth()
        {
            return boardWidth;
        }
    }
}