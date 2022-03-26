using System.Collections;
using System.Collections.Generic;
using _Core.Managers;
using Assets.Scripts.Sound_System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SoundSystem
{
    public enum MusicMood
    {
        Suspense,
        Aggressive,
        Bright,
        Calm,
        Dark,
        Grooving,
        Intense,
        Mysterious,
        Somber,
        Uplifting,
        Sad
    }

    public class AudioManager : MonoBehaviour
    {
        [Range(0,1)] [SerializeField] private float _masterVolume = 1f;
        [SerializeField] private Music[] _musicList;
        private Music _currentMusic;
        [SerializeField] private float _musicFadeTime = 1.5f;


        [SerializeField] private Ambience[] _ambiences;
        private Ambience _currentAmbience;
        [SerializeField] private float ambienceFadeTime = 2.5f;

        [SerializeField] private Sfx[] _soundFx;
        [SerializeField] private Sfx[] _maleDialogueSfx;
        [SerializeField] private Sfx[] _femaleDialogueSfx;

        private Dictionary<string, Music> _musicDict;
        private Dictionary<string, Ambience> _ambienceDict;
        private Dictionary<string, Sfx> _sfxDict;
        private Dictionary<string, Audio> _master;

        public AudioManager(Dictionary<string, Audio> master, Dictionary<string, Sfx> sfxDict, Dictionary<string, Ambience> ambienceDict, Dictionary<string, Music> musicDict)
        {
            _master = master;
            _sfxDict = sfxDict;
            _ambienceDict = ambienceDict;
            _musicDict = musicDict;
        }

        void Awake()
        {
            _musicDict = new Dictionary<string, Music>();
            _ambienceDict = new Dictionary<string, Ambience>();
            _sfxDict = new Dictionary<string, Sfx>();
            _master = new Dictionary<string, Audio>();

            foreach (var music in _musicList)
            {
                if (music.clip == null) continue;
                music.source = gameObject.AddComponent<AudioSource>();
                music.source.clip = music.clip;
                music.source.volume = music.volume;
                music.source.loop = music.loop;
                if (music.name == string.Empty)
                {
                    music.name = music.clip.name;
                }
                _musicDict.Add(music.name.ToUpperInvariant().Replace(" ", string.Empty), music);
                _master.Add(music.name.ToUpperInvariant().Replace(" ", string.Empty), music);
            }

            foreach (var sound in _soundFx)
            {
                if (sound.clip == null) continue;
                sound.source = gameObject.AddComponent<AudioSource>();
                sound.source.clip = sound.clip;
                sound.source.volume = sound.volume;
                if (sound.name == string.Empty)
                {
                    sound.name = sound.clip.name;
                }
                _sfxDict.Add(sound.name.ToUpperInvariant().Replace(" ", string.Empty), sound);
                _master.Add(sound.name.ToUpperInvariant().Replace(" ", string.Empty), sound);
            }

            foreach (var ambience in _ambiences)
            {
                if (ambience.clip == null) continue;
                ambience.source = gameObject.AddComponent<AudioSource>();
                ambience.source.clip = ambience.clip;
                ambience.source.volume = ambience.volume;
                ambience.source.loop = ambience.loop;
                if (ambience.name == string.Empty)
                {
                    ambience.name = ambience.clip.name;
                }
                _ambienceDict.Add(ambience.name.ToUpperInvariant().Replace(" ", string.Empty), ambience);
                _master.Add(ambience.name.ToUpperInvariant().Replace(" ", string.Empty), ambience);
            }

            foreach (var maleDialogue in _maleDialogueSfx)
            {
                if (maleDialogue.clip == null) continue;
                maleDialogue.source = gameObject.AddComponent<AudioSource>();
                maleDialogue.source.clip = maleDialogue.clip;
                maleDialogue.source.volume = maleDialogue.volume;
                if (maleDialogue.name == string.Empty)
                {
                    maleDialogue.name = maleDialogue.clip.name;
                }
                var dialogueName = maleDialogue.name + "_MALE";
                _sfxDict.Add(dialogueName, maleDialogue);
                _master.Add(dialogueName, maleDialogue);
            }

            foreach (var femaleDialogue in _femaleDialogueSfx)
            {
                if(femaleDialogue.clip == null) continue;
                femaleDialogue.source = gameObject.AddComponent<AudioSource>();
                femaleDialogue.source.clip = femaleDialogue.clip;
                femaleDialogue.source.volume = femaleDialogue.volume;
                if (femaleDialogue.name == string.Empty)
                {
                    femaleDialogue.name = femaleDialogue.clip.name;
                }
                var dialogueName = femaleDialogue.name + "_FEMALE";
                _sfxDict.Add(dialogueName, femaleDialogue);
                _master.Add(dialogueName, femaleDialogue);
            }
        }

        void Start()
        {
            if(_musicList.Length <= 0) return;
            PlayMusic(MusicMood.Calm);
            ChangeMasterVolume(_masterVolume);
        }

        public void ChangeMasterVolume(float newVolume)
        {
            _masterVolume = newVolume;
            foreach (var audioClip in _master.Values)
            {
                audioClip.source.volume *= newVolume;
            }
        }

        public void PlayAmbience(string ambienceName)
        {
            if (!_ambienceDict.TryGetValue(ambienceName.ToUpperInvariant().Replace(" ", string.Empty), out var ambience)) return;
            if(_currentAmbience.name == ambienceName) return;

            StopAllCoroutines();
            StartCoroutine(FadeTrack(ambience));
            _currentAmbience = ambience;
        }

        public void PlayMusic(string musicName)
        {
            if (!_musicDict.TryGetValue(musicName.ToUpperInvariant().Replace(" ", string.Empty), out var music)) return;
            if (_currentMusic.name == musicName) return;

            StopAllCoroutines();
            StartCoroutine(FadeTrack(music));
            _currentMusic = music;
        }

        public void PlayMusic(MusicMood musicMood)
        {
            var musicInMood = new List<Music>();

            foreach (var music in _musicList)
            {
                if(music.musicMood != musicMood) return;
                musicInMood.Add(music);
            }

            var musicToPlay = musicInMood[Random.Range(0, musicInMood.Count)];

            if(_currentMusic.name == musicToPlay.name) return;

            StopAllCoroutines();
            StartCoroutine(FadeTrack(musicToPlay));
            _currentMusic = musicToPlay;
        }

        public void PlaySFX(string soundName)
        {
            if (!_sfxDict.TryGetValue(soundName.ToUpperInvariant().Replace(" ", string.Empty), out var sfx)) { return; }
            
            sfx.source.pitch = sfx.randomPitch ? Random.Range(sfx.min, sfx.max) : sfx.pitch;
            sfx.source.Play();
        }

        public void PlaySFX(string soundName, float pitch)
        {
            if (!_sfxDict.TryGetValue(soundName.ToUpperInvariant().Replace(" ", string.Empty), out var sfx)) { return; }

            sfx.source.pitch = sfx.pitch * pitch;
            sfx.source.Play();
        }

        private IEnumerator FadeTrack(Audio audioToChange)
        {
            var audioMusic = audioToChange is Music;
            var fadeTime = audioMusic ? _musicFadeTime : ambienceFadeTime;
            var currentAudio = audioMusic ? (Audio)_currentMusic : _currentAmbience;
            var timeElapsed = 0f;

            audioToChange.source.Play();
            while (timeElapsed < fadeTime)
            {
                audioToChange.source.volume = Mathf.Lerp(0, audioToChange.volume, timeElapsed/fadeTime);
                currentAudio.source.volume = Mathf.Lerp(currentAudio.volume, 0, timeElapsed / fadeTime);
                timeElapsed += Time.unscaledDeltaTime;
                yield return null;
            }
        }
    }
}
