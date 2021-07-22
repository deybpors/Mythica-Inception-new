using _Core.Managers;
using _Core.Others;
using _Core.Player;
using UnityEngine;

namespace Dialogue_System
{
    public class NPCDialogue : MonoBehaviour, IInteractable, ISelectable
    {
        [SerializeField] private string npcName = "No Name";
        [SerializeField] private TextAsset inkJsonFile;
    
        public void Interact(Player player)
        {
            GameManager.instance.uiManager.gameplayUICanvas.SetActive(false);
            GameManager.instance.uiManager.dialogueManager.StartDialogue(inkJsonFile, npcName, player);
            GameManager.instance.uiManager.dialogueUICanvas.SetActive(true);
            Transform playerTransform;
            Transform npcTransform;
            (playerTransform = player.transform).LookAt(transform);
            (npcTransform = transform).LookAt(playerTransform);
            var playerEulerAngles = playerTransform.eulerAngles;
            var npcEulerAngles = npcTransform.eulerAngles;
            npcTransform.rotation = Quaternion.Euler(0, npcEulerAngles.y, npcEulerAngles.z);
            playerTransform.rotation = Quaternion.Euler(0, playerEulerAngles.y, playerEulerAngles.z);
        }
    }
}
