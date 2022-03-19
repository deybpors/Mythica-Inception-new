using System.Collections;
using System.Collections.Generic;
using _Core.Managers;
using Assets.Scripts.Dialogue_System;
using BrunoMikoski.TextJuicer;
using Quest_System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [Range(0.01f, 1f)]
    [SerializeField] private float _secondsPerCharacter = .05f;

    public Image speaker;
    public RectTransform speakerHolder;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nameText;
    public RectTransform nameHolder;
    public UITweener nextLineIconTweener;
    public UITweener choiceTweener;
    public DialogueChoiceUI[] choiceButtons;

    [SerializeField] private TMP_TextJuicer _dialogueTextJuicer;
    private Conversation _currentConversation;
    private int _lineCount = 0;

    void Update()
    {
        if (_dialogueTextJuicer.IsPlaying)
        {
            if(!nextLineIconTweener.isActiveAndEnabled) return;

            nextLineIconTweener.Disable();
            return;
        }
        nextLineIconTweener.gameObject.SetActive(true);
    }

    public void DisplayDialogue(Conversation conversation, Character character)
    {
        //if its a new conversation
        if (_currentConversation != conversation)
        {
            _lineCount = 0;
            _currentConversation = conversation;
        }

        //if it's the last line in the conversation
        if (_lineCount == conversation.lines.Length - 1)
        {
            EndConversation(conversation.choices);
        }

        //change name box if its not the same value already
        if (!nameText.text.Equals(conversation.lines[_lineCount].character.name))
        {
            nameText.text = conversation.lines[_lineCount].character.name;
        }

        //change dialogue text
        dialogueText.text = conversation.lines[_lineCount].text;

        //change speaker picture
        speaker.sprite = conversation.lines[_lineCount].character.moods[0].graphic;
        
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }

        
        _dialogueTextJuicer.Play(true);
        _lineCount++;
    }

    private void EndConversation(Choice[] choices)
    {
        var choiceLength = choices.Length;
        if(choiceLength <= 0) return;

        for (var i = 0; i < choiceButtons.Length; i++)
        {
            choiceButtons[i].gameObject.SetActive(false);
            choiceButtons[i].tooltipTrigger.enabled = false;
            try
            {
                void ChoiceButtonFunc()
                {
                    AddChoiceButtonFunction(choices[i]);
                }

                choiceButtons[i].SetTextActionOnClickButton(ChoiceButtonFunc, choices[i].text);
                choiceButtons[i].gameObject.SetActive(true);

                if (!choices[i].addAQuest || choices[i].quest == null) continue;
                    
                choiceButtons[i].tooltipTrigger.enabled = true;
                choiceButtons[i].SetTooltipTrigger(choices[i].quest.title, choices[i].quest.description);
            }
            catch
            {
                // ignored
            }
        }
        //activate choicePanel
        choiceTweener.gameObject.SetActive(true);
    }

    private void AddChoiceButtonFunction(Choice choice)
    {
        DisableTweener(choiceTweener);
        
        if (choice.addAQuest && choice.quest != null)
        {
            GameManager.instance.player.playerQuestManager.GiveQuestToPlayer(choice.quest);
        }

        if (choice.conversation != null)
        {
            DisplayDialogue(choice.conversation, choice.conversation.speaker);
        }
    }


    public void DisableTweener(UITweener tweener)
    {
        tweener.Disable();
    }
}
