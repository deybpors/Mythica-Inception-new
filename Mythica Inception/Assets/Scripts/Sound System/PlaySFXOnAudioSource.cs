using _Core.Managers;
using Assets.Scripts.Sound_System;
using SoundSystem;
using UnityEngine;

public class PlaySFXOnAudioSource : MonoBehaviour
{
    [SerializeField] private string _sfxName;
    [SerializeField] private bool _playOnEnable = true;
    [SerializeField] private bool _loop;

    private AudioManager _am;
    private Sfx _sfx;
    private AudioSource _source;
    void Start()
    {
        Init();
    }

    private void Init()
    {
        _source = GetComponent<AudioSource>();
        _am = GameManager.instance.audioManager;
        _sfx = _am.GetSFX(_sfxName);
    }

    void OnEnable()
    {
        if(!_playOnEnable) return;
        
        if (_source == null || _am == null || _sfx == null) Init();


        var pitch = _sfx.pitch;
        if (_sfx.randomPitch)
        {
            pitch = Random.Range(_sfx.min, _sfx.max);
        }

        _source.pitch = pitch;
        _source.volume = _sfx.volume * _am.masterVolume * _am.sfxVolume;
        _source.loop = _loop;
        
        if (!_loop)
        {
            _source.PlayOneShot(_sfx.clip);
        }
        else
        {
            _source.clip = _sfx.clip;
        }
    }
}
