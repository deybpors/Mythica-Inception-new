using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Sound_System;
using MyBox;
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
        Sad,
        None
    }

    public enum SoundType
    {
        All,
        SFX,
        Background,
        Ambience
    }

    public enum MusicSituation
    {
        Battle,
        Dungeon,
        Wild,
        ShopBarter,
        None
    }

    public enum MusicTypePlayed
    {
        Name, Mood, Situation
    }

    public class AudioManager : MonoBehaviour
    {
        [Range(0,1)] public float masterVolume = 1f;
        [ReadOnly] public float sfxVolume = 1f;
        [ReadOnly] public float ambienceVolume = 1f;
        [ReadOnly] [SerializeField] private float _bgMusicVolume = 1f;

        [SerializeField] private Music[] _musicList;
        private Music _currentMusic;
        private MusicTypePlayed _musicTypePlayed;
        [SerializeField] private float _musicFadeTime = 1.5f;
        [Range(0, 1)] [SerializeField] private float _musicEndOn;


        [SerializeField] private Ambience[] _ambiences;

        [SerializeField] private Sfx[] _soundFx;
        [SerializeField] private Sfx[] _maleDialogueSfx;
        [SerializeField] private Sfx[] _femaleDialogueSfx;

        private readonly Dictionary<string, Music> _musicDict = new Dictionary<string, Music>();
        private readonly Dictionary<string, Ambience> _ambienceDict = new Dictionary<string, Ambience>();
        private readonly Dictionary<string, Sfx> _sfxDict = new Dictionary<string, Sfx>();
        private readonly Dictionary<string, Audio> _master = new Dictionary<string, Audio>();
        private float _timeElapsed;

        void Awake()
        {
            _currentMusic = null;

            foreach (var music in _musicList)
            {
                if (music.clip == null) continue;

                music.source = gameObject.AddComponent<AudioSource>();
                music.source.clip = music.clip;
                music.source.volume = music.volume;
                music.source.loop = music.loop;
                _musicDict.Add(music.name.ToUpperInvariant().Replace(" ", string.Empty), music);
                _master.Add(music.name.ToUpperInvariant().Replace(" ", string.Empty), music);
            }

            foreach (var sound in _soundFx)
            {
                if (sound.clip == null) continue;

                sound.source = gameObject.AddComponent<AudioSource>();
                sound.source.clip = sound.clip;
                sound.source.volume = sound.volume;
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
                _ambienceDict.Add(ambience.name.ToUpperInvariant().Replace(" ", string.Empty), ambience);
                _master.Add(ambience.name.ToUpperInvariant().Replace(" ", string.Empty), ambience);
            }

            foreach (var maleDialogue in _maleDialogueSfx)
            {
                if (maleDialogue.clip == null) continue;

                maleDialogue.source = gameObject.AddComponent<AudioSource>();
                maleDialogue.source.clip = maleDialogue.clip;
                maleDialogue.source.volume = maleDialogue.volume;
                maleDialogue.name = maleDialogue.clip.name;
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
                femaleDialogue.name = femaleDialogue.clip.name;
                var dialogueName = femaleDialogue.name + "_FEMALE";
                _sfxDict.Add(dialogueName, femaleDialogue);
                _master.Add(dialogueName, femaleDialogue);
            }
        }

        void Update()
        {
            _timeElapsed += Time.unscaledDeltaTime;

            if (_timeElapsed < _currentMusic.clip.length * _musicEndOn) return;
            switch (_musicTypePlayed)
            {
                case MusicTypePlayed.Mood:
                    PlayMusic(_currentMusic.mood);
                    break;
                case MusicTypePlayed.Situation:
                    PlayMusic(_currentMusic.situation);
                    break;
            }
            _timeElapsed = 0;
        }

        public void ChangeVolume(float newVolume, SoundType typeToChange)
        {
            if (typeToChange == SoundType.SFX || typeToChange == SoundType.All)
            {
                var allSfx = _sfxDict.Values.ToList();
                var allSfxCount = allSfx.Count;
                
                if (typeToChange == SoundType.SFX)
                {
                    sfxVolume = newVolume;
                }
                else
                {
                    masterVolume = newVolume;
                }

                for (var i = 0; i < allSfxCount; i++)
                {
                    var sfx = allSfx[i];
                    sfx.source.volume = sfx.volume * masterVolume * sfxVolume;
                }
            }

            if (typeToChange == SoundType.Background || typeToChange == SoundType.All)
            {

                if (typeToChange == SoundType.Background)
                {
                    _bgMusicVolume = newVolume;
                }
                else
                {
                    masterVolume = newVolume;
                }

                if(_currentMusic == null) return;
                _currentMusic.source.volume = _currentMusic.volume * masterVolume * _bgMusicVolume;
            }

            if (typeToChange == SoundType.Ambience || typeToChange == SoundType.All)
            {
                var allAmbience = _ambienceDict.Values.ToList();
                var allAmbienceCount = allAmbience.Count;

                if (typeToChange == SoundType.Ambience)
                {
                    ambienceVolume = newVolume;
                }
                else
                {
                    masterVolume = newVolume;
                }

                for (var i = 0; i < allAmbienceCount; i++)
                {
                    var ambience = allAmbience[i];
                    ambience.source.volume = ambience.volume * masterVolume * sfxVolume;
                }
            }
        }

        public void PlayAmbience(string ambienceName)
        {
            if (!_ambienceDict.TryGetValue(ambienceName.ToUpperInvariant().Replace(" ", string.Empty), out var ambience)) return;
            ambience.source.Play();
        }

        public Ambience GetAmbience(string ambienceName)
        {
            return !_ambienceDict.TryGetValue(ambienceName.ToUpperInvariant().Replace(" ", string.Empty), out var ambience) ? null : ambience;
        }

        public void PlayMusic(string musicName)
        {
            if (!_musicDict.TryGetValue(musicName.ToUpperInvariant().Replace(" ", string.Empty), out var music)) return;
            if (_currentMusic.name == musicName) return;

            StopAllCoroutines();
            StartCoroutine(FadeTrack(music));
            _currentMusic = music;
            _musicTypePlayed = MusicTypePlayed.Name;
        }

        public void PlayMusic(MusicMood mood)
        {
            if (_currentMusic != null && _currentMusic.mood == mood)
            {
                if(_timeElapsed < _currentMusic.clip.length * _musicEndOn) return;
            }

            var musicInMood = new List<Music>();

            foreach (var music in _musicList)
            {
                if(music.mood != mood) continue;
                musicInMood.Add(music);
            }
            
            var musicToPlay = musicInMood[Random.Range(0, musicInMood.Count)];
            if (_currentMusic != null)
            {
                while (musicToPlay.clip == _currentMusic.clip)
                {
                    musicToPlay = musicInMood[Random.Range(0, musicInMood.Count)];
                }
            }

            if (_currentMusic != null && _currentMusic.name == musicToPlay.name) return;

            StopAllCoroutines();
            StartCoroutine(FadeTrack(musicToPlay));
            _currentMusic = musicToPlay;
            _musicTypePlayed = MusicTypePlayed.Mood;
        }

        public void PlayMusic(MusicSituation situation)
        {
            if (_currentMusic != null && _currentMusic.situation == situation)
            {
                if (_timeElapsed < _currentMusic.clip.length * _musicEndOn) return;
            }

            var musicInSituation = new List<Music>();

            foreach (var music in _musicList)
            {
                if (music.situation != situation) continue;
                musicInSituation.Add(music);
            }

            var musicToPlay = musicInSituation[Random.Range(0, musicInSituation.Count)];
            if (_currentMusic != null)
            {
                while (musicToPlay.clip == _currentMusic.clip)
                {
                    musicToPlay = musicInSituation[Random.Range(0, musicInSituation.Count)];
                }
            }


            if (_currentMusic != null && _currentMusic.name == musicToPlay.name) return;

            StopAllCoroutines();
            StartCoroutine(FadeTrack(musicToPlay));
            _currentMusic = musicToPlay;
            _musicTypePlayed = MusicTypePlayed.Situation;
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

        public Sfx GetSFX(string soundName)
        {
            return !_sfxDict.TryGetValue(soundName.ToUpperInvariant().Replace(" ", string.Empty), out var sfx) ? null : sfx;
        }

        private IEnumerator FadeTrack(Music audioToChange)
        {
            var fadeTime = _musicFadeTime;
            var currentAudio = _currentMusic;
            var timeElapsed = 0f;

            audioToChange.source.Play();
            audioToChange.source.volume = 0;

            while (timeElapsed < fadeTime)
            {
                audioToChange.source.volume = Mathf.Lerp(audioToChange.source.volume, audioToChange.volume * _bgMusicVolume * masterVolume, timeElapsed/fadeTime);
                
                if (currentAudio != null)
                {
                    currentAudio.source.volume = Mathf.Lerp(currentAudio.source.volume, 0, timeElapsed / fadeTime);
                }
                
                timeElapsed += Time.unscaledDeltaTime;
                yield return null;
            }
        }
    }
}
