using System.Collections.Generic;
using _Core.Managers;
using Pluggable_AI.Scripts.General;
using UnityEngine;

public class MonsterStarter : MonoBehaviour
{
    public List<MonsterTamerAI> starters;
    [SerializeField] private string _saveKey = "_starter_";

    private bool _doneStarter;
    private List<GameObject> _starterObjects = new List<GameObject>();
    private int _count;
    private MonsterTamerAI _monsterAi;

    void Start()
    {
        GameManager.instance.saveManager.LoadDataObject(_saveKey, out bool starter);
        _doneStarter = starter;
        if (_doneStarter)
        {
            Destroy(gameObject);
            return;
        }
        _count = starters.Count;
        _monsterAi = GetComponent<MonsterTamerAI>();
        for (var i = 0; i < _count; i++)
        {
            _starterObjects.Add(starters[i].gameObject);
        }
    }
    void Update()
    {
        var caught = GetMonsterCaught();
        
        if (caught < 0) return;
        if(starters[caught] == _monsterAi) return;

        _monsterAi.Die();
        _doneStarter = true;
        GameManager.instance.saveManager.SaveOtherData(_saveKey, _doneStarter);
    }

    int GetMonsterCaught()
    {
        for (var i = 0; i < _count; i++)
        {
            if (_starterObjects[i].activeInHierarchy) continue;
            return i;
        }

        return -1;
    }
}
