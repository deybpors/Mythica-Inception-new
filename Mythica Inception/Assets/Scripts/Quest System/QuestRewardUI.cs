using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quest_System
{
    public class QuestRewardUI : MonoBehaviour
    {
        public Image icon;
        public TextMeshProUGUI type;
        public TextMeshProUGUI value;

        public void ChangeRewardInfo(Sprite iconSprite, string typeText, string valueText)
        {
            icon.sprite = iconSprite;
            type.text = typeText;
            value.text = valueText;
        }
    }
}
