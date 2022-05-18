using System.Collections.Generic;
using _Core.Managers;
using Assets.Scripts.UI;
using Databases.Scripts;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DebugConsoleUI : MonoBehaviour
{
    public TextMeshProUGUI DDAText;
    public TMP_InputField debugField;
    private GameObject _debugFieldObject;
    public List<DebugCommand> debugCommands;
    public List<TextMeshProUGUI> debugConsoleItems;
    public Dictionary<TextMeshProUGUI, CanvasGroup> canvasGroups = new Dictionary<TextMeshProUGUI, CanvasGroup>();
    private Dictionary<TextMeshProUGUI, GameObject> _consoleItemObjects = new Dictionary<TextMeshProUGUI, GameObject>();
    private Dictionary<string, DebugCommand> _commands = new Dictionary<string, DebugCommand>();
    private int _consoleItemCount;
    private string[] _currentCommandString;
    [ReadOnly] public bool showConsole;
    private DatabaseManager _databaseManager;

    void Start()
    {
        _debugFieldObject = debugField.gameObject;
        _consoleItemCount = debugConsoleItems.Count;
        var commandsCount = debugCommands.Count;
        for (var i = 0; i < commandsCount; i++)
        {
            _commands.Add(debugCommands[i].commandId.ToLowerInvariant().Replace(" ", string.Empty), debugCommands[i]);
        }

        _databaseManager = GameManager.instance.databaseManager;
    }

    public void OnToggleDebug()
    {
        showConsole = !showConsole;
        _debugFieldObject.SetActive(showConsole);
        EventSystem.current.SetSelectedGameObject(_debugFieldObject, null);
        debugField.OnPointerClick(new PointerEventData(EventSystem.current));
    }

    public void OnEnterDebug()
    {
        if(debugField.text == string.Empty) return;

        _currentCommandString = debugField.text.Split(' ');
        if (!_commands.TryGetValue(_currentCommandString[0], out var command))
        {
            ResetDebugConsole();
            return;
        }
        
        command.InvokeCommand();
        ResetDebugConsole();
    }

    private void ResetDebugConsole()
    {
        DisplayLogUI(debugField.text);
        debugField.text = string.Empty;
        OnToggleDebug();
    }

    #region Commands
    public void UpdateQuest()
    {
        if (!int.TryParse(_currentCommandString[2], out var amount))
        {
            amount = 1;
        }

        var objectToFind = _currentCommandString[1].ToLowerInvariant().Replace(" ", string.Empty);

        switch (_currentCommandString[0])
        {
            case "update_quest_kill":
                if (_databaseManager.monsterDictionary.TryGetValue(objectToFind, out var monster))
                {
                    for (var i = 0; i < amount; i++)
                    {
                        GameManager.instance.questManager.UpdateKillQuest(monster);
                    }
                }
                else
                {
                    if (objectToFind == "any")
                    {
                        for (var i = 0; i < amount; i++)
                        {
                            GameManager.instance.questManager.UpdateKillQuest(null);
                        }
                    }
                }
                break;
            case "update_quest_gather":
                if (_databaseManager.itemsDictionary.TryGetValue(objectToFind, out var item))
                {
                    GameManager.instance.player.playerInventory.AddItemInPlayerInventory(item, amount);
                }
                break;
        }
    }
    #endregion

    void Update()
    {
        for (var i = 0; i < _consoleItemCount; i++)
        {
            var consoleItem = debugConsoleItems[i];
            if (!consoleItem.gameObject.activeInHierarchy) continue;

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
        if(logString == string.Empty) return;
        if (!_consoleItemObjects.TryGetValue(consoleItem, out var obj))
        {
            obj = consoleItem.gameObject;
            _consoleItemObjects.Add(consoleItem, obj);
        }

        obj.SetActive(true);
        consoleItem.text = logString;
    }
}
