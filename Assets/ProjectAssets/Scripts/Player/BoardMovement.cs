/*
 Title: Player Movement Script
 Author: Ron Matthew Rivera
 Sub-System: Gameplay
 Date Written/Revised: Dec. 11, 2021
 Purpose: Handles the board movement of the player 
 Data Structures, algorithms, and control: Class. Lists
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProjectAssets.Scripts.Gameplay;
using ProjectAssets.Scripts.Puzzle_Generation;
using ProjectAssets.Scripts.UI;
using ProjectAssets.Scripts.Util;
using UnityEngine;
using UnityTemplateProjects.UI;
using Debug = UnityEngine.Debug;
using Random = Unity.Mathematics.Random;

namespace ProjectAssets.Scripts.Player
{

    public class BoardMovement : MonoBehaviour
    {
        // Start is called before the first frame update

        public Cell currentCell;
        public GameObject currentCellGameObject;
        public List<GameObject> traversedCells = new List<GameObject>();
        public int totalMoves = 1;
        public int moveOnPath = 0;
        public int forceRotation = 3;
        
        private int _boardX;
        private int _boardY;
        private float x = 0;
        private float y = 0;
        private int _degreeRotation = 0;

        private bool _isCellRotating = false;
        private bool _isNeighborCellRotating = false;

        private Vector3 _currPosition;
        private Transform _cellObjectTransform;
        public Direction availablePath;

        [SerializeField] private Animator jammoAnimator;
        [SerializeField] private float countdownToDanceIdle = 5;

        private int _danceIndex = 0;
        private bool _canMove = true;

        private readonly float _cellRotationSpeed = 0.075f;

        private void Start()
        {
            var currPosition = gameObject.transform.position;
            x = currPosition.x;
            y = currPosition.z;
            _boardX = GameManager.Instance.GetBoardWidth();
            _boardY = GameManager.Instance.GetBoardHeight();
            MovePlayer();
        }

       
        private void LateUpdate()
        {
            PlayerMovement();
            Rotate();
        }

        private IEnumerator MoveCoolDown()
        {
            _canMove = false;
            yield return new WaitForSecondsRealtime(.5f);
            _canMove = true;
        }

        public void SetStartPosition(Vector3 pos)
        {
            traversedCells.Clear();
            totalMoves = 1; // this IDK where the culprit for this
            moveOnPath = 0; // ????
            GameManager.Instance.modifier.GetPlayerMovement(totalMoves, moveOnPath);
            UIManager.Instance.ChangeMoveText(totalMoves);
            x = pos.x;
            y = pos.z;
            currentCell = GameManager.Instance.GetCurrentCell((int)Math.Floor(x), (int)Math.Floor(y));

            this.transform.position = new Vector3(x, 0, y);
            _cellObjectTransform = currentCell.gameObject.transform;
            
            availablePath.path = new List<ConnectionType>(4);
            availablePath.path.Add(ConnectionType.Open);
            availablePath.path.Add(ConnectionType.Open);
            availablePath.path.Add(ConnectionType.Open);
            availablePath.path.Add(ConnectionType.Open);
            
            CheckAvailablePath();
            EnableDisableNeighbor();
            GetCurrentCellPosition();
            MovePlayer();
            forceRotation = 0;
        }


        private void PlayerMovement()
        {
            if (_canMove)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    GameManager.Instance.text.text = $"Pressed: W";
                    if (availablePath.path[2] == ConnectionType.Open)
                    {
                        y++;
                        gameObject.transform.LeanRotateY(AvatarRotation(0), 0.05f)
                            .setOnComplete(OnAvatarCompleteRotation);
                        _degreeRotation = 0;

                    }
                }

                else if (Input.GetKeyDown(KeyCode.S))
                {
                    GameManager.Instance.text.text = $"Pressed: S";
                    if (availablePath.path[0] == ConnectionType.Open)
                    {
                        y--;
                        gameObject.transform.LeanRotateY(AvatarRotation(2), 0.05f)
                            .setOnComplete(OnAvatarCompleteRotation);
                        _degreeRotation = 2;

                    }
                }

                else if (Input.GetKeyDown(KeyCode.A))
                {
                    GameManager.Instance.text.text = $"Pressed: A";
                    if (availablePath.path[3] == ConnectionType.Open)
                    {
                        x--;
                        gameObject.transform.LeanRotateY(AvatarRotation(1), 0.05f)
                            .setOnComplete(OnAvatarCompleteRotation);
                        _degreeRotation = 1;

                    }
                }

                else if (Input.GetKeyDown(KeyCode.D))
                {
                    GameManager.Instance.text.text = $"Pressed: D";
                    if (availablePath.path[1] == ConnectionType.Open)
                    {
                        x++;
                        gameObject.transform.LeanRotateY(AvatarRotation(3), 0.05f)
                            .setOnComplete(OnAvatarCompleteRotation);
                        _degreeRotation = 3;
                    }
                }
            }
            
            if (countdownToDanceIdle <= 0)
            {
                jammoAnimator.SetInteger("Dance", _danceIndex);
                jammoAnimator.SetInteger("danceHold", 0);
            }
            else
            {
                countdownToDanceIdle -= Time.deltaTime;
                jammoAnimator.SetInteger("danceHold", (int)countdownToDanceIdle);
            }

            GameManager.Instance.modifier.GetPlayerMovement(totalMoves, moveOnPath);

            // Get current coordinates of player
        }

        private void OnAvatarCompleteRotation()
        {
            MovePlayer();
            countdownToDanceIdle = 5;
        }

        private void MovePlayer()
        {
            StartCoroutine(MoveCoolDown());
            if (_boardX > x && x >= 0 && _boardY > y && y >= 0)
            {
                jammoAnimator.SetBool("isRunning", true);
                _currPosition = new Vector3(x, 0, y);
                gameObject.transform.LeanMove(_currPosition, .15f).setOnComplete(OnMovementComplete);
            }
            else
            {
                // this means that just don't move?
                var position = gameObject.transform.position;
                x = position.x;
                y = position.z;
            }

            //
        }

        private void OnMovementComplete()
        {
            jammoAnimator.SetBool("isRunning", false);

            // Have some other Condition here
            currentCell.RotationLock();

            currentCell = GameManager.Instance.GetCurrentCell((int)Math.Floor(x), (int)Math.Floor(y));

            if (!MatchIfCellOnPath(currentCell))
            {
                totalMoves++;
                if (currentCell.isSuggestedPath)
                {
                    moveOnPath++;
                }
            }

            CheckAvailablePath();
            _cellObjectTransform = currentCell.gameObject.transform;
            GameManager.Instance.GoalChecker(currentCell);
            GetCurrentCellPosition();
            CellVisuals.Instance.ChangeGridColor(currentCell, Color.white); // instead of White 
            CellVisuals.Instance.ChangeWallColor(currentCell, Color.yellow);
        }

        // Instead of rotating current cell, rotate neighbor cells
        // assign the neighbors edge to the available path
        // 
        private void Rotate()
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

            else if (Input.GetKeyDown(KeyCode.F))
            {
                UnlockRotationOfCells();
                RemoveDeathCells();
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

            return 0;
        }

        // also get the cell that will be rotated
        private void RotateCell(int degrees)
        {
            if (!currentCell.isRotatable) return;
            // this is the current 
            _degreeRotation++;
            if (_degreeRotation == 4)
            {
                _degreeRotation = 0;
            }

            gameObject.transform.LeanRotateY(AvatarRotation(_degreeRotation), .25f);

            var cellQuaternion = _cellObjectTransform.rotation;
            var cellEuler = _cellObjectTransform.rotation.eulerAngles;
            cellEuler += new Vector3(0, degrees, 0);
            cellQuaternion.eulerAngles = cellEuler;
            
            _isCellRotating = !_isCellRotating;
            _cellObjectTransform.LeanRotate(cellEuler, _cellRotationSpeed).setOnComplete(RotateCurrentCell);
            currentCell.RotateRight();

        }

        void RotateCurrentCell()
        {
            _isCellRotating = false;
        }

        private void RemoveDeathCells()
        {
            for (int i = 0; i < currentCell.neighbors.Length; i++)
            {
                if (currentCell.neighbors[i] != null && currentCell.neighbors[i].isDeathCell)
                {
                    currentCell.neighbors[i].isDeathCell = false;
                }
            }
        }

        private void UnlockRotationOfCells()
        {
            bool hasLockedCell = !currentCell.isRotatable;

            for (int i = 0; i < currentCell.neighbors.Length; i++)
            {
                if (currentCell.neighbors[i] != null && !currentCell.neighbors[i].isRotatable)
                {
                    hasLockedCell = true;
                }
            }

            if (hasLockedCell && forceRotation > 0)
            {
                currentCell.lockRotation = false;
                currentCell.isRotatable = true;
                // CellVisuals.Instance.ChangeWallColor(c,Color.green);

                for (int i = 0; i < currentCell.neighbors.Length; i++)
                {
                    if (currentCell.neighbors[i] != null)
                    {
                        currentCell.neighbors[i].lockRotation = false;
                        currentCell.neighbors[i].isRotatable = true;

                        CellVisuals.Instance.ChangeWallColor(currentCell.neighbors[i], Color.green);
                    }
                }

                GameManager.Instance.DecrementKeys();
                forceRotation--;
            }
            // unlock all 
        }

        private void RotateNeighborCells(int degrees)
        {
            if (_isNeighborCellRotating) return;
            _isNeighborCellRotating = true;
            var holdPath = currentCell.direction;
            for (int i = 0; i < currentCell.neighbors.Length; i++)
            {
                // Get object rotation
                if (currentCell.neighbors[i] != null)
                {
                    if (!currentCell.neighbors[i].isRotatable) continue;
                    var neighborRotation = currentCell.neighbors[i].transform.rotation;
                    var neighborRotationEulerAngles = currentCell.neighbors[i].transform.rotation.eulerAngles;
                    neighborRotationEulerAngles += new Vector3(0, degrees, 0);
                    neighborRotation.eulerAngles = neighborRotationEulerAngles;
                    // currentCell.neighbors[i]
                    //     .transform.rotation = neighborRotation;
                    currentCell.neighbors[i]
                        .transform.LeanRotate(neighborRotationEulerAngles, _cellRotationSpeed)
                        .setOnComplete(OnNeighborCellFinishRotating);
                    currentCell.neighbors[i].RotateLeft();
                }
            }

            currentCell.direction = holdPath; // <- culprit?
        }

        void OnNeighborCellFinishRotating()
        {
            _isNeighborCellRotating = false;
        }

        // bottom right top left
        // 0 1 2 3
        // when you rotate the current cell AND neighboring cell, available path should be updated
        private void CheckAvailablePath()
        {
            //  Debug.Log($"neighbor at the bottom: {currentCell.neighbors[0].direction.path[2]} right: {currentCell.neighbors[1].direction.path[3]} top: {currentCell.neighbors[2].direction.path[0]} left: {currentCell.neighbors[3].direction.path[1]}");

            if (currentCell.neighbors[0] != null && currentCell.direction.path[0] == ConnectionType.Open &&
                currentCell.neighbors[0].direction.path[2] == ConnectionType.Open)
            {
                availablePath.path[0] = ConnectionType.Open;
            }

            else
            {
                availablePath.path[0] = ConnectionType.Blocked;
            }

            if (currentCell.neighbors[1] != null && currentCell.direction.path[1] == ConnectionType.Open &&
                currentCell.neighbors[1].direction.path[3] == ConnectionType.Open)
            {
                availablePath.path[1] = ConnectionType.Open;
            }

            else
            {
                availablePath.path[1] = ConnectionType.Blocked;
            }

            if (currentCell.neighbors[2] != null && currentCell.direction.path[2] == ConnectionType.Open &&
                currentCell.neighbors[2].direction.path[0] == ConnectionType.Open)
                availablePath.path[2] = ConnectionType.Open;

            else
            {
                availablePath.path[2] = ConnectionType.Blocked;
            }

            if (currentCell.neighbors[3] != null && currentCell.direction.path[3] == ConnectionType.Open &&
                currentCell.neighbors[3].direction.path[1] == ConnectionType.Open)
                availablePath.path[3] = ConnectionType.Open;

            else
            {
                availablePath.path[3] = ConnectionType.Blocked;
            }
        }

        // Get current cell Object via this object's transform

        private void GetCurrentCellPosition()
        {
            var currObjectCellList = GameManager.Instance.cellGameObjects;

            var endCell = GameManager.Instance.endCell;

            var position = this.gameObject.transform.position;
            var playerPos = new Vector3(position.x, 0, position.z);

            foreach (var cell in currObjectCellList)
            {
                if (playerPos == cell.transform.position)
                {
                    currentCellGameObject = cell;
                    // retain path

                    if (MatchCellsOnPath(cell)) continue;
                    cell.gameObject.GetComponent<Cell>().EaseToPosition(true);
                    cell.gameObject.GetComponent<Cell>().cellOnPosition = false;

                    //cell.gameObject.SetActive(true);
                    if (!MatchCellsOnPath(cell.gameObject)) traversedCells.Add(cell);
                }
                else
                {
                    // this is where you retain stuff
                    if (cell.GetComponent<Cell>() == endCell || MatchCellsOnPath(cell)) continue;
                    cell.gameObject.GetComponent<Cell>().EaseToPosition(false);
                    cell.gameObject.GetComponent<Cell>().cellOnPosition = true;
                }
            }

            EnableDisableNeighbor();
            SetTraversedCellsTrue();
        }

        private void EnableDisableNeighbor()
        {
            var endCell = GameManager.Instance.endCell;
            foreach (var cell in currentCell.neighbors)
            {
                if (cell == null || cell == GameManager.Instance.endCell) continue;
                if (!cell.GetComponent<Cell>() == endCell || !MatchCellsOnPath(cell.gameObject))
                {
                    cell.gameObject.GetComponent<Cell>()
                        .EaseToPosition(cell.gameObject.GetComponent<Cell>().cellOnPosition);
                }

                //CellVisuals.Instance.ChangeGridColor(cell,cell.GridColorBasedOnProperties());
                CellVisuals.Instance.ChangeWallColor(cell);
            }
        }

        private bool MatchCellsOnPath(GameObject cellObject)
        {
            foreach (var cell in traversedCells)
            {
                if (cellObject.name == cell.name) return true;
            }

            return false;
        }

        private bool MatchIfCellOnPath(Cell cCell)
        {
            foreach (var cell in traversedCells)
            {
                if (cCell.cellID == cell.GetComponent<Cell>().cellID) return true;
            }

            return false;
        }

        private void SetTraversedCellsTrue()
        {
            foreach (var cell in traversedCells)
            {
                var c = cell.gameObject.GetComponent<Cell>();
                CellVisuals.Instance.ChangeGridColor(c, Color.white);
                if (c.isRotatable)
                    CellVisuals.Instance.ChangeWallColor(c, Color.green);
                else
                    CellVisuals.Instance.ChangeWallColor(c, Color.magenta);
            }
        }
    }
}