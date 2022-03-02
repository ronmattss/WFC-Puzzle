/*
 Title: Module
 Author: Ron Matthew Rivera
 Sub-System: Part of Wave Function Collapse Algorithm
 Date Written/Revised: Aug. 14, 2021
 Purpose: This class is used to create a modules for the Wave Function Collapse Algorithm.
 Data Structures, algorithms, and control: Class,enums,interfaces
 */
using UnityEngine;

namespace ProjectAssets.Scripts.Puzzle_Generation
{

    public enum ConnectionType
    {
        Blocked,
        Open,
        None // This means that no connection can be made?
    }
    [CreateAssetMenu(fileName = "NewModule", menuName = "WFC/Module", order = 0)]
    public class Module : ScriptableObject
    {
        // test two types of edges
        // Types of edge: Blocked, Open


        public GameObject moduleGameObject;
        [Header("Connections: Bottom,Right,Top,Left")]
        public ConnectionType[] connections = new ConnectionType[4];
    }
}