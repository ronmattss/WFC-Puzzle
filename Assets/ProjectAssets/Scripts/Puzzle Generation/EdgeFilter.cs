/*
 Title: Edge Filter 
 Author: Ron Matthew Rivera
 Sub-System: Core part of Wave Function Collapse Algorithm
 Date Written/Revised: Aug. 15, 2021
 Purpose: Filters the edges of a cell
 Data Structures, algorithms, and control: Class, Lists
 */
namespace ProjectAssets.Scripts.Puzzle_Generation
{

    public class EdgeFilter
    {

        private readonly ConnectionType _filterType;

        private readonly bool _isInclusive; // dk what is this

        private readonly int _edgeDirection; // direction of the edge [bottom right top left]

        // EdgeFilter Constructor
        public EdgeFilter(int edgeDirection, ConnectionType filterType, bool isInclusive)
        {
            _edgeDirection = edgeDirection;
            _filterType = filterType;
            _isInclusive = isInclusive;
        }
        
        //returns true if a module matches this type of filter
        public bool CheckModule(Module module)
        {
            var edge = (_edgeDirection + 2) % 4; // this is a weird equation to get the edge direction but it works
            var match = module.connections[edge] == _filterType; // does this module's edge match with the filter type?

            return _isInclusive ? !match : match;
        }


    }
}