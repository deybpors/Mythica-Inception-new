using System.Collections;
using System.Collections.Generic;
using _Core.Managers;
using Assets.Scripts.Sound_System;
using MyBox;
using UnityEngine;

public class AnimationSFX : MonoBehaviour
{
    [SerializeField] private string _grassStep = "Grass Step";
    [SerializeField] private string _sandStep = "Sand Step";
    [SerializeField] private string _concreteStep = "Concrete Step";
    [SerializeField] private string _rockStep = "Rock Step";
    private TerrainDetector _terrainDetector;
    private Transform _parentTransform;
    private Transform _thisTransform;
    private AudioSource _audioSource;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _thisTransform = transform;
        _terrainDetector = new TerrainDetector();
        _parentTransform = _thisTransform.parent;
        if (_parentTransform == null)
        {
            _parentTransform = _thisTransform;
        }
    }

    private void Step()
    {
        if (_audioSource == null || GameManager.instance == null) return;

        var sfx = GameManager.instance.audioManager.GetSFX(GetSoundStepName());
        Play(sfx);
    }

    private void Flap()
    {
        if (_audioSource == null || GameManager.instance == null) return;

        var sfx = GameManager.instance.audioManager.GetSFX("Flap");
        Play(sfx);
    }

    private void Play(Sfx sfx)
    {
        _audioSource.volume = sfx.volume * GameManager.instance.audioManager.sfxVolume * GameManager.instance.audioManager.masterVolume;
        _audioSource.pitch = sfx.randomPitch ? Random.Range(sfx.min, sfx.max) : sfx.pitch;
        _audioSource.PlayOneShot(sfx.clip);
    }

    private string GetSoundStepName()
    {
        var index = _terrainDetector.GetActiveTerrainTextureIdx(_parentTransform.position);

        return index switch
        {
            0 => _grassStep,
            1 => _rockStep,
            2 => _concreteStep,
            3 => _sandStep,
            _ => _concreteStep
        };
    }
}
