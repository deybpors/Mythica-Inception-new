using System;
using _Core.Managers;
using UnityEngine;

public class PlaySFXOnEnable : MonoBehaviour
{

    [SerializeField] private AudioClip _audioClip;

    void OnEnable()
    {
        if(_audioClip == null) return;

        GameManager.instance.audioManager.PlaySFX(_audioClip.name);
    }
}
