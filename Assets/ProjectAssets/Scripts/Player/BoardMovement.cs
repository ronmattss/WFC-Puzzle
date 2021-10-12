using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProjectAssets.Scripts.Gameplay;
using ProjectAssets.Scripts.Puzzle_Generation;
using ProjectAssets.Scripts.Util;
using UnityEngine;
using UnityTemplateProjects.UI;
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
        public GameObject currentCellGameObject;
        public List<GameObject> traversedCells = new List<GameObject>();
        private float x = 0;
        private float y = 0;
        private Transform cellObjectTransform;
        public Direction availablePath;
        private bool isCellRotating = false;
        private bool isNeighborCellRotating = false;
        private int _degreeRotation = 0;
        [SerializeField] private Animator jammoAnimator;

        public int totalMoves = 1;

        private bool canMove = true;

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

        IEnumerator MoveCoolDown()
        {
            canMove = false;
            yield return new WaitForSecondsRealtime(.5f);
            canMove = true;
        }
        

        public void SetStartPosition(Vector3 pos)
        {    traversedCells.Clear();
            totalMoves = 1; // this IDK where the culprit for this
            GameManager.Instance.modifier.GetPlayerMovement(totalMoves);
            UIManager.Instance.ChangeMoveText(totalMoves);
            x = pos.x;
            y = pos.z;
            currentCell = GameManager.Instance.GetCurrentCell((int) Math.Floor(x),
                (int) Math.Floor(y));

            this.transform.position = new Vector3(x,
                0,
                y);
            cellObjectTransform = currentCell.gameObject.transform; // get the direction this is a culprit
            availablePath.path = new List<ConnectionType>(4);
            availablePath.path.Add(ConnectionType.Open);
            availablePath.path.Add(ConnectionType.Open);
            availablePath.path.Add(ConnectionType.Open);
            availablePath.path.Add(ConnectionType.Open);
            CheckAvailablePath();
            EnableDisableNeighbor();
            GetCurrentCellPosition();
            MovePlayer();


        }

