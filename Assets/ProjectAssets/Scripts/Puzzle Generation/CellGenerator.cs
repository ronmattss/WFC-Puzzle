using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using ProjectAssets.Scripts.Gameplay;
using ProjectAssets.Scripts.Tools;
using ProjectAssets.Scripts.Util;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;


namespace ProjectAssets.Scripts.Puzzle_Generation
{
    public class CellGenerator : GridGenerator
    {
        public List<Module> modules;

        public Module startModule;
        public Module endModule;

        public Heap<Cell> orderedCells;

        public int seed;

        [Header("Tools")] 
        public float genSpeed = .15f;
        public int genSize = 5;
        public UnityEvent onApplyConstraints;


            private void Awake()
        {
            // GenerateLevel On Command
            // GenerateLevel();
        }

        public void GenerateRandomLevel(int size)
        {
            height = size;
            width = size;
            GenerateLevel();
        }

        public void GenerateBoard()
        {
            height = GenerateEmptyBoard.Instance.boardSize;
            width = GenerateEmptyBoard.Instance.boardSize;
            StartCoroutine(BoardGeneration());
        }

        IEnumerator BoardGeneration()
        {
            RemoveGrid();
            GenerateGrid(this);
            GameManager.Instance.solver.debugRenderer.enabled = false;
            // then Generate Cells
            var finalSeed = seed != -1 ? seed : Environment.TickCount;

            UnityEngine.Random.InitState(finalSeed);

            // contain all cells into a heap
            orderedCells = new Heap<Cell>(cells.GetLength(0) * cells.GetLength(1));
            for (var i = 0; i < cells.GetLength(0); i++)
            for (var j = 0; j < cells.GetLength(1); j++)
            {
                orderedCells.Add(cells[i, j]);
            }

            var stopwatch = new Stopwatch(); // check exec Time

            //TODO: APPLY CONSTRAINTS
            ApplyInitialConstraints(); //<- Set Start and End Modules Constraints
            // ~~ Main Wave Function Collapse Algorithm ~~\\

            // HAHA WHILE LOOP

            // Loop until
            while (orderedCells.Count > 0)
            {
                // get the first cell in the heap
                var cell = orderedCells.GetFirst();

                if (cell.possibleModules.Count == 1)
                {
                    cell.Collapse(); //collapse the cell
                    // When a Cell is solved remove from Heap

                    orderedCells.RemoveFirst();
                }

                else
                {
                    // set a random module for this cell // can be modified to what module has the lowest entropy cost
                    // this should not happen
                    //  Debug.Log($"Cell that caused an error: {cell.name} number of possible modules: {cell.possibleModules.Count}");
                    // we fix this
                    cell.SetModule(cell.possibleModules[UnityEngine.Random.Range(0, cell.possibleModules.Count)]);
                }
            }


            GameManager.Instance.cellGameObjects.Clear();
            var solvedCells = GameManager.Instance.solver.cellPath;
            foreach (var c in cells)
            {

                var t = c.transform;
                var x = ReturnCellInSolvePathCells(c);

                if (x != null)
                {
                    Instantiate(x.module.moduleGameObject, t.position, x.module.moduleGameObject.transform.rotation, t);
                    c.module = x.module;
                }
                else
                {
                    Instantiate(c.possibleModules[0].moduleGameObject, t.position,
                        c.possibleModules[0].moduleGameObject.transform.rotation, t);
                    c.module = c.possibleModules[0];
                }

                c.SetEdges();
                GameManager.Instance.cellGameObjects.Add(c.gameObject);
                c.gameObject.GetComponent<Cell>().cellOnPosition = false;
                c.gameObject.GetComponent<Cell>().EaseToPosition(!c.gameObject.GetComponent<Cell>().cellOnPosition);
                yield return new WaitForSeconds(genSpeed);
                GameManager.Instance.solver.debugRenderer.enabled = true;
            }

            // foreach (var c in cells)
            // {
            //
            //     var t = c.transform;
            //
            //
            //         Instantiate(c.possibleModules[0].moduleGameObject, t.position, c.possibleModules[0].moduleGameObject.transform.rotation, t);
            //         c.module = c.possibleModules[0];
            //
            //         c.SetEdges();
            //     GameManager.Instance.cellGameObjects.Add(c.gameObject);
            //     // c.gameObject.GetComponent<Cell>().cellOnPosition = false;
            //     // c.gameObject.GetComponent<Cell>().EaseToPosition(!c.gameObject.GetComponent<Cell>().cellOnPosition);
            //

            // }
            CheckGeneratedLevel();


            // GameManager.Instance.SetActiveCells(cells);
            // GameManager.Instance.SetPlayerPosition();
            //
        }


