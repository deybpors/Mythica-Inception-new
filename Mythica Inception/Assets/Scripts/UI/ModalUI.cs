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
    public Button closeButton;
    public UnityEvent closeAct;

    public void OpenModal(string message, Sprite icon, Color iconColor, UnityAction confirmAction)
    {
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(confirmAction);

        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(closeAct.Invoke);

        messageUI.text = message;
        this.icon.sprite = icon;
        this.icon.color = iconColor;
        overlay.gameObject.SetActive(true);
        modalWhole.gameObject.SetActive(true);
    }

    public void OpenModal(string message, Sprite icon, Color iconColor, UnityAction confirmAction, UnityAction closeAction)
    {
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(confirmAction);

        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(closeAction);

        messageUI.text = message;
        this.icon.sprite = icon;
        this.icon.color = iconColor;
        overlay.gameObject.SetActive(true);
        modalWhole.gameObject.SetActive(true);
    }

    public void CloseModal()
    {
        if(!modalWhole.gameObject.activeInHierarchy && !overlay.gameObject.activeInHierarchy) return;

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
