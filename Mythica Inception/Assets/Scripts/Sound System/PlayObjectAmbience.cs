using System.Collections;
using System.Collections.Generic;
using _Core.Managers;
using Assets.Scripts.Sound_System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayObjectAmbience : MonoBehaviour
{
    [SerializeField] private string _ambienceToPlay;
    private Transform _playerTransform;
    private Transform _thisTransform;
    private AudioSource _audioSource;
    private Ambience _ambience;

    void OnEnable()
    {
        if (_playerTransform != null && _thisTransform != null && _audioSource != null) return;
        
        _thisTransform = transform;
        _audioSource = GetComponent<AudioSource>();
        _ambience = GameManager.instance.audioManager.GetAmbience(_ambienceToPlay);
        _audioSource.clip = _ambience.clip;
        if(!_audioSource.isPlaying) _audioSource.Play();

        if (GameManager.instance.player == null) return;
        _playerTransform = GameManager.instance.player.transform;
    }

    void Update()
    {
        UpdateVolume();
    }

    private void UpdateVolume()
    {
        if (_playerTransform == null || _thisTransform == null)
        {
            enabled = false;
            enabled = true;
            return;
        }

        var distance = Vector3.Distance(_playerTransform.position, _thisTransform.position);
        if (distance > 30f) return;

        _audioSource.volume = _ambience.source.volume;

    }
}
