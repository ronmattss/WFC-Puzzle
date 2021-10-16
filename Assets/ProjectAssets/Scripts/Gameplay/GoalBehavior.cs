using UnityEngine;

namespace ProjectAssets.Scripts.Gameplay
{
    public class GoalBehavior : MonoBehaviour
    {
        public Target targetIndicator;
        void LateUpdate()
        {
            if (GetComponent<Renderer>().isVisible)
            {
                Debug.Log("Visible");
             //   targetIndicator.enabled = false;
            }
            else
            {
                Debug.Log("Invisible");

           //     targetIndicator.enabled = true;
                
            }
        }
    }
}