﻿using System.Collections.Generic;
using ProjectAssets.Scripts.Puzzle_Generation;
using ProjectAssets.Scripts.Util;
using UnityEngine;

namespace ProjectAssets.Scripts
{
    public class CellVisuals : Singleton<CellVisuals>
    {
        
        private List<Renderer> cellRenderers = new List<Renderer>();
         private Color baseWallColor = new Color(9,18,91);
        
        // Get Cell GameObject.GameObject

        
        // This will get the current cell's Material
        public void GetCellMesh(Cell cell)
        {
            var gObjectObject = cell.gameObject.transform.GetChild(0); // Get the first 
            var childCount = gObjectObject.transform.childCount;

            for (int i = 0; i < childCount; i++)
            {
                Debug.Log($"Current Cell Children: {gObjectObject.GetChild(i).name}");
                cellRenderers.Add(gObjectObject.GetChild(i).GetComponent<Renderer>());
            }


            switch (childCount)
            {
                case 4: 
                    cellRenderers[3].material.EnableKeyword("_EMISSION");
                    cellRenderers[3].material.SetColor("_EmissionColor",Color.white);
                    break;
                case 2:
                    break;
                
            }
           

            // visualsGameObject = gObjectObject.transform.GetChildren
            Debug.Log($"Child Count{childCount}");
            
            cellRenderers.Clear();
        }


        public void ChangeGridColor(Cell cell, Color color)
        {
            var gObjectObject = cell.gameObject.transform.GetChild(0); // Get the first 
            var childCount = gObjectObject.transform.childCount;
            switch (childCount)
            {
                case 4: 
                    gObjectObject.GetChild(3).GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
                  // var oldColor =  gObjectObject.GetChild(3).GetComponent<Renderer>().material.GetColor("_EmissionColor");
                    // Vector3 rgbNew = new Vector3(color.r,color.g,color.b);
                    // Vector3 rgbOld = new Vector3(oldColor.r,oldColor.g,oldColor.b);
                    // var x = new Color(
                    //     LeanTween.easeInSine(rgbOld.x, rgbNew.x, 0.25f),
                    //     LeanTween.easeInSine(rgbOld.y, rgbNew.y, 0.25f),
                    //     LeanTween.easeInSine(rgbOld.z, rgbNew.z, 0.25f));
                    gObjectObject.GetChild(3).GetComponent<Renderer>().material.SetColor("_EmissionColor",color );
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
                    // var oldColor =  gObjectObject.GetChild(3).GetComponent<Renderer>().material.GetColor("_EmissionColor");
                    // Vector3 rgbNew = new Vector3(color.r,color.g,color.b);
                    // Vector3 rgbOld = new Vector3(oldColor.r,oldColor.g,oldColor.b);
                    // var x = new Color(
                    //     LeanTween.easeInSine(rgbOld.x, rgbNew.x, 0.25f),
                    //     LeanTween.easeInSine(rgbOld.y, rgbNew.y, 0.25f),
                    //     LeanTween.easeInSine(rgbOld.z, rgbNew.z, 0.25f));
                    gObjectObject.GetChild(1).GetComponent<Renderer>().material.SetColor("_EmissionColor",color);
                    break;
                case 2:
                    break;
                
            }
        }
        public void ChangeWallColor(Cell cell)
        {
            var gObjectObject = cell.gameObject.transform.GetChild(0); // Get the first 
           // var childCount = gObjectObject.transform.childCount;

                    gObjectObject.GetChild(1).GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
                    // Vector3 rgbNew = new Vector3(color.r,color.g,color.b);
                    // Vector3 rgbOld = new Vector3(oldColor.r,oldColor.g,oldColor.b);
                    // var x = new Color(
                    //     LeanTween.easeInSine(rgbOld.x, rgbNew.x, 0.25f),
                    //     LeanTween.easeInSine(rgbOld.y, rgbNew.y, 0.25f),
                    //     LeanTween.easeInSine(rgbOld.z, rgbNew.z, 0.25f));
                    gObjectObject.GetChild(1).GetComponent<Renderer>().material.SetColor("_EmissionColor",baseWallColor * 0.01f);

                
            
        }
        
        
        //Hierarchy of th cell
        // cell itself
            // GameObject Empty
                // GameObject Children Meshes <- this is what we will edit
    }
}