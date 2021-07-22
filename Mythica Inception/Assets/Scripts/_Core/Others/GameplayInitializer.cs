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
            GameManager.instance.uiManager.gameplayUICanvas.SetActive(true);
        }
    }
}
