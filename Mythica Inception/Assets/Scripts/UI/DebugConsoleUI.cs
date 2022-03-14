using _Core.Managers;
using TMPro;
using UnityEngine;

public class DebugConsoleUI : MonoBehaviour
{
    public TextMeshProUGUI debugConsoleItem;
    public GameObject debugConsoleInput;
    private Transform debugConsoleInputTrans;

    void OnEnable()
    {
        debugConsoleInputTrans = debugConsoleInput.transform;
    }

    public void DisplayLogUI(string logString)
    {
        var consoleItem = Instantiate(debugConsoleItem, transform);
        consoleItem.text = logString;
    }
}
