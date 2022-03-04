using _Core.Managers;
using UnityEngine;

namespace Quest_System
{
    public class QuestUI : MonoBehaviour
    {
        public void Accept()
        {
            GameManager.instance.player.playerQuestManager.GiveQuestToPlayer(GameManager.instance.questManager.questSelected);
            BackToGameplay();
        }

        public void Finish()
        {
            var questSelected = GameManager.instance.questManager.questSelected;
            var player = GameManager.instance.player;

            if (!GameManager.instance.player.playerQuestManager.IsQuestSucceeded(questSelected))
            {
                if (questSelected.failedQuest == null) return;
                
                player.playerQuestManager.RemoveQuestToPlayer(questSelected);
                player.playerQuestManager.GiveQuestToPlayer(questSelected.failedQuest);
                
                return;
            }

            player.playerQuestManager.RemoveQuestToPlayer(questSelected);
            player.playerQuestManager.GetQuestRewards(questSelected);
            
            if(questSelected.succeedQuest == null) return;
            player.playerQuestManager.GiveQuestToPlayer(questSelected.succeedQuest);
        }

        public void Decline()
        {
            Debug.Log("Quest Declined");
            BackToGameplay();
        }

        private void BackToGameplay()
        {
            GameManager.instance.player.inputHandler.activate = true;
            GameManager.instance.uiManager.gameplayUICanvas.SetActive(true);
            GameManager.instance.uiManager.questUICanvas.SetActive(false);
        }
    }
}
