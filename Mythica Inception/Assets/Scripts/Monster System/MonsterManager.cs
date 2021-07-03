using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Core;
using UnityEngine;

namespace Assets.Scripts.Monster_System
{
    [RequireComponent(typeof(IHaveMonsters))]
    public class MonsterManager : MonoBehaviour
    {
        //TODO: change type from gameobject to whatever the data type name of monster
        [SerializeField]
        private List<GameObject> _monstersPrefabs;
        private IHaveMonsters _haveMonsters;
        private List<Animator> _monsterAnimators;
        private GameObject _tamerPrefab;
        private Animator _tamerAnimator;
        private float _timer;
        private int _currentMonster;

        void Start()
        {
            Init();
        }

        void OnEnable()
        {
            Init();
        }
        private void Init()
        {
            var tamer = FindChildTag.FindChildWithTag(transform, "Tamer");
            if (tamer != null) { _tamerPrefab = tamer.gameObject; }
            _haveMonsters = GetComponent<IHaveMonsters>();
            _monstersPrefabs = _haveMonsters.GetMonsters();
            _monsterAnimators = GetMonsterAnimators();
            if (_tamerPrefab == null) return;
            _tamerAnimator = _tamerPrefab.GetComponent<Animator>();
        }

        private List<Animator> GetMonsterAnimators()
        {
            return _monstersPrefabs.Select(monster => monster.GetComponent<Animator>()).ToList();
        }

        void Update()
        {
            if (_monstersPrefabs.Count <= 0) return;
            
            _timer += Time.deltaTime;
            if(_timer < _haveMonsters.GetMonsterSwitchRate()) return;
            
            int monsterSlotSelected = _haveMonsters.MonsterSwitched();
            
            if (_haveMonsters.isPlayerSwitched())
            {
                SwitchToPlayer(); 
                return;
            }
            
            if(monsterSlotSelected == _currentMonster) return;
            
            if (_monstersPrefabs[monsterSlotSelected] == null)
            {
                //TODO: Update UI to send message that there is currently no monsters in the selected slot
                Debug.Log("Currently no monsters in the selected slot");
                return;
            }
            
            SwitchMonster(_haveMonsters.MonsterSwitched());
            _timer = 0;
        }

        private void SwitchToPlayer()
        {
            if (_tamerPrefab == null) return;
            InactiveAll();
            _tamerPrefab.SetActive(true);
            _haveMonsters.SetAnimator(_tamerAnimator);
        }

        public void SwitchMonster(int slot)
        {
            InactiveAll();
            _monstersPrefabs[slot].SetActive(true);
            _currentMonster = slot;
            Debug.Log("Monster switched to slot " + (slot + 1));
            _haveMonsters.SetAnimator(_monsterAnimators[slot]);
            //TODO: change the stats of monsters here
        }

        private void InactiveAll()
        {
            foreach (var monster in _monstersPrefabs)
            {
                monster.SetActive(false);
            }
            _tamerPrefab.SetActive(false);
        }
    }
}
