using ProjectAssets.SFX;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace UnityTemplateProjects.UI
{
    public class ButtonEvents : Selectable
    {
        public UnityEvent onClick;
        public UnityEvent onHighlight;
        public AudioClip sfx; 
        void Update()
        {

            //Check if the GameObject is being highlighted
            if (IsHighlighted())
            {

                //Output that the GameObject was highlighted, or do something else
                Debug.Log("Selectable is Highlighted");
            }

            if (IsPressed())
            {
                onClick.Invoke();
            }
        }

        public void PlayClip()
        {
            SoundManager.Instance.PlaySFX(sfx);
        }
    }
}