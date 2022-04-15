using System;
using System.Collections;
using System.Linq;
using _Core.Managers;
using _Core.Others;
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
    [ReadOnly] public bool cutscene = false;

    [SerializeField] private string _playerNameTag = "<PLAYER_NAME>";
    [SerializeField] private string _firstPartyMythicaTag = "<MYTHICA_NICKNAME>";
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

        //if it's the last lineToDisplay in the nextConversation
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
        ManageDialogueString(conversationToDisplay.lines[_lineCount].text);

        _lineCount++;

        SetDialogueUiTextJuicer();
    }

    private void ManageDialogueString(string lineToDisplay)
    {
        //change player tag
        if (GameManager.instance.loadedSaveData != null)
        {
            lineToDisplay = lineToDisplay.Replace(_playerNameTag, GameManager.instance.loadedSaveData.name);
        }

        //change mythica tag to first party mythica
        if (GameManager.instance.player != null)
        {
            try
            {
                var nickName = GameManager.instance.player.monsterSlots[0].name != string.Empty
                    ? GameManager.instance.player.monsterSlots[0].name
                    : GameManager.instance.player.monsterSlots[0].monster.monsterName;
                lineToDisplay = lineToDisplay.Replace(_firstPartyMythicaTag, nickName);
            }
            catch
            {
                //ignore
            }
        }


        dialogueText.text = lineToDisplay;
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

    public void StartDialogue(Line line, Choice[] choices, bool displayCharPic)
    {
        //initialize current dialogue ui
        _dialogueTextJuicer.SetDirty();
        _lineCount = 0;
        _currentConversation = null;
        _currentCharacter = line.character;
        speakerHolder.gameObject.SetActive(false);
        nameHolder.SetActive(false);

        //initialize choices
        if (choices != null && choices.Length > 0)
        {
            EndConversation(choices);
        }

        var lineCharacter = line.character;
        if (lineCharacter != null && displayCharPic)
        {
            //change name box if its not the same value already
            if (!nameText.text.Equals(lineCharacter.name))
            {
                nameText.text = lineCharacter.name;
            }

            nameHolder.SetActive(true);
            //get the mood graphic of the character and initialize speaker picture placement
            var characterMoodGraphic = GetEmotionGraphic(lineCharacter, line.emotion);
            InitializeSpeakerPicture(characterMoodGraphic, line.speakerDirection);
            speakerHolder.gameObject.SetActive(true);
        }

        //change the lineToDisplay of the text
        ManageDialogueString(line.text);

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
        try
        {
            return _lineCount >= _currentConversation.lines.Length;
        }
        catch
        {
            return true;
        }
    }

    public void OnDialogueEnd()
    {
        mainDialogueTweener.Disable();
        GameManager.instance.timelineManager.ResumeTimelineForDialogue();
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
            GameManager.instance.player.playerQuestManager.GiveQuestToPlayer(choice.quest, _currentCharacter);
        }

        if (choice.nextConversation != null)
        {
            StartDialogue(choice.nextConversation);
        }
        else
        {
            mainDialogueTweener.Disable();
            if (!cutscene)
            {
                GameManager.instance.inputHandler.EnterGameplay();
            }
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
            if (!Char.IsPunctuation(text[index]) && !Char.IsWhiteSpace(text[index]) && _currentCharacter != null)
            {
                var dialogueSFXName = _currentCharacter.sexOfCharacter == Sex.Male
                    ? text[index] + "_MALE".ToUpperInvariant()
                    : text[index] + "_FEMALE".ToUpperInvariant();
                GameManager.instance.audioManager.PlaySFX(dialogueSFXName, _currentCharacter.dialoguePitch);
            }
            yield return new WaitForSeconds(_dialogueTextJuicer.Delay * 2.5f);
            index++;
        }
    }

    private void DisableTweener(UITweener tweener)
    {
        tweener.Disable();
    }
}
