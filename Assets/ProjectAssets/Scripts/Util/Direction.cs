using System;
using System.Collections.Generic;
using ProjectAssets.Scripts.Puzzle_Generation;

namespace ProjectAssets.Scripts.Util
{
    /// <summary>
    /// Handles the type of connection is on this direction
    /// </summary>
    [Serializable]
    public class Direction
    {
        public List<ConnectionType> path;

        public Direction(List<ConnectionType> path)
        {
            this.path = path;
        }
    }
}