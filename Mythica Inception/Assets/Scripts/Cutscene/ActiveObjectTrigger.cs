using System.Collections.Generic;
using Dialogue_System;
using UnityEngine;

public class ActiveObjectTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private List<GameObject> _objectToCheck;
    [SerializeField] private bool inverse;
    [SerializeField] private List<MonoBehaviour> _componentsToEnable;
    private int _componentsCount;
    private int _objectToCheckCount;

    void Start()
    {
        _objectToCheckCount = _objectToCheck.Count;
        _componentsCount = _componentsToEnable.Count;
    }

    void Update()
    {
        var activate = true;

        for (var i = 0; i < _objectToCheckCount; i++)
        {
            activate = activate && _objectToCheck[i] != null;
            activate = activate && _objectToCheck[i].activeInHierarchy;
        }

        activate = inverse ? !activate : activate;

        if (_componentsCount > 0)
        {
            for (var i = 0; i < _componentsCount; i++)
            {
                _componentsToEnable[i].enabled = activate;
            }
        }

        if(_prefab == null) return;
        _prefab.SetActive(activate);
    }
}
