using System;
using _Core.Managers;
using _Core.Others;
using _Core.Player;
using Quest_System;
using UnityEngine;

namespace Dialogue_System
{
    public class NPCDialogue : MonoBehaviour, IInteractable, ISelectable
    {
        [SerializeField] private string npcName = "No Name";
        [SerializeField] private TextAsset inkJsonFile;
        [SerializeField] private float speed;
        private Quaternion npcTo;
        private Quaternion playerTo;
        private Transform npcTransform;
        private bool turn = false;
        private QuestGiver questGiver;
        private Transform playerTransform;

        public void Interact(Player player)
        {
            GameManager.instance.uiManager.gameplayUICanvas.SetActive(false);

            if (questGiver == null)
            {
                var qGiver = GetComponent<QuestGiver>();
                questGiver = qGiver;
            }

            if (!GameManager.instance.questManager.QuestAcceptedByPlayer(questGiver.questToGive))
            {
                GameManager.instance.uiManager.dialogueManager.StartDialogue(inkJsonFile, npcName, player, questGiver);
                GameManager.instance.uiManager.dialogueUICanvas.SetActive(true);
            }
            else
            {
                questGiver.OpenQuestWindow(true);
            }
            
            if (playerTransform == null)
            {
                playerTransform = player.transform;
            }

            if (npcTransform == null)
            {
                npcTransform = transform;
            }

            var playerPosition = playerTransform.position;
            var npcPosition = npcTransform.position;
            
            playerTransform.LookAt(npcTransform);
            npcTo = Quaternion.LookRotation(playerPosition - npcPosition);
            
            turn = true;
        }

        private void Update()
        {
            HandleTurn();
        }

        private void HandleTurn()
        {
            if (!turn) return;

            npcTransform.rotation = Quaternion.Slerp(npcTransform.rotation, npcTo, Time.deltaTime * speed);

            if (npcTransform.rotation.eulerAngles == npcTo.eulerAngles)
            {
                turn = false;
            }
        }
    }
}
