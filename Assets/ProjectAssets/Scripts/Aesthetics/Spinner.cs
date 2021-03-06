using UnityEngine;

namespace ProjectAssets.Scripts.Aesthetics
{
    /// <summary>
    /// Monobehaviour that handles the rotation of the keys
    /// </summary>
    public class Spinner : MonoBehaviour
    {
        public GameObject objectToRotate;
        public float rotateTime = 1;

        void Start()
        {
            Spin();
        }
        
        void Spin()
        {
            LeanTween.rotateAround(objectToRotate, Vector3.up, 360, rotateTime).setRepeat(-1);
            //  currentRotation = gameObject.transform.rotation.y;
        }
        
        
    }
}