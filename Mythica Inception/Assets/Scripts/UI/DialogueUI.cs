using System;
using System.Collections;
using System.Linq;
using _Core.Managers;
using Assets.Scripts.Dialogue_System;
using BrunoMikoski.TextJuicer;
using MyBox;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public UITweener mainDialogueTweener;

    public Image speaker;
    public RectTransform speakerHolder;
    public TextMeshProUGUI dialogueText;
    public GameObject nameHolder;
    public TextMeshProUGUI nameText;
    public UITweener nextLineIconTweener;
    public UITweener choiceTweener;
    public RectTransform choiceHolder;
    public DialogueChoiceUI[] choiceButtons;

    [SerializeField] private TMP_TextJuicer _dialogueTextJuicer;
    private Conversation _currentConversation;
    private Character _currentCharacter;
    private int _lineCount = 0;

    void Update()
    {
        if (_dialogueTextJuicer.IsPlaying)
        {
            if(!nextLineIconTweener.isActiveAndEnabled) return;

            nextLineIconTweener.Disable();
            return;
        }
        CompleteTextJuicer();
        nextLineIconTweener.gameObject.SetActive(true);
    }

    public bool TextJuicerPlaying()
    {
        return _dialogueTextJuicer.IsPlaying;
    }

    public void CompleteTextJuicer()
    {
        _dialogueTextJuicer.Stop();
        _dialogueTextJuicer.SetProgress(1f);
        _dialogueTextJuicer.enabled = false;
    }

    public void StartDialogue(Conversation conversationToDisplay)
    {
        _dialogueTextJuicer.SetDirty();

        //if its a new nextConversation
        if (_currentConversation != conversationToDisplay)
        {
            _lineCount = 0;
            _currentConversation = conversationToDisplay;
        }

        //if it's the last line in the nextConversation
        if (_lineCount == conversationToDisplay.lines.Length - 1)
        {
            EndConversation(conversationToDisplay.choices);
        }

        speakerHolder.gameObject.SetActive(false);
        nameHolder.SetActive(false);
        nameText.text = string.Empty;

        var lineCharacter = conversationToDisplay.lines[_lineCount].character;
        if (lineCharacter != null)
        {
            _currentCharacter = lineCharacter;

            //change name box if its not the same value already
            if (!nameText.text.Equals(lineCharacter.name))
            {
                nameText.text = lineCharacter.name;
            }

            nameHolder.SetActive(true);
            //get the mood graphic of the character and initialize speaker picture placement
            var characterMoodGraphic = GetEmotionGraphic(lineCharacter, conversationToDisplay.lines[_lineCount].emotion);
            InitializeSpeakerPicture(characterMoodGraphic, conversationToDisplay.lines[_lineCount].speakerDirection);
            speakerHolder.gameObject.SetActive(true);
        }

        //change dialogue text
        dialogueText.text = conversationToDisplay.lines[_lineCount].text;

        _lineCount++;

        SetDialogueUiTextJuicer();
    }

    private Sprite GetEmotionGraphic(Character character, Emotion lineEmotion)
    {
        foreach (var mood in character.moods.Where(mood => lineEmotion == mood.emotion))
        {
            return mood.graphic;
        }

        return character.moods[0].graphic;
    }

    private void InitializeSpeakerPicture(Sprite graphic, SpeakerDirection direction)
    {
        //change speaker picture
        speaker.sprite = graphic;

        //switch graphic
        switch (direction)
        {
            case SpeakerDirection.Left:
                if (speakerHolder.anchoredPosition.x > 0)
                {
                    speakerHolder.SetPositionX(speakerHolder.anchoredPosition.x * -1f);
                }
                if (speakerHolder.localScale.x < 0)
                {
                    var tempScale = speakerHolder.localScale;
                    tempScale.x *= -1f;
                    speakerHolder.localScale = tempScale;
                }

                if (choiceHolder.anchoredPosition.x < 0)
                {
                    choiceHolder.SetPositionX(choiceHolder.anchoredPosition.x * -1f);
                }
                break;

            case SpeakerDirection.Right:
                if (speakerHolder.anchoredPosition.x < 0)
                {
                    speakerHolder.SetPositionX(speakerHolder.anchoredPosition.x * -1f);
                }
                if (speakerHolder.localScale.x > 0)
                {
                    var tempScale = speakerHolder.localScale;
                    tempScale.x *= -1f;
                    speakerHolder.localScale = tempScale;
                }
                if (choiceHolder.anchoredPosition.x > 0)
                {
                    choiceHolder.SetPositionX(choiceHolder.anchoredPosition.x * -1f);
                }
                break;
        }
    }

    private void SetDialogueUiTextJuicer()
    {
        gameObject.SetActive(false);
        gameObject.SetActive(true);

        _dialogueTextJuicer.enabled = true;
        _dialogueTextJuicer.SetProgress(0f);
        _dialogueTextJuicer.Play();
        StopAllCoroutines();
        StartCoroutine(PlayDialogueSound());
    }

    public void StartDialogue(Line line, Choice[] choices)
    {
        //initialize current dialogue ui
        _dialogueTextJuicer.SetDirty();
        _lineCount = 0;
        _currentConversation = null;
        _currentCharacter = line.character;

        //initialize choices
        if (choices != null)
        {
            EndConversation(choices);
        }

        //set name if it value is still not the character's name
        if (!nameText.text.Equals(line.character.name))
        {
            nameText.text = line.character.name;
        }

        //change the line of the text
        dialogueText.text = line.text;

        //get the mood graphic of the character and initialize speaker picture placement
        var characterMoodGraphic = GetEmotionGraphic(line.character, line.emotion);
        InitializeSpeakerPicture(characterMoodGraphic, line.speakerDirection);

        SetDialogueUiTextJuicer();
    }

    public bool CurrentConversationHasChoice()
    {
        if (_currentConversation != null)
        {
            return _currentConversation.choices.Length > 0;
        }

        return choiceHolder.gameObject.activeInHierarchy;
    }

    public bool IsEnd()
    {
        return _lineCount >= _currentConversation.lines.Length;
    }

    public void ContinueExistingDialogue()
    {
        StartDialogue(_currentConversation);
    }

    private void EndConversation(Choice[] choices)
    {
        var choiceLength = choices.Length;
        
        //if no responseChoices end
        if(choiceLength <= 0) return;

        //initializing all choice for choice buttons
        for (var i = 0; i < choiceButtons.Length; i++)
        {
            //set objects to false in case it is already opened
            choiceButtons[i].gameObject.SetActive(false);
            choiceButtons[i].tooltipTrigger.enabled = false;

            //initialize only when there is an instance of responseChoices[choiceNum]
            try
            {
                var choiceNum = i;
                UnityAction buttonFunc = () => AddChoiceButtonFunction(choices[choiceNum]);

                //the the action to the botton as event and set the text of the button
                choiceButtons[i].SetTextActionOnClickButton(buttonFunc, choices[i].text);

                //enable object so it can be seen
                choiceButtons[i].gameObject.SetActive(true);

                //if the choice does not entail adding a quest to the player, then continue to loop
                if (!choices[i].addAQuest || choices[i].quest == null) continue;
                    
                choiceButtons[i].tooltipTrigger.enabled = true;
                choiceButtons[i].tooltipTrigger.SetTitleContent(choices[i].quest.title, choices[i].quest.description);
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
        choiceTweener.gameObject.SetActive(false);
        
        if (choice.addAQuest && choice.quest != null)
        {
            GameManager.instance.player.playerQuestManager.GiveQuestToPlayer(choice.quest);
        }

        if (choice.nextConversation != null)
        {
            StartDialogue(choice.nextConversation);
        }
        else
        {
            mainDialogueTweener.Disable();
            GameManager.instance.player.inputHandler.EnterGameplay();
            _lineCount = 0;
            _currentConversation = null;
        }

        GameManager.instance.uiManager.tooltip.tooltipTweener.Disable();
    }

    public IEnumerator PlayDialogueSound()
    {
        var text = dialogueText.text;
        var index = 0;
        while (_dialogueTextJuicer.IsPlaying)
        {
            if(!Char.IsPunctuation(text[index]))
                GameManager.instance.audioManager.PlaySFX(text[index].ToString(), _currentCharacter.dialoguePitch); 
            yield return new WaitForSeconds(_dialogueTextJuicer.Delay * 2.5f);
            index++;
        }
    }

    private void DisableTweener(UITweener tweener)
    {
        tweener.Disable();
    }
}
