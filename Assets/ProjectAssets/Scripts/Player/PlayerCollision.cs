using System;
using UnityEngine;

namespace ProjectAssets.Scripts.Player
{
    public class PlayerCollision : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.name);
        }
    }
}