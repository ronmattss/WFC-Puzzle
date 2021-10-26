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
            expectedMoves = moves +1;
            walk = new SelfAvoidingWalk(currentCell,moves);
            walk.Walk();
            Walker();
            GameManager.Instance.modifier.parameters.suggestedPath = cellPath.Count;
            //RandomDirection();
          //  CheckUniqueness();
            
           // DebugShowPath();
            RandomlyChangePathCells();

        }

        void DebugShowPath()
        {
            if (!debugMode) return;
            debugRenderer.enabled = true;
            debugRenderer.positionCount = cellPath.Count;
            for (var i = 0; i < cellPath.Count; i++)
            {
                debugRenderer.SetPosition(i,cellPath[i].transform.position);
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
            cellPath.Clear();            // make sure 
            cellPath = walk.completedPath;
            foreach (var cell in cellPath)
            {
                cell.lockRotation = true;
                cell.isSuggestedPath = true;
            }

            cellPath[cellPath.Count-1].lockRotation = false;
            DebugShowPath();
//            Debug.Log($"Start Path: {GameManager.Instance.solver.cellPath[GameManager.Instance.solver.cellPath.Count-1].gameObject.transform.position}");

            
        }
        
        void RandomDirection()
        {

            cellPath.Clear();            // make sure 
            previousCell = currentCell;
            for (var i = 0; i < expectedMoves; i++)
            {
                 noChance = 4;
                Cell nextCell;
                do
                {
                    var nextNeighbors = NeighborFilter(currentCell);

                    nextCell = nextNeighbors[Random.Range(0, nextNeighbors.Length)];
                    if(noChance == 0) break;
                    noChance--;
                } while (nextCell == null || CellMatcher(nextCell)); // should I add a back tracker?

                cellPath.Add(nextCell);
                previousCell = cellPath[cellPath.Count-1];
                currentCell = nextCell;
                
            }
            
            // repeat until Unique

            if (CheckUniqueness())
            {
                RandomDirection();
            }
            Debug.Log($"Start Path: {GameManager.Instance.solver.cellPath[GameManager.Instance.solver.cellPath.Count-1].gameObject.transform.position}");

            //Get teh current cell, randomly select a neighbor, get it, remove the cell from the list of neighbors for the next cell (meaning you have 3 choices)
            //Queue it, repeat
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
            return cellPath[cellPath.Count-1].gameObject.transform.position;
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
    }

    // move away from the end Cell
    // movement can be  in four direction
    // stop when you reached the expected Moves
}