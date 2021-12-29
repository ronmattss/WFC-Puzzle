using System;
using System.Collections.Generic;
using ProjectAssets.Scripts.Puzzle_Generation;
using UnityEngine;

namespace ProjectAssets.Scripts.Gameplay.Pathfinding
{
    
    /// <summary>
    ///
    /// Saw Algorithm, used to generate a path from start to end without having duplicate points
    /// 
    /// </summary>
    [Serializable]
    public class SelfAvoidingWalk
    {
        public Cell startingCell;
        public int steps;
        public CellPath StartPathCell = new CellPath();
        public Stack<CellPath> path = new Stack<CellPath>();
        public List<Cell> completedPath = new List<Cell>();
        private int giveUp;
        // Get First the starting cell
        public SelfAvoidingWalk(Cell cell,int step)
        {
            startingCell = cell;
            StartPathCell.currentCell = startingCell;
            StartPathCell.visitedPathCells.Add(startingCell);
            steps = step;
            giveUp = 10000;
        }

        void CheckIfVisited(CellPath cellPath)
        {
            var pathCount = cellPath.visitedPathCells.Count;
            // meaning empty path
            if (pathCount == 0)
            {
                for (int i = 0; i < cellPath.currentCell.neighbors.Length; i++)
                {
                    // add cells to the possible paths if it is not in the visitedPathCells
                    if(cellPath.currentCell.neighbors[i] == null) continue; // when neighbor is null
                    cellPath.possiblePathCells.Add(cellPath.currentCell.neighbors[i]);
                }

                return;
            }

            if (cellPath.currentCell.neighbors.Length == 0) 
                return;
            for (int i = 0; i < cellPath.currentCell.neighbors.Length; i++)
            {
                // add cells to the possible paths if it is not in the visitedPathCells
                if(cellPath.currentCell.neighbors[i] == null) continue; // when neighbor is null
                if (!MatchCellsInVisitedPath(cellPath, cellPath.currentCell.neighbors[i]))
                {
                    cellPath.possiblePathCells.Add(cellPath.currentCell.neighbors[i]);
                }
                
            }


        }
        // check if cell is in the cell path visited cells
        bool MatchCellsInVisitedPath(CellPath cellPath, Cell cell)
        {        // if there are no available paths
            if (cellPath.visitedPathCells.Count == 0) return false;
            for (int i = 0; i < cellPath.visitedPathCells.Count;i++)
            {
                if(cellPath.visitedPathCells[i] == null) continue;
                if (cellPath.visitedPathCells[i].cellID == cell.cellID)
                {
                    return true;
                }
            }
            
            // if there are no available paths
            return false;
        }

     public void Walk()
        {
            var currentPath = new CellPath(RandomlyAssignNewCell(StartPathCell),StartPathCell.visitedPathCells); // walk start here now repeat it
            path.Push(currentPath);
            while (steps > 0)
            {
                // RandomlyAssignNewCellAgain
                if (currentPath.currentCell == null && path.Count >0)
                {
                    currentPath = BackTrack();
                }
                // final options
                if (currentPath == null)
                {
                    Walk();
                }
                currentPath = new CellPath(RandomlyAssignNewCell(currentPath),currentPath.visitedPathCells);
                // what is this, If there is no current path, backtrack
                if (currentPath.currentCell == null)
                {
                    var x = 2; // DEBUG ONLY just checking if the possibility 
                    Debug.Log("Help I'm Stuck" + x);
                    // recursive Backtrack here and pop and increment walk
                    currentPath = BackTrack();

                    try
                    {
                        if (currentPath.currentCell == null && path.Count == 0)
                        {
                            steps = 0;
                            Debug.Log("Help I'm Stuck Forever");
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
   //                     Walk();
                    }



                    path.Push(currentPath);
//                    Debug.Log($"Steps-in: {steps } stack: {path.Count}");

                    steps--;

                }
                else
                {
                    path.Push(currentPath);
              //      Debug.Log($"Steps: {steps } Stack Count: {path.Count}");

                    steps--;
                }
                
                

            }

            Debug.Log($"Walk Complete: {path.Count}");
            while (path.Count != 0)
            {
                completedPath.Add(path.Pop().currentCell);
            }

        }



        
        
        // Backtrack
        // pop the top most cell in the stack, place it in the visited cell, get the next cell from the stack
        // check all possible paths
        // if null then do it again
        CellPath BackTrack()
        {
            if (path.Count == 0) return null;
           var previousPath = path.Pop(); //index[0]
           if (path.Count == 0) return null; // Test to check if stack is still getting errors
           var newPath = path.Peek();    //new index[0] previous [1]
           newPath.visitedPathCells.Add(previousPath.currentCell); // add previous cell to visited path
           
          var redirectedPath = new CellPath(RandomlyAssignNewCell(newPath),newPath.visitedPathCells);
          if(giveUp> 0)
                steps++; // increment steps when popping
          giveUp--; // to force the algo to give up walking  (anti- stack overflow)
          if (redirectedPath.currentCell == null || redirectedPath.visitedPathCells.Contains(redirectedPath.currentCell)) // OR if this current cell is in the visited cell then backtrack again
          {
              BackTrack();
          }

          if (redirectedPath == null)
          {
              Debug.Log("Path is Null");
          }
          return redirectedPath;
        }

        Cell RandomlyAssignNewCell(CellPath cellPath)
        {
            // first check if paths are visited
            CheckIfVisited(cellPath);
            // next is to randomly assign new Cell from possible Cells
            if (cellPath.possiblePathCells.Count == 0) return null; // there is no possible path, Backtrack?
          var indexOfNextCell = UnityEngine.Random.Range(0, cellPath.possiblePathCells.Count); // Check if this cell is in the visited Cells, if yes return -1
          cellPath.visitedPathCells.Add(cellPath.currentCell);
          return cellPath.possiblePathCells[indexOfNextCell];


        }


        
        
        


    }
    

    // 
    public class CellPath
    {
        public Cell currentCell;
        public List<Cell> possiblePathCells = new List<Cell>();
        public List<Cell> visitedPathCells = new List<Cell>();

        public CellPath()
        {
            
        }

        public CellPath(Cell cell,List<Cell> visitedCells)
        {
            currentCell = cell;
            visitedPathCells = new List<Cell>(visitedCells); 
            
        }
    }
}