using _Core.Managers;
using Assets.Scripts._Core.Player;
using UnityEngine;

namespace _Core.Others
{
    public class GameSceneInitializer : MonoBehaviour
    {
        public Player player;
        public Camera currentWorldCamera;
        private void Awake()
        {
            if(GameManager.instance == null) return;
        
            GameManager.instance.InitializePlayerReference(player);
            GameManager.instance.InitializeCurrentWorldCamera(currentWorldCamera);
        }
    }
}
