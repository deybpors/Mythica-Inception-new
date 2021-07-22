using System.Collections.Generic;
using MyBox;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Core.Managers
{
    public enum Mood
    {
        suspense,
        aggressive,
        bright,
        calm,
        dark,
        grooving,
        intense,
        mysterious,
        somber,
        uplifting,
        sad
    }

    public class AudioManager : MonoBehaviour
    {
        [Range(0,1)] public float volume = 1f;
        public Music[] music;
        public Sfx[] soundFX;
        private Dictionary<string, Music> _musicDict;
        private Dictionary<string, Sfx> _sfxDict;
        private List<Audio> master;

        void Awake()
        {
            foreach (var m in music)
            {
                m.source = gameObject.AddComponent<AudioSource>();
                m.source.clip = m.clip;
                m.source.volume = m.volume;
                m.source.loop = m.loop;
                _musicDict.Add(m.name, m);
                master.Add(m);
            }

            foreach (var s in soundFX)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                _sfxDict.Add(s.name, s);
                master.Add(s);
            }
        }

        void Start()
        {
            if(music.Length <= 0) return;
            PlayMusicViaName(music[Random.Range(0, music.Length)].name);
            ChangeMasterVolume(volume);
        }

        public void ChangeMasterVolume(float newVolume)
        {
            volume = newVolume;
            foreach (var audio in master)
            {
                audio.source.volume *= newVolume;
            }
        }

        public void PlayMusicViaName(string musicName)
        {
            if (_musicDict.TryGetValue(musicName, out var music))
            {
                music.source.Play();
            }
        }

        public void PlayMusicViaMood(Mood mood)
        {
            var musicInMood = new List<Music>();
            foreach (var m in music)
            {
                if(m.mood != mood) return;
                musicInMood.Add(m);
            }
            musicInMood[Random.Range(0, musicInMood.Count)].source.Play();
        }
        
        public void PlaySfx(string soundName)
        {
            if (!_sfxDict.TryGetValue(soundName, out var sfx)) return;
            
            sfx.source.pitch = sfx.randomPitch ? Random.Range(sfx.min, sfx.max) : sfx.pitch;
            sfx.source.Play();
        }
    }

    public class Audio
    {
        public string name;
        public AudioClip clip;
        
        [Range(0f,1f)]
        public float volume = 1f;
        [HideInInspector]
        public AudioSource source;
    }
    
    [System.Serializable]
    public class Music : Audio
    {
        public Mood mood;
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

        [ConditionalField(nameof(randomPitch), inverse:true)]
        [Range(.1f,3f)]
        public float pitch = 1f;
    }
}