        public void GenerateLevel()
        {
            RemoveGrid();
            GenerateGrid(this); 
            
            // then Generate Cells
            var finalSeed = seed != -1 ? seed : Environment.TickCount;
            
            UnityEngine.Random.InitState(finalSeed);

            // contain all cells into a heap
            orderedCells = new Heap<Cell>(cells.GetLength(0) * cells.GetLength(1));
            for (var i = 0; i < cells.GetLength(0); i++)
            for (var j = 0; j < cells.GetLength(1); j++)
            {
                orderedCells.Add(cells[i, j]);
            }
            
            var stopwatch = new Stopwatch(); // check exec Time
            
            //TODO: APPLY CONSTRAINTS
            ApplyInitialConstraints(); //<- Set Start and End Modules Constraints
            // ~~ Main Wave Function Collapse Algorithm ~~\\
            
            

            // Loop until
            while (orderedCells.Count > 0)
            {
                // get the first cell in the heap
                var cell = orderedCells.GetFirst();
                
                if (cell.possibleModules.Count == 1)
                {
                    cell.Collapse(); //collapse the cell
                    // When a Cell is solved remove from Heap
                    
                    orderedCells.RemoveFirst();
                }

                else
                {
                    // set a random module for this cell // can be modified to what module has the lowest entropy cost
                    // this should not happen
                  //  Debug.Log($"Cell that caused an error: {cell.name} number of possible modules: {cell.possibleModules.Count}");
                    // we fix this
                    cell.SetModule(cell.possibleModules[UnityEngine.Random.Range(0, cell.possibleModules.Count)]);
                }

                
                // CheckLevel


            }
            
            
            // ~~ END WFC ~~ \\
            // instantiate Game Objects
            // Modify Random Open Cells
            // modify Cells
            // all cells in the cell path should check the modules in the solved path
                GameManager.Instance.cellGameObjects.Clear();
                    var solvedCells = GameManager.Instance.solver.cellPath;
            foreach (var c in cells)
            {

                var t = c.transform;
                var x = ReturnCellInSolvePathCells(c);

                if (x != null)
                {
                    Instantiate(x.module.moduleGameObject, t.position, x.module.moduleGameObject.transform.rotation, t);
                    c.module = x.module; 
                }
                else
                {
                    Instantiate(c.possibleModules[0].moduleGameObject, t.position, c.possibleModules[0].moduleGameObject.transform.rotation, t);
                    c.module = c.possibleModules[0]; 
                }

                c.SetEdges();
                GameManager.Instance.cellGameObjects.Add(c.gameObject);
                c.gameObject.GetComponent<Cell>().cellOnPosition = false;
                c.gameObject.GetComponent<Cell>().EaseToPosition(!c.gameObject.GetComponent<Cell>().cellOnPosition);
                
            }
            CheckGeneratedLevel();
            
            GameManager.Instance.SetActiveCells(cells);
            GameManager.Instance.SetPlayerPosition();
//            Debug.Log($"Start Path: {GameManager.Instance.solver.cellPath[GameManager.Instance.solver.cellPath.Count-1].gameObject.transform.position}");
            
            //TODO is 
            // remove all Gameobjects in the cell path with Closed Modules
            // Then Replace it with random modules
            // then place it again in the board 

        }

        private void CheckGeneratedLevel()
        {
            for (var x = 0; x < cells.GetLength(0); x++)
            {
                for (var z = 0; z < cells.GetLength(1); z++)
                {
                    var cell = cells[x, z];
                    var bCell = cell.neighbors[0];
                    var rCell = cell.neighbors[1];

                    if (bCell != null &&
                        cell.possibleModules[0].connections[0] != bCell.possibleModules[0].connections[2])
                        Debug.LogWarning($"CheckGeneratedLevel | ({x}, {z}) not matching with ({x}, {z - 1})");


                    if (rCell != null &&
                        cell.possibleModules[0].connections[1] != rCell.possibleModules[0].connections[3])
                        Debug.LogWarning($"CheckGeneratedLevel | ({x}, {z}) not matching with ({x + 1}, {z})");
                }
            }
        }

