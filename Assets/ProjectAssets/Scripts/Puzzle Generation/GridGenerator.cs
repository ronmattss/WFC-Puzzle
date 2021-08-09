using System.Collections.Generic;
using ProjectAssets.Scripts.Util;
using UnityEngine;

namespace ProjectAssets.Scripts.Puzzle_Generation
{
public class GridGenerator : MonoBehaviour
    {
        public int width;
        public int height;
        public float cellSize;
        
        
        private int[,] gridArray;
        [SerializeField] GameObject cellPrefab; // -> this opens the WFC
        protected Cell[,] cells;




        public void GenerateGrid(CellGenerator cellGenerator)
        {
            gridArray = new int[width,height];
            cells = new Cell[width,height];
            Debug.Log($"{width} {height}");

            GameManager.Instance.SetBoardSize(width,height);
            #region GridCellGeneration
            
            for(int x = 0; x < gridArray.GetLength(0);x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                  //  Debug.Log($"{x} {y}");
                    
                    var cellObj = Instantiate(cellPrefab,GetWorldPosition(x, y) + new Vector3(cellSize, 0, cellSize) * 0.5f, Quaternion.identity,gameObject.transform);

                    cellObj.name = $"{x} {y}";
                    var cellComponent = cellObj.GetComponent<Cell>();
                    cellComponent.generator = cellGenerator;
                    cellComponent.GetAllModules();
                    cells[x, y] = cellComponent;
                    
                    
                    // assign Neighbours

                    if (x > 0)
                    {
                        var leftCell = cells[x - 1, y];
                        cellComponent.neighbors[3] = leftCell; // right side
                        leftCell.neighbors[1] = cellComponent; // left side
                    }

                    if (y > 0)
                    {
                        var bottomCell = cells[x, y - 1];
                        cellComponent.neighbors[0] = bottomCell;
                        bottomCell.neighbors[2] = cellComponent;
                    }
                    
                    
                    
                    
                    // Draws some Guide Lines in the scene
                    Debug.DrawLine(GetWorldPosition(x,y),GetWorldPosition(x,y+1),Color.white,100f);
                    Debug.DrawLine(GetWorldPosition(x,y),GetWorldPosition(x+1,y),Color.white,100f);
                    
                }
            }
            // Draws some Guide Lines in the scene
            Debug.DrawLine(GetWorldPosition(0,height),GetWorldPosition(width,height),Color.white,100f);
            Debug.DrawLine(GetWorldPosition(width,0),GetWorldPosition(width,height),Color.white,100f);
            
            #endregion 
            // ~~Initial Cell Generator~~ \\
            
      
        }

        private Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x,0,y) * cellSize;
        }

        public void GetXY(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.FloorToInt(worldPosition.x / cellSize);
            y = Mathf.FloorToInt(worldPosition.y / cellSize);

        }
        
        // Set Value of

    }
}

