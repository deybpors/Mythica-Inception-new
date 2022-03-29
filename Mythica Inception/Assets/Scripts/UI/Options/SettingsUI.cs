using System.Collections.Generic;
using _Core.Managers;
using Assets.Scripts._Core.Player;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private List<GameObject> _objectsToDisable;

    void OnEnable()
    {
        if (_objectsToDisable.Count <= 0) return;
        
        foreach (var obj in _objectsToDisable)
        {
            obj.SetActive(false);
        }
    }

    public void ToggleButton(Button button)
    {
        button.interactable = !button.interactable;
    }

    public void VerifySaveButton(Button button)
    {
        button.interactable = GameManager.instance.saveManager.VerifySaving();
    }

    public void SaveButton()
    {
        GameManager.instance.saveManager.SavePlayerData(GameManager.instance.player.GetCurrentSaveData());
    }

    void OnDisable()
    {
        SaveButton();

        if (_objectsToDisable.Count <= 0) return;

        foreach (var obj in _objectsToDisable)
        {
            obj.SetActive(true);
        }
    }
}
