using MyBox;
using SoundSystem;
using UnityEngine;

namespace Assets.Scripts.Sound_System
{
    public class Audio
    {
        public string name;
        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume = 1f;

        [HideInInspector]
        public AudioSource source;
    }

    [System.Serializable]
    public class Music : Audio
    {
        public MusicMood musicMood;
        public bool loop;
    }

    [System.Serializable]
    public class Ambience : Audio
    {
        public bool loop;
    }

    [System.Serializable]
    public class Sfx : Audio
    {
        public bool randomPitch;
        [ConditionalField(nameof(randomPitch))]
        public float min;
        [ConditionalField(nameof(randomPitch))]
        public float max;

        [ConditionalField(nameof(randomPitch), inverse: true)]
        [Range(.1f, 3f)]
        public float pitch = 1f;
    }
}
