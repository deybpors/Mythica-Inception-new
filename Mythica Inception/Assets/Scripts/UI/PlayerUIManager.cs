using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class PlayerUIManager : MonoBehaviour
    {
        public ProgressBarUI currentCharHealth;
        public ProgressBarUI currentMonsterExp;
        public List<Image> currentMonsterSkillImages;
        public List<Image> currentMonsterItemImages;
    }
}
