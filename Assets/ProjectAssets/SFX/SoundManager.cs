using ProjectAssets.Scripts.Util;
using UnityEngine;

namespace ProjectAssets.SFX
{
    public class SoundManager : Singleton<SoundManager>
    {
        // Test BGM

        [SerializeField]private AudioClip clip;
        public AudioSource gameAudio;

    }
}