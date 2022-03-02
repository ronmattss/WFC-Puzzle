using System.Collections.Generic;
using ProjectAssets.Scripts.Puzzle_Generation;

namespace ProjectAssets.Scripts.Gameplay.Pathfinding
{
    public class CellPath
    {
        public Cell CurrentCell;
        public List<Cell> PossiblePathCells = new List<Cell>();
        public List<Cell> VisitedPathCells = new List<Cell>();

        public CellPath()
        {
        }

        public CellPath(Cell cell, List<Cell> visitedCells)
        {
            CurrentCell = cell;
            VisitedPathCells = new List<Cell>(visitedCells);
        }
    }
}