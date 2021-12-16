using System.Collections.Generic;
using System.Linq;
using ProjectAssets.Scripts.Util;
using UnityEngine;

namespace ProjectAssets.Scripts.Puzzle_Generation
{
    
    /// <summary>
    /// Cell class the main component of the puzzle.
    /// This is what the generator generates and where the player moves in game space
    /// </summary>
    public class Cell : MonoBehaviour, IHeapItem<Cell>
    {
        // cell properties
        // Coordinates
        // Neighbors
        // Domain (Possible Modules)
        
        // Modules -> what type of Module this cell will be?
        // >This affects the possible modules of the neighboring cells
        // >Because Modules has edge types meaning neighboring cells will be filtered
        
        // edge type
        
        //[bottom,right,top,left]
        public string cellID;
        public Cell[] neighbors = new Cell[4]; // new neighbors

        public List<Module> possibleModules; // all possible modules list
        public CellGenerator generator;

        public Module module;
        public Direction direction;
        public ConnectionType[] edges = new ConnectionType[4];
        public Vector3 cellPosition;
        public Vector3 cellPositionDisabled;
        
        
        public bool cellOnPosition = true;
        public bool isRotatable = true;
        public bool isDeathCell = false;    // Cell that instantly cause lose condition
        public bool lockRotation = false;
        public bool isSuggestedPath = false;
        
        public int HeapIndex { get; set; }

        void Awake()
        {
            cellID = RandomGeneratedCellName();
            possibleModules = new List<Module>();
            cellPosition = this.gameObject.transform.position;
            cellPositionDisabled = new Vector3(cellPosition.x,cellPosition.y-50,cellPosition.z);
        }
        
        // populate cell with all Modules
        public void GetAllModules()
        {    
            for (var i = 0; i < generator.modules.Count; i++)
            {
                possibleModules.Add(generator.modules[i]);

            }
        }

        public void Collapse()
        {
            // check if the current cell fits to other "collapsed/ finished" neighboring cells
            for (var i = 0; i < neighbors.Length; i++)
            {
                // if neighbor is null or neighbor[i].possibleModules is Greater than 1
                if (neighbors[i] == null || neighbors[i].possibleModules.Count > 1) continue;

                if (possibleModules[0].connections[i] != neighbors[i].possibleModules[0].connections[(i + 2) % 4])
                {
                    Debug.LogError(
                        $"Setting module {possibleModules[0]} would not fit already set neighbour {neighbors[i].gameObject}!",
                        gameObject);
                }

            }
                //Propagate changes to neighbors
                for (var i = 0; i < neighbors.Length; i++)
                {
                    if (neighbors[i] == null) continue;
                    
                    neighbors[i].FilterCell(new EdgeFilter(i,possibleModules[0].connections[i],true));
                   
                }
            
        }

        // method that filters possible modules of each side edge of a cell
        // Applies an EdgeFilter to this cell
        // "Filter" is the filter to be applied
        public void FilterCell(EdgeFilter filter)
        {
            if (possibleModules.Count == 1) return;

            var removingModules = new List<Module>();
            
            // filter possible modules list

            for (var i = 0; i < possibleModules.Count; i++)
            {
                if (filter.CheckModule(possibleModules[i]))
                {
                    removingModules.Add(possibleModules[i]);
                }
            }
            // remove filtered modules
            for (var i = 0; i < removingModules.Count; i++)
            {
                RemoveModule(removingModules[i]);
            }
            
        }

        public void RemoveModule(Module module)
        {
            // remove a module from list of possible modules
            possibleModules.Remove(module);
            
            // update the cell on the heap
            generator.orderedCells.UpdateItem(this);

            for (var i = 0; i < neighbors.Length; i++)
            {
                // only check if has a neighbor on this side
                if (neighbors[i] == null) continue;

                var edgeType = module.connections[i];
                var lastWithEdgeType = true;
                
                // search other possible modules for same edge type
                for (var j = 0; j < possibleModules.Count; j++)
                {
                    if (possibleModules[j].connections[i] == edgeType)
                    {
                        lastWithEdgeType = false;
                        break;
                    }
                }

                if (lastWithEdgeType)
                {
                    // populate edge changes to neighbor cell
                    var edgeFilter = new EdgeFilter(i,edgeType,false);
                    neighbors[i].FilterCell(edgeFilter);
                }
            }
            
        }

        public void SetModule(Module module)
        {
            possibleModules = new List<Module> {module};
            generator.orderedCells.UpdateItem(this);
            
            Collapse();
        }


        /// <summary>
        ///  Should this be useful or not IDK
        /// Compares two cells using their solved score.
        /// TODO: Refactor. Is the extra randomness necessary?
        /// </summary>
        /// <param name="other">Cell to compare.</param>
        /// <returns>Comparison value.</returns>
        public int CompareTo(Cell other)
        {
            var compare = possibleModules.Count.CompareTo(other.possibleModules.Count);
            if (compare == 0)
            {
                var r = Random.Range(1, 3);
                return r == 1 ? -1 : 1;
            }

            return -compare;
        }
        
        
        // Gameplay Methods \\
        // Get edges

        public void SetEdges()
        {
            direction = new Direction(module.connections.ToList());
        }
        
        // Rotate Edges
        
        public Direction RotateLeft() {
            var input = direction.path.ToArray();
            var numOfRotations = 1;
            var length = input.Length;
            for (int i = 0; i < numOfRotations; i++) {

                // take out the first element
                var temp = input[0];
                for (int j = 0; j < length - 1; j++) {

                    // shift array elements towards left by 1 place
                    input[j] = input[j + 1];
                }
                input[length - 1] = temp;
            }
            direction.path = input.ToList();
            return new Direction(input.ToList());

        }
         
        public Direction RotateRight()
        {
            var input = direction.path.ToArray();
            var numOfRotations = 1;
            var length = input.Length;
            
            
            for (int i = 0; i < numOfRotations; i++) {

                // take out the last element
                var temp = input[length - 1];
                for (int j = length - 1; j > 0; j--) {

                    // shift array elements towards right by one place
                    input[j] = input[j - 1];
                }
                input[0] = temp;
            }

            direction.path = input.ToList();
            return new Direction(input.ToList());

        }

        public void EaseToPosition(bool onPosition)
        {
            //if true go to position if not lower
           
            this.transform.LeanMoveY(onPosition ? cellPosition.y : cellPositionDisabled.y, .5f);
            cellOnPosition = !cellOnPosition;
        }

        public string RandomGeneratedCellName()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 20)
                .Select(s => s[Random.Range(0,s.Length)]).ToArray());
        }

        public void RotationLock()
        {
            if (lockRotation)
            {
                isRotatable = false;
            }
        }


        public Color GridColorBasedOnProperties()
        {
            return isRotatable ? Color.green : Color.red;
        }
        
        // this controls the  Cell GameObject

       
    }
}