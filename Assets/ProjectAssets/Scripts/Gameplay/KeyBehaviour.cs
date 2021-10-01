using System;
using UnityEngine;

namespace ProjectAssets.Scripts.Gameplay
{
    public class KeyBehaviour : MonoBehaviour
    {
        // THis will just increment the keys
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.name);
            if (other.CompareTag("Player"))
            {
                GameManager.Instance.RemoveKeys(this.gameObject);
                Destroy(this.gameObject);

            }
        }
    }
}
