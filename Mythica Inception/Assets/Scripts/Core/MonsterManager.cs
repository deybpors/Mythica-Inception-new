using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Assets.Scripts.Core
{
    public class MonsterManager : MonoBehaviour
    {
        private IHaveMonsters _haveMonsters;
        //TODO: change type from gameobject to whatever the data type name of monster
        [SerializeField]
        private List<GameObject> _monsters;

        private float _timer;
        [HideInInspector] public int currentMonster;

        void Start()
        {
            _haveMonsters = GetComponent<IHaveMonsters>();
            _monsters = _haveMonsters.GetMonsters();
        }

        void Update()
        {
            if (_monsters.Count <= 0) return;
            
            _timer += Time.deltaTime;
            if(_timer < _haveMonsters.GetMonsterSwitchRate()) return;
            int monsterSlotSelected = _haveMonsters.MonsterSwitched();
            if(monsterSlotSelected == currentMonster) return;
            
            if (_monsters[monsterSlotSelected] == null)
            {
                //TODO: Update UI to send message that there is currently no monsters in the selected slot
                Debug.Log("Currently no monsters in the selected slot");
                return;
            }
            
            SwitchMonster(_haveMonsters.MonsterSwitched());
            _timer = 0;
        }

        public void SwitchMonster(int slot)
        {
            InactiveAll();
            _monsters[slot].SetActive(true);
            currentMonster = slot;
            Debug.Log("Monster switched to slot " + (slot + 1));
            //TODO: change the stats of monsters here
        }

        private void InactiveAll()
        {
            foreach (var monster in _monsters)
            {
                monster.SetActive(false);
            }
        }
    }

    [System.Serializable]
    public class MonsterData
    {
        
    }
}
