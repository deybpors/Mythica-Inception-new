using Assets.Scripts._Core.Managers;
using Assets.Scripts._Core.Others;
using Assets.Scripts._Core.Player;
using Assets.Scripts.Dialogue_System;
using UnityEngine;

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
        (playerTransform = player.transform).LookAt(transform);
        transform.LookAt(playerTransform);
        var eulerAngles = playerTransform.eulerAngles;
        playerTransform.rotation = Quaternion.Euler(0, eulerAngles.y, eulerAngles.z);
    }
}
