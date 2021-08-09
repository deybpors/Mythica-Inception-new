using _Core.Managers;
using UnityEngine;

namespace Quest_System
{
    public class QuestUI : MonoBehaviour
    {
        public void Accept()
        {
            GameManager.instance.player.GiveQuestToPlayer(GameManager.instance.questManager.questSelected);
            BackToGameplay();
        }

        public void Finish()
        {
            if (GameManager.instance.player.IsQuestFinish(GameManager.instance.questManager.questSelected))
            {
                GameManager.instance.player.GetQuestRewards(GameManager.instance.questManager.questSelected);
            }   
        }

        public void Decline()
        {
            Debug.Log("Quest Declined");
            BackToGameplay();
        }

        public void BackToGameplay()
        {
            GameManager.instance.player.inputHandler.activate = true;
            GameManager.instance.uiManager.gameplayUICanvas.SetActive(true);
            GameManager.instance.uiManager.questUICanvas.SetActive(false);
        }
    }
}
