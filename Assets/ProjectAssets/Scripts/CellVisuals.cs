using System.Collections.Generic;
using ProjectAssets.Scripts.Puzzle_Generation;
using ProjectAssets.Scripts.Util;
using UnityEngine;

namespace ProjectAssets.Scripts
{
    public class CellVisuals : Singleton<CellVisuals>
    {
        private List<Renderer> _cellRenderers = new List<Renderer>();
        private Color _baseWallColor = new Color(9, 18, 91);
        public Gradient pathGradient;

        // Get Cell GameObject.GameObject


        // This will get the current cell's Material
        public void GetCellMesh(Cell cell)
        {
            var gObjectObject = cell.gameObject.transform.GetChild(0); // Get the first 
            var childCount = gObjectObject.transform.childCount;

            for (int i = 0; i < childCount; i++)
            {
                //                Debug.Log($"Current Cell Children: {gObjectObject.GetChild(i).name}");
                _cellRenderers.Add(gObjectObject.GetChild(i).GetComponent<Renderer>());
            }


            switch (childCount)
            {
                case 4:
                    _cellRenderers[3].material.EnableKeyword("_EMISSION");
                    _cellRenderers[3].material.SetColor("_EmissionColor", Color.white);
                    break;
                case 2:
                    break;
            }

            _cellRenderers.Clear();
        }


        public void ChangeGridColor(Cell cell, Color color)
        {
            var gObjectObject = cell.gameObject.transform.GetChild(0); // Get the first 
            var childCount = gObjectObject.transform.childCount;
            switch (childCount)
            {
                case 4:
                    gObjectObject.GetChild(3).GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
                    gObjectObject.GetChild(3).GetComponent<Renderer>().material.SetColor("_EmissionColor", color);
                    break;
                case 2:
                    break;
            }
        }


        // Change Color of Current Cell Walls
        public void ChangeWallColor(Cell cell, Color color)
        {
            var gObjectObject = cell.gameObject.transform.GetChild(0); // Get the first 
            var childCount = gObjectObject.transform.childCount;
            switch (childCount)
            {
                case 4:
                    gObjectObject.GetChild(1).GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
                    gObjectObject.GetChild(1).GetComponent<Renderer>().material.SetColor("_EmissionColor", color);
                    break;
                case 2:
                    break;
            }
        }

        public void ChangeWallColor(Cell cell)
        {
            if (cell.isDeathCell) return;

            var gObjectObject = cell.gameObject.transform.GetChild(0); // Get the first child
            gObjectObject.GetChild(1).GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
            gObjectObject.GetChild(1).GetComponent<Renderer>().material
                .SetColor("_EmissionColor", _baseWallColor * 0.01f);
        }

        public void GradientPath(List<Cell> cellPath)
        {
            for (var index = 0; index < cellPath.Count; index++)
            {
                float gradientMap = Mathf.InverseLerp(cellPath.Count, 0, index);

                var cell = cellPath[index];
                var gObjectObject = cell.gameObject.transform.GetChild(0); // Get the first 
                gObjectObject.GetChild(3).GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
                gObjectObject.GetChild(3).GetComponent<Renderer>().material
                    .SetColor("_EmissionColor", pathGradient.Evaluate(gradientMap));
            }
        }

        public void DeathPath(Cell cell)
        {
            var gObjectObject = cell.gameObject.transform.GetChild(0); // Get the first 
            if (gObjectObject.childCount == 4)
            {
                gObjectObject.GetChild(3).GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
                gObjectObject.GetChild(3).GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.red);
                gObjectObject.GetChild(2).GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
                gObjectObject.GetChild(2).GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.red);
                gObjectObject.GetChild(1).GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
                gObjectObject.GetChild(1).GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.red);
            }
        }


        //Hierarchy of th cell
        // cell itself
        // GameObject Empty
        // GameObject Children Meshes <- this is what we will edit
    }
}