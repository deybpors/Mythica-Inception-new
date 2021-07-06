using UnityEngine;

namespace Assets.Scripts.Monster_System
{
    public class TameValue : MonoBehaviour, ITameable
    {
        public int maxTameValue;
        public int currentTameValue;
        [HideInInspector] public Monster wildMonster;

        void Start()
        {
            InitializeMonsterData();
        }

        private void InitializeMonsterData()
        {
            //TODO: Get monster's data and level here
            
        }

        public void AddCurrentTameValue(int tameBeamValue)
        {
            currentTameValue += tameBeamValue;

            if (currentTameValue >= maxTameValue)
            {
                Tamed();
            }
        }

        public void Tamed()
        {
            //TODO: make this monster captured
        }
    }
}
