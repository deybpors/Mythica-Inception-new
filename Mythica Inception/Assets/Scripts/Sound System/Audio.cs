using MyBox;
using SoundSystem;
using UnityEngine;
using System;

namespace Assets.Scripts.Sound_System
{
    public class Audio
    {
        [HideInInspector] public string name;
        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume = 1f;

        [HideInInspector]
        public AudioSource source;
    }

    [Serializable]
    public class Music : Audio
    {
        public MusicMood mood;
        public MusicSituation situation;
        public bool loop;
    }

    [Serializable]
    public class Ambience : Audio
    {
        public bool loop;
    }

    [Serializable]
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
