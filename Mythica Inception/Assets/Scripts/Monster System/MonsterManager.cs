using System.Collections.Generic;
using System.Linq;
using Assets.Scripts._Core;
using Assets.Scripts._Core.Player;
using Assets.Scripts.Skill_System;
using UnityEngine;

namespace Assets.Scripts.Monster_System
{
    [RequireComponent(typeof(IHaveMonsters))]
    public class MonsterManager : MonoBehaviour
    {
        #region Hidden Fields

        [SerializeField] private bool _activated;
        [SerializeField] private List<Monster> _monsters;
        [HideInInspector] public bool isPlayer;
        private SkillManager _skillManager;
        private List<GameObject> _monsterGameObjects = new List<GameObject>();
        private IHaveMonsters _haveMonsters;
        private List<Animator> _monsterAnimators = new List<Animator>();
        private GameObject _tamerPrefab;
        private Animator _tamerAnimator;
        private float _timer;
        [SerializeField]
        private int _currentMonster;
        private float _tamerTameRadius;
        private Player _player;
        private bool _tamerBefore;
        
        #endregion
        
        public void ActivateMonsterManager(IHaveMonsters haveMonsters, SkillManager skillManager)
        {
            _currentMonster = -1;
            _haveMonsters = haveMonsters;
            
            //meaning it is a player since only the player has a tamer
            if (_haveMonsters.GetTamer() != null)
            {
                isPlayer = true;
                _tamerPrefab = _haveMonsters.GetTamer();
                _player = GetComponent<Player>();
                _tamerTameRadius = _player.tameRadius;
                _tamerBefore = true;
                _tamerAnimator = _tamerPrefab.GetComponent<Animator>();
            }
            
            _skillManager = skillManager;
            _monsterGameObjects = new List<GameObject>();
            RequestPoolMonstersPrefab();
            GetMonsterAnimators();
            
            _activated = true;
        }

        public void RequestPoolMonstersPrefab()
        {
            if (_monsterGameObjects.Count > 0)
            {
                foreach (var monsterObj in _monsterGameObjects)
                {
                    GameManager.instance.pooler.BackToPool(monsterObj);
                }
                _monsterGameObjects.Clear();
            }
            
            _monsters = _haveMonsters.GetMonsters();
            for (int i = 0; i < _monsters.Count; i++)
            {
                if(_monsters[i]==null) continue;
                
                GameObject monsterObj = GameManager.instance.pooler.SpawnFromPool(transform,
                    _monsters[i].monsterName, _monsters[i].monsterPrefab, Vector3.zero, Quaternion.identity);
                monsterObj.SetActive(false);
                if (isPlayer)
                {
                    monsterObj.GetComponentInChildren<Renderer>().gameObject.layer = 11;
                }
                _monsterGameObjects.Add(monsterObj);
            }
        }

        public void GetMonsterAnimators()
        {
            if (_monsterAnimators.Count > 0)
            {
                _monsterAnimators.Clear();
            }
            _monsterAnimators = _monsterGameObjects.Select(monster => monster.GetComponent<Animator>()).ToList();
        }

        void Update()
        {
            if(!_activated) return;
            
            if (_monsters.Count <= 0) return;
            
            _timer += Time.deltaTime;
            if(_timer < _haveMonsters.GetMonsterSwitchRate()) return;
            
            if (_haveMonsters.isPlayerSwitched())
            {
                if(_tamerBefore) return;
                
                SwitchToTamer();
                _tamerBefore = true;
                return;
            }
            
            var monsterSlotSelected = _haveMonsters.CurrentMonsterSlotNumber();
            if(monsterSlotSelected == _currentMonster) return;

            _tamerBefore = false;
            SwitchMonster(monsterSlotSelected);
            _timer = 0;
        }

        public void SwitchToTamer()
        {
            if (_tamerPrefab == null) return;
            InactiveAllMonsters();
            _tamerPrefab.SetActive(true);
            _haveMonsters.SetAnimator(_tamerAnimator);
            _skillManager.activated = false;
            _player.unitIndicator.transform.localScale =
                new Vector3(_tamerTameRadius, _tamerTameRadius, _tamerTameRadius);
            _currentMonster = -1;
            _haveMonsters.SpawnSwitchFX();
            _haveMonsters.ChangeStatsToMonster(_currentMonster);
        }

        public void SwitchMonster(int slot)
        {
            InactiveAllMonsters();
            _monsterGameObjects[slot].transform.rotation = transform.rotation;
            _monsterGameObjects[slot].SetActive(true);
            _haveMonsters.SetAnimator(_monsterAnimators[slot]);
            _currentMonster = slot;
            _haveMonsters.ChangeMonsterUnitIndicatorRadius(_haveMonsters.GetMonsters()[slot].basicAttackSkill.castRadius);
            _haveMonsters.SpawnSwitchFX();
            _skillManager.ActivateSkillManager(_haveMonsters);
            _haveMonsters.ChangeStatsToMonster(slot);
        }

        private void InactiveAllMonsters()
        {
            foreach (var monster in _monsterGameObjects)
            {
                monster.SetActive(false);
            }
            _tamerPrefab.SetActive(false);
        }
    }
}
