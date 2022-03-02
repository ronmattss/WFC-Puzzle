/*
 Title: Self Avoiding Walk
 Author: Ron Matthew Rivera
 Sub-System: Part of Puzzle Generation Puzzle Solver
 Date Written/Revised: Aug. 23, 2021
 Purpose: Implements a self avoiding walk algorithm to solve a puzzle
 Data Structures, algorithms, and control: Class, Lists, Stacks,
 */

using System;
using System.Collections.Generic;
using ProjectAssets.Scripts.Puzzle_Generation;
using UnityEngine;

namespace ProjectAssets.Scripts.Gameplay.Pathfinding
{

    [Serializable]
    public class SelfAvoidingWalk
    {
        public Cell startingCell;
        public int steps;
        public CellPath StartPathCell = new CellPath();
        public Stack<CellPath> path = new Stack<CellPath>();
        public List<Cell> completedPath = new List<Cell>();

        private int _giveUp;

        // Get First the starting cell
        public SelfAvoidingWalk(Cell cell, int step)
        {
            startingCell = cell;
            StartPathCell.CurrentCell = startingCell;
            StartPathCell.VisitedPathCells.Add(startingCell);
            steps = step;
            _giveUp = 10000;
        }

        void CheckIfVisited(CellPath cellPath)
        {
            var pathCount = cellPath.VisitedPathCells.Count;
            // meaning empty path
            if (pathCount == 0)
            {
                for (int i = 0; i < cellPath.CurrentCell.neighbors.Length; i++)
                {
                    // add cells to the possible paths if it is not in the visitedPathCells
                    if (cellPath.CurrentCell.neighbors[i] == null) continue; // when neighbor is null
                    cellPath.PossiblePathCells.Add(cellPath.CurrentCell.neighbors[i]);
                }

                return;
            }

            if (cellPath.CurrentCell.neighbors.Length == 0)
                return;
            for (int i = 0; i < cellPath.CurrentCell.neighbors.Length; i++)
            {
                // add cells to the possible paths if it is not in the visitedPathCells
                if (cellPath.CurrentCell.neighbors[i] == null) continue; // when neighbor is null
                if (!MatchCellsInVisitedPath(cellPath, cellPath.CurrentCell.neighbors[i]))
                {
                    cellPath.PossiblePathCells.Add(cellPath.CurrentCell.neighbors[i]);
                }
            }
        }

        // check if cell is in the cell path visited cells
        private bool MatchCellsInVisitedPath(CellPath cellPath, Cell cell)
        {
            // if there are no available paths
            if (cellPath.VisitedPathCells.Count == 0) return false;
            for (int i = 0; i < cellPath.VisitedPathCells.Count; i++)
            {
                if (cellPath.VisitedPathCells[i] == null) continue;
                if (cellPath.VisitedPathCells[i].cellID == cell.cellID)
                {
                    return true;
                }
            }

            // if there are no available paths
            return false;
        }

        public void Walk()
        {
            var currentPath =
                new CellPath(RandomlyAssignNewCell(StartPathCell),
                    StartPathCell.VisitedPathCells); // walk start here now repeat it
            path.Push(currentPath);
            while (steps > 0)
            {
                // RandomlyAssignNewCellAgain
                if (currentPath.CurrentCell == null && path.Count > 0)
                {
                    currentPath = BackTrack();
                }

                // final options
                if (currentPath == null)
                {
                    Walk();
                }

                currentPath = new CellPath(RandomlyAssignNewCell(currentPath), currentPath.VisitedPathCells);
                // what is this, If there is no current path, backtrack
                if (currentPath.CurrentCell == null)
                {
                    var x = 2; // DEBUG ONLY just checking of the possibility 
                    Debug.Log("Help I'm Stuck" + x);
                    // recursive Backtrack here and pop and increment walk
                    currentPath = BackTrack();

                    try
                    {
                        if (currentPath.CurrentCell == null && path.Count == 0)
                        {
                            steps = 0;
                            Debug.Log("Help I'm Stuck Forever");
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);

                    }

                    path.Push(currentPath);
                    steps--;
                }
                else
                {
                    path.Push(currentPath);

                    steps--;
                }
            }

            Debug.Log($"Walk Complete: {path.Count}");
            while (path.Count != 0)
            {
                completedPath.Add(path.Pop().CurrentCell);
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
            var newPath = path.Peek(); //new index[0] previous [1]
            newPath.VisitedPathCells.Add(previousPath.CurrentCell); // add previous cell to visited path

            var redirectedPath = new CellPath(RandomlyAssignNewCell(newPath), newPath.VisitedPathCells);
            if (_giveUp > 0)
                steps++; // increment steps when popping
            _giveUp--; // to force the algo to give up walking  (anti- stack overflow)
            if (redirectedPath.CurrentCell == null ||
                redirectedPath.VisitedPathCells.Contains(redirectedPath
                    .CurrentCell)) // OR if this current cell is in the visited cell then backtrack again
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
            if (cellPath.PossiblePathCells.Count == 0) return null; // there is no possible path, Backtrack?
            var indexOfNextCell =
                UnityEngine.Random.Range(0,
                    cellPath.PossiblePathCells.Count); // Check if this cell is in the visited Cells, if yes return -1
            cellPath.VisitedPathCells.Add(cellPath.CurrentCell);
            return cellPath.PossiblePathCells[indexOfNextCell];
        }
    }
}