using _Core.Managers;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private LTDescr _delay;
    public string title;
    
    [TextArea(10,5)]
    public string content;

    public void SetTitleContent(string title, string content)
    {
        this.title = title;
        this.content = content;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var delayTime = GameManager.instance.uiManager.tooltip.tweenerDelay;
        _delay = LeanTween.delayedCall(delayTime, () =>
        {
            GameManager.instance.uiManager.tooltip.ShowToolTip(title, content);
        });
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.cancel(_delay.uniqueId);
        GameManager.instance.uiManager.tooltip.HideToolTip();
    }
}
