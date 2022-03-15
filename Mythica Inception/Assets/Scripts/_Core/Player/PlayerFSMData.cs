using UnityEngine;

namespace _Core.Player.Player_FSM
{
    [CreateAssetMenu(menuName = "Player FSM/Player FSM Data")]
    public class PlayerFSMData : ScriptableObject
    {
        public float turnSmoothTime;
        public float speed;
        public float dashTime;
        public float dashSpeed;
        public float attackRate;
        public float monsterSwitchRate = .5f;

        [HideInInspector]
        public float temporaryTurnSmoothTime;
    }
}
