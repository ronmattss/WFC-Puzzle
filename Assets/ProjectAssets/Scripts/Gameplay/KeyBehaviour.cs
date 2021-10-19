using System;
using ProjectAssets.SFX;
using UnityEngine;

namespace ProjectAssets.Scripts.Gameplay
{
    public class KeyBehaviour : MonoBehaviour
    {
        // THis will just increment the keys
        // Start is called before the first frame update
        public float rotateTime = 2;
        public Target targetIndicator;
        void Start()
        {
            targetIndicator = GetComponent<Target>();
            Spin();
        }

        // Update is called once per frame
        void LateUpdate()
        {

        }

        void Spin()
        {
            LeanTween.rotateAround(gameObject, Vector3.up, 360, rotateTime).setRepeat(-1);
          //  currentRotation = gameObject.transform.rotation.y;
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.name);
            if (other.CompareTag("Player"))
            {
                SoundManager.Instance.PlayPickupClip();
                GameManager.Instance.RemoveKeys(this.gameObject);
                Destroy(this.gameObject);

            }
        }

    }
}
