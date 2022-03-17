using _Core.Managers;
using Assets.Scripts.Dialogue_System;
using BrunoMikoski.TextJuicer;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public Image speaker;
    public RectTransform speakerHolder;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nameText;
    public RectTransform nameHolder;
    public UITweener nextLineIconTweener;
    public UITweener choiceTweener;
    public DialogueChoiceUI[] choiceButtons;

    [SerializeField] private TMP_TextJuicer _dialogueTextJuicer;
    private int _lineCount = 0;

    public void DisplayDialogue(Conversation conversation, Character character)
    {
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
            DisplayDialogue(conversation, character);
        }

        //change name box
        if (!nameText.text.Equals(conversation.lines[_lineCount].character.name))
        {
            nameText.text = conversation.lines[_lineCount].character.name;
        }

        //change dialogue text
        dialogueText.text = conversation.lines[_lineCount].text;
        
        //if it's the last line in the conversation
        if (_lineCount == conversation.lines.Length - 1)
        {
            DisplayChoiceButtons(conversation.choices);
        }

        _dialogueTextJuicer.Play(true);
        _lineCount++;
    }

    private void DisplayChoiceButtons(Choice[] choices)
    {
        var choiceLength = choices.Length;
        if(choiceLength == 0) return;

        for (var i = 0; i < choiceButtons.Length; i++)
        {
            choiceButtons[i].gameObject.SetActive(false);
            try
            {
                UnityAction nextConversation = delegate { NextConversation(choices[i]); };
                choiceButtons[i].SetTextActionOnClickButton(nextConversation, choices[i].text);
                choiceButtons[i].gameObject.SetActive(true);
            }
            catch
            {
                // ignored
            }
        }
        //activate choicePanel
        choiceTweener.gameObject.SetActive(true);
    }

    private void NextConversation(Choice choice)
    {
        DisableTweener(choiceTweener);

        if (choice.addAQuest)
        {
            for (var i = 0; i < choice.quests.Value.Length; i++)
            {
                var quest = choice.quests.Value[i];
                if (quest == null) continue;

                GameManager.instance.player.playerQuestManager.GiveQuestToPlayer(quest);
            }
        }
    }


    public void DisableTweener(UITweener tweener)
    {
        tweener.Disable();
    }
}
