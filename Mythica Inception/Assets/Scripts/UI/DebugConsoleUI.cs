using System.Collections.Generic;
using System.Linq;
using TMPro;
using UI;
using UnityEngine;

public class DebugConsoleUI : MonoBehaviour
{
    public List<TextMeshProUGUI> debugConsoleItems;
    public Dictionary<TextMeshProUGUI, CanvasGroup> canvasGroups = new Dictionary<TextMeshProUGUI, CanvasGroup>();

    void Update()
    {
        foreach (var consoleItem in debugConsoleItems)
        {
            if(!consoleItem.gameObject.activeInHierarchy) continue;

            canvasGroups.TryGetValue(consoleItem, out var canvasGroup);
            if (canvasGroup == null)
            {
                canvasGroup = consoleItem.GetComponent<CanvasGroup>();
                try
                {
                    canvasGroups.Add(consoleItem, canvasGroup);
                }
                catch
                {
                    //ignored
                }
            }

            var hasCanvasGroup = canvasGroup != null && canvasGroup.alpha == 0;

            if (hasCanvasGroup) consoleItem.gameObject.SetActive(false);
        }
    }

    TextMeshProUGUI FindInactive()
    {
        foreach (var consoleItem in debugConsoleItems)
        {

            if (consoleItem.gameObject.activeInHierarchy) continue;
            
            consoleItem.gameObject.SetActive(false);
            return consoleItem;
        }

        return null;
    }

    public void DisplayLogUI(string logString)
    {
        var consoleItem = FindInactive();
        if (consoleItem == null) return;
        consoleItem.gameObject.SetActive(true);
        consoleItem.text = logString;
    }
}
