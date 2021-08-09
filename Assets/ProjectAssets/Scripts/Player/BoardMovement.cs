using System;
using System.Collections.Generic;
using System.Linq;
using ProjectAssets.Scripts.Puzzle_Generation;
using ProjectAssets.Scripts.Util;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ProjectAssets.Scripts.Player
{
    public class BoardMovement : MonoBehaviour
    {
        // Start is called before the first frame update

        private int boardX;
        private int boardY;
        private Vector3 currPosition;
        public Cell currentCell;
        private float x = 0;
        private float y = 0;
        private Transform cellObjectTransform;
        public Direction availablePath;

        private void Start()
        {
            var currPosition = gameObject.transform.position;
            x = currPosition.x;
            y = currPosition.z;
            boardX = GameManager.Instance.GetBoardWidth();
            boardY = GameManager.Instance.GetBoardHeight();
            MovePlayer();
        }

        // Update is called once per frame
        private void LateUpdate()
        {
            PlayerMovement();
            Rotate();
        }

        public void SetStartPosition(Vector3 pos)
        {
            x = pos.x;
            y = pos.z;
            currentCell = GameManager.Instance.GetCurrentCell((int) Math.Floor(x), (int) Math.Floor(y));
            cellObjectTransform = currentCell.gameObject.transform; // get the direction this is a culprit
            availablePath.path = new List<ConnectionType>(4);
            availablePath.path.Add(ConnectionType.Open);
            availablePath.path.Add(ConnectionType.Open);
            availablePath.path.Add(ConnectionType.Open);
            availablePath.path.Add(ConnectionType.Open);

            CheckAvailablePath();           
            MovePlayer();
        }

// should it lerp or teleport
        private void PlayerMovement()
        {
            // Get Current Cell
            // Get axis or input button?
            //Debug.Log($"current Cell: {currentCell} Edge: Top: {currentCell.module.connections[2]} Bottom: {currentCell.module.connections[0]} Left: {currentCell.module.connections[3]} Right: {currentCell.module.connections[1]}");
            if (Input.GetKeyDown(KeyCode.W))
            {
                GameManager.Instance.text.text = $"Pressed: W";
                if (availablePath.path[2] == ConnectionType.Open)
                {
                    y++;
                    MovePlayer();
                }
            }
            // else if to avoid multiple inputs
            else if (Input.GetKeyDown(KeyCode.S))
            {
                GameManager.Instance.text.text = $"Pressed: S";
                if (availablePath.path[0] == ConnectionType.Open)
                {
                    y--;
                    MovePlayer();
                }
            }

            else if (Input.GetKeyDown(KeyCode.A))
            {
                GameManager.Instance.text.text = $"Pressed: A";
                if (availablePath.path[3] == ConnectionType.Open)
                {
                    x--;
                    MovePlayer();
                }
            }

            else if (Input.GetKeyDown(KeyCode.D))
            {
                GameManager.Instance.text.text = $"Pressed: D";
                if (availablePath.path[1] == ConnectionType.Open)
                {
                    x++;
                    MovePlayer();
                }
            }
        }

        public void MovePlayer()
        {
            if (boardX > x && x >= 0 && boardY > y && y >= 0)
            {
                currPosition = new Vector3(x, 1, y);
                gameObject.transform.position = currPosition;
                currentCell = GameManager.Instance.GetCurrentCell((int) Math.Floor(x), (int) Math.Floor(y));
             //   availablePath = currentCell.direction; // <- culprit??? available path is copied to the current cell from the prev cell after moving
                CheckAvailablePath();
                cellObjectTransform = currentCell.gameObject.transform;
                GameManager.Instance.GoalChecker(currentCell);
            }
            else
            {
                var position = gameObject.transform.position;
                x = position.x;
                y = position.z;
            }
        }

        // Instead of rotating current cell, rotate neighbor cells
        // assign the neighbors edge to the available path
        // 
        public void Rotate()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                // Debug.Log($"old: {availablePath.path[0]} {availablePath.path[1]} {availablePath.path[2]} {availablePath.path[3]}");
                //  Debug.Log($"new: {availablePath.path[0]} {availablePath.path[1]} {availablePath.path[2]} {availablePath.path[3]}");

                RotateCell(-90);
                CheckAvailablePath();

            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                // Debug.Log($"old: {availablePath.path[0]} {availablePath.path[1]} {availablePath.path[2]} {availablePath.path[3]}");
                // Debug.Log($"new: {availablePath.path[0]} {availablePath.path[1]} {availablePath.path[2]} {availablePath.path[3]}");
                RotateNeighborCells(90);
                CheckAvailablePath();

            }

        }

        
        // also get the cell that will be rotated
        private void RotateCell(int degrees)
        {
            var cellQuaternion = cellObjectTransform.rotation;
            var cellEuler = cellObjectTransform.rotation.eulerAngles;
            cellEuler += new Vector3(0, degrees, 0);
            cellQuaternion.eulerAngles = cellEuler;
            cellObjectTransform.rotation = cellQuaternion;
             currentCell.RotateRight(); //<- probably culprit
            // If Available Path is open check if neighbor path is Open
            // if Open player can traverse if not then don't lol
        }

        private void RotateNeighborCells(int degrees)
        {
            var holdPath = currentCell.direction;
            for (int i = 0; i < currentCell.neighbors.Length; i++)
            {
                // Get object rotation
                if (currentCell.neighbors[i] != null)
                {
                    var neighborRotation = currentCell.neighbors[i].transform.rotation;
                    var neighborRotationEulerAngles = currentCell.neighbors[i].transform.rotation.eulerAngles;
                    neighborRotationEulerAngles += new Vector3(0, degrees, 0);
                    neighborRotation.eulerAngles = neighborRotationEulerAngles;
                    currentCell.neighbors[i].transform.rotation = neighborRotation;
                    currentCell.neighbors[i].RotateLeft();
                }
            }

            currentCell.direction = holdPath; // <- culprit?
        }

        // bottom right top left
        // 0 1 2 3
        // when you rotate the current cell AND neighboring cell, available path should be updated
        // TODO: FIX BLOCKING OF OTHER CELLS WEIRD SHIT When Moving the next CEll's neighbor is not checked so the path remains same
        // IF PLAYER MOVE THEN Rotate neighbors the current cell also adjust which is dapat hindi
        // something is wrong with the rotate neighbors
        // what happens is that if that is pressed the current cell also changes but IF the player rotates current cell before that it does not change
        // Directions takes the the value of the next cell 
        private void CheckAvailablePath()
        {
          //  Debug.Log($"neighbor at the bottom: {currentCell.neighbors[0].direction.path[2]} right: {currentCell.neighbors[1].direction.path[3]} top: {currentCell.neighbors[2].direction.path[0]} left: {currentCell.neighbors[3].direction.path[1]}");
            
            if (currentCell.neighbors[0] != null && 
                currentCell.direction.path[0] == ConnectionType.Open && 
                currentCell.neighbors[0].direction.path[2] == ConnectionType.Open )
            {
                availablePath.path[0] = ConnectionType.Open;
            }

            else
            {
                availablePath.path[0] = ConnectionType.Blocked;
            }


            if (currentCell.neighbors[1] != null && 
                currentCell.direction.path[1] ==  ConnectionType.Open && 
                currentCell.neighbors[1].direction.path[3] ==  ConnectionType.Open)
            {
                availablePath.path[1] = ConnectionType.Open;
            }

            else
            {
                availablePath.path[1] = ConnectionType.Blocked;
            }

            if (currentCell.neighbors[2] != null &&  
                currentCell.direction.path[2] == ConnectionType.Open && 
                currentCell.neighbors[2].direction.path[0] == ConnectionType.Open)

                availablePath.path[2] = ConnectionType.Open;

            else
            {
                availablePath.path[2] = ConnectionType.Blocked;
            }

            if (currentCell.neighbors[3] != null &&
                currentCell.direction.path[3] ==  ConnectionType.Open 
                && currentCell.neighbors[3].direction.path[1] ==  ConnectionType.Open)
                availablePath.path[3] = ConnectionType.Open;

            else
            {
                availablePath.path[3] = ConnectionType.Blocked;
            }

            
        }
    }
}