using _Core.Managers;
using UnityEngine;

namespace _Core.Others
{
    public class GameplayInitializer : MonoBehaviour
    {
        public Material skybox;
        public Player.Player player;
        private void Start()
        {
            if(GameManager.instance == null) return;
            if(GameManager.instance.uiManager == null) return;
            if(GameManager.instance.uiManager.gameplayUICanvas == null) return;
            GameManager.instance.uiManager.gameplayUICanvas.SetActive(true);
        }
    }
}
