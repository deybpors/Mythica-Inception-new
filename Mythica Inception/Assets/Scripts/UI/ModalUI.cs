using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ModalUI : MonoBehaviour
{
    public UITweener overlay;
    public UITweener modalWhole;
    public TextMeshProUGUI messageUI;
    public Image icon;
    public Button confirmButton;

    public void OpenModal(string message, Sprite icon, UnityAction confirmAction)
    {
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(confirmAction);

        messageUI.text = message;
        this.icon.sprite = icon;
        overlay.gameObject.SetActive(true);
        modalWhole.gameObject.SetActive(true);
    }

    public void CloseModal()
    {
        try
        {
            overlay.Disable();
            modalWhole.Disable();
        }
        catch
        {
            // ignored
        }
    }
}
