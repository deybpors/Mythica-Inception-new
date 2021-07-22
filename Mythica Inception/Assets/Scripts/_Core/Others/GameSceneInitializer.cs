using _Core.Managers;
using UnityEngine;

namespace _Core.Others
{
    public class GameSceneInitializer : MonoBehaviour
    {
        public Player.Player player;
        public Camera currentWorldCamera;
        private void Awake()
        {
            if(GameManager.instance == null) return;
        
            GameManager.instance.InitializePlayerReference(player);
            GameManager.instance.InitializeCurrentWorldCamera(currentWorldCamera);
        }
    }
}
