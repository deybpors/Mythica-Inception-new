using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SaveFileUI : MonoBehaviour
    {
        public Button button;
        public TextMeshProUGUI playerName;
        public TextMeshProUGUI saveFileInfo;

        public void SetSaveFileData(string saveFileName, string saveInfo)
        {
            playerName.text = saveFileName;
            saveFileInfo.text = saveInfo;
        }
    }
}