using UnityEngine;
namespace ProjectAssets.Scripts.Util
{
    public class GeneralUtility : Singleton<GeneralUtility>
    {
        // Start is called before the first frame update
  
        public Vector3 GetWorldPosition(int x = 1,int y = 0, int z = 1 ,float objectSize = 1)
        {
            return new Vector3(x,y,z) * objectSize;
        }

        public Vector3 CenterGameObject(Vector3 currentPosition, Vector3 currentScale)
        {
            var gObjectScale =currentScale;
            var gObjectPosition = currentPosition;
           return GetWorldPosition((int)gObjectPosition.x,(int)gObjectPosition.y,(int)gObjectPosition.z,1) + new Vector3(gObjectScale.x, 0, gObjectScale.z) * 0.5f;
        }
        


         



     

        
    }
}
