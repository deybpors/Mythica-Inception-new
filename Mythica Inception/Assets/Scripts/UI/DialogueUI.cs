using System.Collections;
using System.Collections.Generic;
using MyBox;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public Image speaker;
    public RectTransform speakerHolder;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nameText;
    public RectTransform nameHolder;
    public UITweener nextLineTweener;
    public UITweener choiceTweener;
    public Button[] choiceButtons;

    public void DisableTweener(UITweener tweener)
    {
        tweener.Disable();
    }
}
