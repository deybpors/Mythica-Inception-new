using System.Collections.Generic;
using _Core.Managers;
using Assets.Scripts._Core.Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public Sprite exitIcon;

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

    public void Unstuck()
    {
        var playerTransform = GameManager.instance.player.playerTransform;
        GameManager.instance.TransferToNearestCheckpoint(playerTransform);
    }

    public void Exit()
    {
        GameManager.instance.uiManager.modal.OpenModal("Are you sure you want to go back to the start menu?", exitIcon,
            Color.white, GameManager.instance.BackToStartScreen);
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