// should it lerp or teleport
        private void PlayerMovement()
        {
            // Get Current Cell
            // Get axis or input button?
            //Debug.Log($"current Cell: {currentCell} Edge: Top: {currentCell.module.connections[2]} Bottom: {currentCell.module.connections[0]} Left: {currentCell.module.connections[3]} Right: {currentCell.module.connections[1]}");
            if (canMove)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    GameManager.Instance.text.text = $"Pressed: W";
                    if (availablePath.path[2] == ConnectionType.Open)
                    {
                        y++;
                        gameObject.transform.LeanRotateY(AvatarRotation(0), 0.15f).setOnComplete(OnAvatarCompleteRotation);
                        _degreeRotation = 0;
                        //   MovePlayer();
                        //  totalMoves++;
                        //  GetCurrentCellPosition();

                    }
                }
                // else if to avoid multiple inputs
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    GameManager.Instance.text.text = $"Pressed: S";
                    if (availablePath.path[0] == ConnectionType.Open)
                    {
                        y--;
                        gameObject.transform.LeanRotateY(AvatarRotation(2), 0.15f).setOnComplete(OnAvatarCompleteRotation);
                        _degreeRotation = 2;
                        //MovePlayer();
                        //  totalMoves++;
                        // GetCurrentCellPosition();
                    }
                }

                else if (Input.GetKeyDown(KeyCode.A))
                {
                    GameManager.Instance.text.text = $"Pressed: A";
                    if (availablePath.path[3] == ConnectionType.Open)
                    {
                        x--;
                        gameObject.transform.LeanRotateY(AvatarRotation(1), 0.15f).setOnComplete(OnAvatarCompleteRotation); 
                        _degreeRotation = 1;
                        //MovePlayer();
                      //  totalMoves++;
                      //  GetCurrentCellPosition();
                    }
                }

                else if (Input.GetKeyDown(KeyCode.D))
                {
                    GameManager.Instance.text.text = $"Pressed: D";
                    if (availablePath.path[1] == ConnectionType.Open)
                    {
                        x++;
                        gameObject.transform.LeanRotateY(AvatarRotation(3), 0.15f).setOnComplete(OnAvatarCompleteRotation);
                        _degreeRotation = 3;
                       // MovePlayer();
                      //  totalMoves++;
                       // GetCurrentCellPosition();
                    }
                }  
            }


            GameManager.Instance.modifier.GetPlayerMovement(totalMoves);

            // Get current coordinates of player
        }

        public void OnAvatarCompleteRotation()
        {
            MovePlayer();
        }

        public void MovePlayer()
        {
            StartCoroutine(MoveCoolDown());
            // only show the current four neighbors available
            if (boardX > x && x >= 0 && boardY > y && y >= 0)
            {
                jammoAnimator.SetBool("isRunning",true);
                currPosition = new Vector3(x,
                    0,
                    y);
                gameObject.transform.LeanMove(currPosition, .15f).setOnComplete(OnMovementComplete);
                //gameObject.transform.position = currPosition;// Lean Tween this

            }
            else
            {    // this means that just don't move?
                var position = gameObject.transform.position;
                x = position.x;
                y = position.z;
            }

            //
        }

        private void OnMovementComplete()
        {
            jammoAnimator.SetBool("isRunning",false);
            currentCell = GameManager.Instance.GetCurrentCell((int) Math.Floor(x),
                (int) Math.Floor(y));
            //   availablePath = currentCell.direction; // <- culprit??? available path is copied to the current cell from the prev cell after moving
            // check if currentCell is on the Visited Cells
            if (!MatchIfCellOnPath(currentCell))
            {
                totalMoves++;
            }
            
            CheckAvailablePath();
            cellObjectTransform = currentCell.gameObject.transform;
            GameManager.Instance.GoalChecker(currentCell);
            GetCurrentCellPosition();
            CellVisuals.Instance.ChangeGridColor(currentCell,Color.white);
            CellVisuals.Instance.ChangeWallColor(currentCell,Color.yellow);
            
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

        float AvatarRotation(int state)
        {
            switch (state)
            {
                case 0:
                    return 0;
                case 1:
                    return -90;
                case 2:
                    return 180;
                case 3:
                    return 90;
            }

            return  0;
        }

        // also get the cell that will be rotated
        private void RotateCell(int degrees)
        {
            if (!currentCell.isRotatable) return;
            // this is the current 
            _degreeRotation ++;
            if (_degreeRotation == 4)
            {
                _degreeRotation = 0;
            }
            gameObject.transform.LeanRotateY(AvatarRotation(_degreeRotation), .25f);

            var cellQuaternion = cellObjectTransform.rotation;
            var cellEuler = cellObjectTransform.rotation.eulerAngles;
            cellEuler += new Vector3(0,
                degrees,
                0);
            cellQuaternion.eulerAngles = cellEuler;
            //cellObjectTransform.rotation = cellQuaternion;
            isCellRotating = !isCellRotating;
            cellObjectTransform.LeanRotate(cellEuler,.1f).setOnComplete(RotateCurrentCell);
           currentCell.RotateRight();
             //<- probably culprit
            // If Available Path is open check if neighbor path is Open
            // if Open player can traverse if not then don't lol
        }

        void RotateCurrentCell()
        {
           
            isCellRotating = false;
        }

        private void RotateNeighborCells(int degrees)
        {
            if (isNeighborCellRotating) return;
            isNeighborCellRotating = true;
            var holdPath = currentCell.direction;
            for (int i = 0;
                i < currentCell.neighbors.Length;
                i++)
            {
                // Get object rotation
                if (currentCell.neighbors[i] != null)
                {
                    if (!currentCell.neighbors[i].isRotatable) continue;
                    var neighborRotation = currentCell.neighbors[i]
                        .transform.rotation;
                    var neighborRotationEulerAngles = currentCell.neighbors[i]
                        .transform.rotation.eulerAngles;
                    neighborRotationEulerAngles += new Vector3(0,
                        degrees,
                        0);
                    neighborRotation.eulerAngles = neighborRotationEulerAngles;
                    // currentCell.neighbors[i]
                    //     .transform.rotation = neighborRotation;
                    currentCell.neighbors[i]
                        .transform.LeanRotate(neighborRotationEulerAngles, .1f).setOnComplete(OnNeighborCellFinishRotating);
                    currentCell.neighbors[i]
                        .RotateLeft();
                }
            }

            currentCell.direction = holdPath; // <- culprit?
        }

        void OnNeighborCellFinishRotating()
        {
            isNeighborCellRotating = false;
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
                currentCell.neighbors[0]
                    .direction.path[2] ==
                ConnectionType.Open)
            {
                availablePath.path[0] = ConnectionType.Open;
            }

            else
            {
                availablePath.path[0] = ConnectionType.Blocked;
            }


            if (currentCell.neighbors[1] != null &&
                currentCell.direction.path[1] == ConnectionType.Open &&
                currentCell.neighbors[1]
                    .direction.path[3] ==
                ConnectionType.Open)
            {
                availablePath.path[1] = ConnectionType.Open;
            }

            else
            {
                availablePath.path[1] = ConnectionType.Blocked;
            }

            if (currentCell.neighbors[2] != null &&
                currentCell.direction.path[2] == ConnectionType.Open &&
                currentCell.neighbors[2]
                    .direction.path[0] ==
                ConnectionType.Open)

                availablePath.path[2] = ConnectionType.Open;

            else
            {
                availablePath.path[2] = ConnectionType.Blocked;
            }

            if (currentCell.neighbors[3] != null &&
                currentCell.direction.path[3] == ConnectionType.Open &&
                currentCell.neighbors[3]
                    .direction.path[1] ==
                ConnectionType.Open)
                availablePath.path[3] = ConnectionType.Open;

            else
            {
                availablePath.path[3] = ConnectionType.Blocked;
            }


        }

        // Get current cell Object via this object's transform

        public void GetCurrentCellPosition()
        {
            var currObjectCellList = GameManager.Instance.cellGameObjects;

            var endCell = GameManager.Instance.endCell;

            var position = this.gameObject.transform.position;
            var playerPos = new Vector3(position.x,
                0,
                position.z);
            
            foreach (var cell in currObjectCellList)
            {
                if (playerPos == cell.transform.position)
                {
                    currentCellGameObject = cell;
                    // retain path
                    
                        if(MatchCellsOnPath(cell))continue;
                        cell.gameObject.GetComponent<Cell>().EaseToPosition(true);
                        cell.gameObject.GetComponent<Cell>().cellOnPosition = false;
                    

                    //cell.gameObject.SetActive(true);
                    if (!MatchCellsOnPath(cell.gameObject))
                        traversedCells.Add(cell);
                }
                else
                {    // this is where you retain stuff
                    if (cell.GetComponent<Cell>() == endCell || MatchCellsOnPath(cell)) continue;
                    cell.gameObject.GetComponent<Cell>().EaseToPosition(false);
                    cell.gameObject.GetComponent<Cell>().cellOnPosition = true;
                }
            }

            EnableDisableNeighbor();
            SetTraversedCellsTrue();
        }

        public void EnableDisableNeighbor()
        {
            var endCell = GameManager.Instance.endCell;
            foreach (var cell in currentCell.neighbors)
            {
                if (cell == null || cell == GameManager.Instance.endCell) continue;
                if (!cell.GetComponent<Cell>() == endCell || !MatchCellsOnPath(cell.gameObject))
                {
                    cell.gameObject.GetComponent<Cell>().EaseToPosition(cell.gameObject.GetComponent<Cell>().cellOnPosition);
                }
               
                //CellVisuals.Instance.ChangeGridColor(cell,cell.GridColorBasedOnProperties());
                CellVisuals.Instance.ChangeWallColor(cell);
            }
        }

        public void ShowGoalCell()
        {
            GameManager.Instance.endCell.EaseToPosition(true);
        }

        public Boolean MatchCellsOnPath(GameObject cellObject)
        {
            foreach (var cell in traversedCells)
            {
                if (cellObject.name == cell.name)
                    return true;
            }

            return false;

        }
        public Boolean MatchIfCellOnPath(Cell cCell)
        {
            foreach (var cell in traversedCells)
            {
                if (cCell.cellID == cell.GetComponent<Cell>().cellID)
                    return true;
            }

            return false;

        }

        public void SetTraversedCellsTrue()
        {
            foreach (var cell in traversedCells)
            {
                var c = cell.gameObject.GetComponent<Cell>();
                CellVisuals.Instance.ChangeGridColor(c,Color.white);
                CellVisuals.Instance.ChangeWallColor(c,Color.green);

            }


        }

        // TODO: Just Enable the current cell and the neighboring cells
        // once not in proximity, go disable, IF became a path, then retain
        // data? list of cells player used, 
        // previous cell, current cell
    }
}