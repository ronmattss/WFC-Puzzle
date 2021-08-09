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