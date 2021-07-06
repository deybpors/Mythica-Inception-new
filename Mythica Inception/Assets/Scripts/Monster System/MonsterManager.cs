using System.Collections.Generic;
using System.Linq;
using Assets.Scripts._Core;
using Assets.Scripts.Skill_System;
using UnityEngine;

namespace Assets.Scripts.Monster_System
{
    [RequireComponent(typeof(IHaveMonsters))]
    public class MonsterManager : MonoBehaviour
    {
        [SerializeField] private bool _activated;
        [SerializeField] private List<Monster> _monsters;

        #region Hidden Fields

        [HideInInspector] public bool isPlayer;
        private SkillManager _skillManager;
        private List<GameObject> _monsterGameObjects;
        private IHaveMonsters _haveMonsters;
        private List<Animator> _monsterAnimators;
        private GameObject _tamerPrefab;
        private Animator _tamerAnimator;
        private float _timer;
        [SerializeField]
        private int _currentMonster;
        #endregion
        
        public void ActivateMonsterManager()
        {
            _currentMonster = -1;
            _haveMonsters = GetComponent<IHaveMonsters>();
            _skillManager = GetComponent<SkillManager>();
            if (_haveMonsters.GetTamer() != null)
            {
                isPlayer = true;
                _tamerPrefab = _haveMonsters.GetTamer().gameObject;
            }
            
            _monsters = _haveMonsters.GetMonsters();
            _monsterGameObjects = new List<GameObject>();
            RequestPoolMonstersPrefab();
            _monsterAnimators = GetMonsterAnimators();
            if (_tamerPrefab == null) return;
            _tamerAnimator = _tamerPrefab.GetComponent<Animator>();
            _activated = true;
        }

        private void RequestPoolMonstersPrefab()
        {
            for (int i = 0; i < _monsters.Count; i++)
            {
                GameObject monsterObj = GameManager.instance.pooler.SpawnFromPool(transform,
                    _monsters[i].monsterName, _monsters[i].monsterPrefab, Vector3.zero, Quaternion.identity);
                monsterObj.SetActive(false);
                _monsterGameObjects.Add(monsterObj);
            }
        }

        private List<Animator> GetMonsterAnimators()
        {
            return _monsterGameObjects.Select(monster => monster.GetComponent<Animator>()).ToList();
        }

        void Update()
        {
            if(!_activated) return;
            
            if (_monsters.Count <= 0) return;
            
            _timer += Time.deltaTime;
            if(_timer < _haveMonsters.GetMonsterSwitchRate()) return;
            
            if (_haveMonsters.isPlayerSwitched())
            {
                SwitchToTamer();
                return;
            }
            
            int monsterSlotSelected = _haveMonsters.MonsterSwitched();
            if(monsterSlotSelected == _currentMonster) return;
            if (_monsterGameObjects[monsterSlotSelected] == null)
            {
                if (isPlayer)
                {
                    //TODO: Update UI to send message that there is currently no monsters in the selected slot
                    Debug.Log("Currently no monsters in the selected slot"); 
                }
                return;
            }

            SwitchMonster(_haveMonsters.MonsterSwitched());
            _timer = 0;
        }

        private void SwitchToTamer()
        {
            if (_tamerPrefab == null) return;
            InactiveAll();
            _tamerPrefab.SetActive(true);
            _haveMonsters.SetAnimator(_tamerAnimator);
            _skillManager.activated = false;
            _currentMonster = -1;
        }

        public void SwitchMonster(int slot)
        {
            InactiveAll();
            
            _skillManager.ActivateSkillManager();
            
            _monsterGameObjects[slot].SetActive(true);
            
            _haveMonsters.SetAnimator(_monsterAnimators[slot]);

            _currentMonster = slot;
            ChangeMonsterStats(slot);
        }

        private void ChangeMonsterStats(int slot)
        {
            //TODO: change the stats of monsters here
        }

        private void InactiveAll()
        {
            foreach (var monster in _monsterGameObjects)
            {
                monster.SetActive(false);
            }
            _tamerPrefab.SetActive(false);
        }
    }
}
