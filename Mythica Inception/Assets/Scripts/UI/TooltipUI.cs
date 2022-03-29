using TMPro;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TooltipUI : MonoBehaviour
{
    public RectTransform toolTipTransform;
    public UITweener tooltipTweener;
    public TextMeshProUGUI titleField;
    public TextMeshProUGUI contentField;
    public LayoutElement layoutElement;

    public int characterWrapLimit;

    void Update()
    {
        if (Application.isEditor)
        {
            WrapText();
        }

        PositionToolTip();
    }

    private void PositionToolTip()
    {
        var position = Mouse.current.position.ReadValue();

        var pivotX = position.x / (float) Screen.width;
        var pivotY = position.y / (float) Screen.height;

        toolTipTransform.pivot = new Vector2(pivotX - .2f, pivotY - .2f);
        toolTipTransform.position = position;
    }

    public void ShowToolTip(string title, string content)
    {
        if(title.Equals(string.Empty) && content.Equals(string.Empty)) return;

        titleField.text = title;
        contentField.text = content;

        toolTipTransform.gameObject.SetActive(true);
        WrapText();
    }

    public void HideToolTip()
    {
        toolTipTransform.gameObject.SetActive(false);
    }

    private void WrapText()
    {
        var titleFieldLength = titleField.text.Length;
        var contentFieldLength = contentField.text.Length;

        layoutElement.enabled = (titleFieldLength > characterWrapLimit || contentFieldLength > characterWrapLimit);
    }
}
