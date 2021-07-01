using UnityEngine;

namespace Assets.Scripts.Core.Player_FSM
{
    [CreateAssetMenu(menuName = "Player FSM/Player FSM Data")]
    public class PlayerFSMData : ScriptableObject
    {
        public float turnSmoothTime;
        public float speed;
        public float dashTime;
        public float dashSpeed;
        public float attackRate;
    }
}
