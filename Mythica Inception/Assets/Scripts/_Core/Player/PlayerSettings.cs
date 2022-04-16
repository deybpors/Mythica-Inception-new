using _Core.Player.Player_FSM;
using Monster_System;
using UnityEngine;

namespace Assets.Scripts._Core.Player
{
    [CreateAssetMenu(menuName = "Player/Player Settings")]
    public class PlayerSettings : ScriptableObject
    {
        public float pauseTimeScale = .1f;
        public GameObject deathParticles;
        public GameObject male;
        public GameObject female;
        public Sprite deathIcon;
        public TameBeam tameBeam;
        public GameObject dashGraphic;
        public PlayerFSMData playerData;
        public float tameRadius;
        public Vector3 savePositionOffset = new Vector3(0, .3f, 0);
    }
}
