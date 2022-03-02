/*
 Title: Solver
 Author: Ron Matthew Rivera
 Sub-System: Part of Puzzle Generation Puzzle Solver
 Date Written/Revised: Aug. 23, 2021
 Purpose: Uses the SAW algorithm to solve the puzzle
 Data Structures, algorithms, and control: Class, Lists, Stacks,
 */
using System;
using System.Collections.Generic;
using System.Linq;
using ProjectAssets.Scripts.Gameplay.Difficulty_Adjustment;
using ProjectAssets.Scripts.Puzzle_Generation;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ProjectAssets.Scripts.Gameplay.Pathfinding
{

    public class Solver : MonoBehaviour
    {
        private Cell[,] currentCells;
        public List<Cell> cellPath = new List<Cell>();
        private Cell currentCell;
        public int expectedMoves;
        private Cell previousCell;
        private int noChance = 4;
        public List<Module> selectedRandomCells = new List<Module>();
        public Module endGoalModule;
        public LineRenderer debugRenderer;
        public SelfAvoidingWalk walk;

        public bool debugMode = false;

        // Self Avoiding Walk


        public void InitializeValues(Cell[,] cells, Cell endCell, int moves)
        {
            currentCells = cells;
            currentCell = endCell;
            expectedMoves = moves + 1;
            walk = new SelfAvoidingWalk(currentCell, moves);
            walk.Walk();
            Walker();
            
            
            GameManager.Instance.modifier.parameters.suggestedPath = cellPath.Count;
            
            //RandomDirection();
            //  CheckUniqueness();

            // DebugShowPath();
            RandomlyChangePathCells();
            

        }

        // used in tools
        public void InitializeSolver(Cell[,] cells, Cell endCell, int moves)
        {
            currentCells = cells;
            currentCell = endCell;
            expectedMoves = moves + 1;
            walk = new SelfAvoidingWalk(currentCell, moves);
            walk.Walk();
            Walker();
            GameManager.Instance.modifier.parameters.suggestedPath = cellPath.Count;
            //RandomDirection();
            //  CheckUniqueness();

            DebugShowPath();
            RandomlyChangePathCells();

        }


        void DebugShowPath()
        {
            debugRenderer.positionCount = cellPath.Count;
            for (var i = 0; i < cellPath.Count; i++)
            {
                debugRenderer.SetPosition(i, cellPath[i].transform.position);
            }
        }


        bool CheckUniqueness()
        {
            int similarCount = 0;
            var path = cellPath;
            for (int i = 0; i < path.Count; i++)
            {
                for (int j = 0; j < path.Count; j++)
                {
                    if (path[i] == cellPath[j] && i != j)
                    {
                        similarCount++;
                        //      cellPath.Remove(cellPath[i]);
                    }
                }
            }

            Debug.Log($"Similar paths: {similarCount}");
            return similarCount > 0;
        }

        // Randomly selects cells for the next path
        // should be a random walker with back tracking?

        void Walker()
        {
            cellPath.Clear(); // make sure 
            cellPath = walk.completedPath;
            foreach (var cell in cellPath)
            {
                cell.lockRotation = true;
                cell.isSuggestedPath = true;
            }
            Debug.Log($"Cell path Count: ${cellPath.Count}");
            cellPath[cellPath.Count - 1].lockRotation = false;
            DebugShowPath();
//            Debug.Log($"Start Path: {GameManager.Instance.solver.cellPath[GameManager.Instance.solver.cellPath.Count-1].gameObject.transform.position}");
        }

        


        Boolean CellMatcher(Cell cCell)
        {
            if (cellPath.Count > 2) // only Check if it contains more than 2 cells
            {
                if (cCell == previousCell) // Check IF next Cell is same with previousCell
                {
                    return true;
                }

                for (var index = 1; index < cellPath.Count - 1; index++)
                {
                    var pathCell = cellPath[index];
                    if (cCell == pathCell)
                        return true;
                    if (pathCell.neighbors.Any(t => cCell == t))
                    {
                        return true;
                    }
                }
            }

            noChance = 0;
            return false;
        }

        private Cell[] NeighborFilter(Cell cell)
        {
            var possibleNeighbors = new List<Cell>();


            for (var i = 0; i < cell.neighbors.Length; i++)
            {
                if (cell.neighbors[i] != null)
                {
                    possibleNeighbors.Add(cell.neighbors[i]);
                }
            }

            return possibleNeighbors.ToArray();
        }

        public Vector3 GetStartCellPosition()
        {
//            Debug.Log($"Last Cell path: {cellPath[cellPath.Count-1].gameObject.transform.position}");
            return cellPath[cellPath.Count - 1].gameObject.transform.position;
        }

        // Why? Since WFC generates a good symmetric rule based world, some parts of the puzzle is kinda super symmetric
        // but will this destory the purpose of WFC? actually no
        // this will make the player know that SOME of the solver's Path is better than randomly jus moving in the map, so randoming some tiles can be beneficial
        public void RandomlyChangePathCells()
        {
            for (int i = 0; i < cellPath.Count; i++)
            {
                cellPath[i].module = selectedRandomCells[Random.Range(0, selectedRandomCells.Count)];
//                Debug.Log($"Cell at index {i} is changed");
            }

            // cell 0 should be open
            cellPath[0].module = endGoalModule;
        }

        public void RandomDeathCell(List<GameObject> cells, int percent = 50)
        {
            // check if cell is in cellPath
            var listOfDeathCells = 0;
            var listOfCells =cells;
            
            // check if cell is in cellPath
            for (int i = 0;i< listOfCells.Count; i++)
            {
                if (!MatchCells(listOfCells[i].GetComponent<Cell>()))
                {
                    var randomNumber = Random.Range(0, 100);
                    listOfCells[i].GetComponent<Cell>().isDeathCell = randomNumber < percent;
                    CellVisuals.Instance.DeathPath( listOfCells[i].GetComponent<Cell>());
                    listOfDeathCells++;
                }

            }
            Debug.Log($"Death Cells: {listOfDeathCells}");
            

        }

        bool MatchCells(Cell cell)
        {
            for(int i =0; i< cellPath.Count; i++)
            {
                if (cellPath[i].cellID == cell.cellID)
                {
                    return true;
                }
            }
            return false;
        }
    }

    // move away from the end Cell
    // movement can be  in four direction
    // stop when you reached the expected Moves
}