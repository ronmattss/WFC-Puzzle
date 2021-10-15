using ProjectAssets.Scripts.Util;
using UnityEngine;

namespace ProjectAssets.SFX
{
    public class SoundManager : Singleton<SoundManager>
    {
        // Test BGM

        [SerializeField]private AudioClip clip;
        public AudioSource gameMusic;
        public AudioSource gameSFX;


        public void PlaySFX(AudioClip clip)
        {
            gameSFX.clip = clip;
            gameSFX.Play();
        }

    }
}