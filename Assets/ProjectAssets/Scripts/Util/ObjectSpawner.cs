using System;
using ProjectAssets.Scripts.Puzzle_Generation;
using UnityEngine;

namespace ProjectAssets.Scripts.Util
{
    public class ObjectSpawner : Singleton<ObjectSpawner>
    {
        public GameObject testObject;
        public GameObject objectParent;
        public CellGenerator generator;

        public void SpawnObject(Vector3 worldPosition)
        {
            if (objectParent == null)
            {
                Instantiate(testObject, worldPosition, Quaternion.identity);
            }
            else
            {
                Instantiate(testObject, worldPosition, Quaternion.identity,objectParent.transform);
            }
        }

        private void LateUpdate()
        {
            if (Input.GetKeyUp(KeyCode.R))
            {
                generator.GenerateLevel();
            }
        }
    }
}