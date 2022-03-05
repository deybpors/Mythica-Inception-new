using TMPro;
using UnityEngine;

public class DebugConsoleUI : MonoBehaviour
{
    public TextMeshProUGUI debugConsoleItem;
    public GameObject debugConsoleInput;
    private Transform debugConsoleInputTrans;

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
        debugConsoleInputTrans = debugConsoleInput.transform;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        var consoleItem = Instantiate(debugConsoleItem, transform);
        consoleItem.text = logString;
    }
}
