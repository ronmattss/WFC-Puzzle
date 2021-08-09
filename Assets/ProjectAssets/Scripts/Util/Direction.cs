using System;
using System.Collections.Generic;
using ProjectAssets.Scripts.Puzzle_Generation;

namespace ProjectAssets.Scripts.Util
{
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