        private void ApplyInitialConstraints()
        {
            // apply the Difficulty Constraint here
            // apply the initial constraints
            onApplyConstraints.Invoke();
            // StartGoalConstraint();
            // OutsideGridBorderConstraint();
        }

        public void OutsideGridBorderConstraint()
        {
            // check if this side is outside meaning out of bounds
            // make this edge a BLOCKED edge
            var topFilter = new EdgeFilter(0, ConnectionType.Blocked, true);
            var leftFilter = new EdgeFilter(1, ConnectionType.Blocked, true);
            var bottomFilter = new EdgeFilter(2, ConnectionType.Blocked, true);
            var rightFilter = new EdgeFilter(3, ConnectionType.Blocked, true);

            // filter for top and bottom cells
            // filter bottom and top cells for only border
            for (var i = 0; i < 2; i++)
            {
                var y = i * (height - 1);

                for (var x = 0; x < width; x++)
                {
                    cells[x, y].FilterCell(i == 0 ? bottomFilter : topFilter);
                }
            }

            // filter for the left and right cells
            // filter left and right cells for only border
            for (var i = 0; i < 2; i++)
            {
                var x = i * (width - 1);

                for (var y = 0; y < height; y++)
                {
                    cells[x, y].FilterCell(i == 0 ? leftFilter : rightFilter);
                }
            }
        }

        public void StartGoalConstraint()
        {
            // Instead of StartCell, EndCell then Difficulty constraint for how many tiles then 
            // Goal Constraint
            var startCell = cells[UnityEngine.Random.Range(0, cells.GetLength(0)), UnityEngine.Random.Range(0, cells.GetLength(1) - 1)];
            Cell endCell;

            startCell.SetModule(startModule);
            do
            {
                endCell = cells[UnityEngine.Random.Range(0, cells.GetLength(0)), UnityEngine.Random.Range(1, cells.GetLength(1))];
            } while (endCell == startCell);

            endCell.SetModule(endModule);
//            Debug.Log($"Start Cell: {startCell.name} End Cell: {endCell.name}");
            
            
            // Get starCell Position
            
            GameManager.Instance.currentCell = startCell;
            GameManager.Instance.SetStartPosition(startCell.transform.position);
            GameManager.Instance.solver.InitializeValues(cells,endCell,GameManager.Instance.solverMoves);
            // GameManager.Instance.SetGoalCell(endCell);
            // now collapse it
            
        }
        public void GoalConstraint()
        {
            // Instead of StartCell, EndCell then Difficulty constraint for how many tiles then 
            // Goal Constraint
            var startCell = cells[UnityEngine.Random.Range(0, cells.GetLength(0)), UnityEngine.Random.Range(0, cells.GetLength(1) - 1)];
            Cell endCell;

            startCell.SetModule(startModule);
            do
            {
                endCell = cells[UnityEngine.Random.Range(0, cells.GetLength(0)), UnityEngine.Random.Range(1, cells.GetLength(1))];
            } while (endCell == startCell);

            endCell.SetModule(endModule);
//            Debug.Log($"Start Cell: {startCell.name} End Cell: {endCell.name}");
            
            
            // Get starCell Position
            
            GameManager.Instance.currentCell = startCell;
            GameManager.Instance.SetStartPosition(startCell.transform.position);
            GameManager.Instance.solver.InitializeSolver(cells,endCell,GenerateEmptyBoard.Instance.expectedMoves);
            // GameManager.Instance.SetGoalCell(endCell);
            // now collapse it
            
        }
        private void RemoveGrid()
        {
            foreach (Transform child in gameObject.transform)
            {
                Destroy(child.gameObject);
            }
        }


        private void EndCellConstraint()
        {
            Cell endCell;
            endCell = cells[UnityEngine.Random.Range(0, cells.GetLength(0)), UnityEngine.Random.Range(0, cells.GetLength(1) - 1)];
            
            // Create a path based on the number of expected moves
            
            
        }
        
        // check if this cell is in the solvepath
        private bool MatchCellInSolvePathCells(Cell cell)
        {
            var cellList = GameManager.Instance.solver.cellPath;

            foreach (var c in cellList)
            {
                if (c.cellID == cell.cellID)
                {
                    return true;
                }
            }

            return false;
        }
        
        private Cell ReturnCellInSolvePathCells(Cell cell)
        {
            var cellList = GameManager.Instance.solver.cellPath;

            foreach (var c in cellList)
            {
                if (c.cellID == cell.cellID)
                {
                    return c;
                }
            }

            return null;
        }

    }
}