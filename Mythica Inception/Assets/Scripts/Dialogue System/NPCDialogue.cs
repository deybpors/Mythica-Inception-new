using System;
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
        [SerializeField] private float speed;
        private Quaternion to;
        private Transform npcTransform;
        private bool turn = false;
        public void Interact(Player player)
        {
            GameManager.instance.uiManager.gameplayUICanvas.SetActive(false);
            GameManager.instance.uiManager.dialogueManager.StartDialogue(inkJsonFile, npcName, player);
            GameManager.instance.uiManager.dialogueUICanvas.SetActive(true);

            Transform playerTrans;
            (playerTrans = player.transform).LookAt(transform);
            npcTransform = transform;
            to = Quaternion.LookRotation(playerTrans.position - npcTransform.position);
            turn = true;
        }

        private void Update()
        {
            if(!turn) return;

            npcTransform.rotation = Quaternion.Slerp(npcTransform.rotation, to, Time.deltaTime * speed);
            if (npcTransform.rotation.eulerAngles == to.eulerAngles)
            {
                turn = false;
            }
        }
    }
}
