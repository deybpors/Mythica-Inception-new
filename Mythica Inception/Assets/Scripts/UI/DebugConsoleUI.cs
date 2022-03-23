using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DebugConsoleUI : MonoBehaviour
{
    public List<TextMeshProUGUI> debugConsoleItems;

    TextMeshProUGUI FindInactive()
    {
        return debugConsoleItems.FirstOrDefault(consoleItem => !consoleItem.gameObject.activeInHierarchy);
    }

    public void DisplayLogUI(string logString)
    {
        var consoleItem = FindInactive();
        consoleItem.text = logString;
        consoleItem.gameObject.SetActive(true);
    }
}
