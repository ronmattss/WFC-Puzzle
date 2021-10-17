using ProjectAssets.Scripts.Util;
using UnityEngine;

namespace ProjectAssets.SFX
{
    public class SoundManager : Singleton<SoundManager>
    {
        // Test BGM

        [SerializeField]private AudioClip clip;
        public AudioSource inGameMusic;
        public AudioSource menuMusic;

        public AudioSource gameSFX;
        [SerializeField]private AudioClip keyPickupClip;
        [SerializeField]private AudioClip inGameClip;
        [SerializeField]private AudioClip menuClip;

        [Range(0,1)]
        public float sfxVolume = 0.5f;


        public void PlaySFX(AudioClip clip)
        {
            gameSFX.clip = clip;
            gameSFX.volume = 1f;

            gameSFX.Play();
        }


        public void PlayMenuMusic()
        {
            inGameMusic.Stop();
            menuMusic.UnPause();

        }
        public void PlayGameMusic()
        {
            menuMusic.Pause();
            inGameMusic.Play();
        }

        public void PlayPickupClip()
        {
           // gameSFX.Stop();
            gameSFX.volume = 0.25f;
            gameSFX.clip = keyPickupClip;
            gameSFX.Play();
            
        }
        
        

    }
}