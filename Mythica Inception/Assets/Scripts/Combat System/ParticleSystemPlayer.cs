using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemPlayer : MonoBehaviour
{
    public bool playOnEnable;
    private ParticleSystem _particleSystem;

    void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    void OnEnable()
    {
        if (_particleSystem != null && playOnEnable) Play(true);
    }

    public void Play(bool withChildren)
    {
        _particleSystem.Play(withChildren);
    }
}
