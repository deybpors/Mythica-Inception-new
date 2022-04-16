using System.Collections;
using System.Collections.Generic;
using _Core.Managers;
using UnityEngine;

public class SetMainLight : MonoBehaviour
{
    [SerializeField] private float _lightIntensity;
    private bool _set;

    void Update()
    {
        if (_set) return;
        if(GameManager.instance == null) return;
        if(GameManager.instance.mainLight == null) return;
        GameManager.instance.mainLight.intensity = _lightIntensity;
        _set = true;
    }
}
