using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Gameplay UI")] 
        public GameObject gameplayUICanvas;
        public ProgressBarUI currentCharHealth;
        public ProgressBarUI currentMonsterExp;
        public List<Image> currentMonsterSkillImages;
        public List<Image> currentMonsterItemImages;

        [Header("Dialogue UI")]
        public GameObject dialogueUICanvas;
        public DialogueManager dialogueManager;
        
    }
}
