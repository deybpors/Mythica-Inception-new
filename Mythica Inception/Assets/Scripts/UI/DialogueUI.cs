using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Core.Managers;
using _Core.Others;
using Assets.Scripts.Dialogue_System;
using BrunoMikoski.TextJuicer;
using MyBox;
using Quest_System;
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
    [SerializeField] private Character _maleCharacter;
    [SerializeField] private Character _femaleCharacter;

    private PlayerAcceptedQuest _newQuestGiven;
    private Conversation _currentConversation;
    private Character _currentCharacter;
    private int _lineCount = 0;
    private GameObject _choicesGameObject;
    private Dictionary<DialogueChoiceUI, GameObject> _dialogueChoices = new Dictionary<DialogueChoiceUI, GameObject>();

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
        cutscene = false;
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

        _currentCharacter = conversationToDisplay.lines[_lineCount].character;

        if (_currentCharacter == null)
        {
            _currentCharacter = GameManager.instance.loadedSaveData.sex == Sex.Male ? _maleCharacter : _femaleCharacter;
        }

        //change name box if its not the same value already
        if (!nameText.text.Equals(_currentCharacter.fullName))
        {
            nameText.text = _currentCharacter.fullName;
        }

        nameHolder.SetActive(true);
        //get the mood graphic of the character and initialize speaker picture placement
        var characterMoodGraphic = GetEmotionGraphic(_currentCharacter, conversationToDisplay.lines[_lineCount].emotion);
        InitializeSpeakerPicture(characterMoodGraphic, conversationToDisplay.lines[_lineCount].speakerDirection);

        speakerHolder.gameObject.SetActive(true);

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

        if (_currentCharacter == null)
        {
            _currentCharacter = GameManager.instance.loadedSaveData.sex == Sex.Male ? _maleCharacter : _femaleCharacter;
        }

        if (_currentCharacter != null && displayCharPic)
        {
            //change name box if its not the same value already
            if (!nameText.text.Equals(_currentCharacter.fullName))
            {
                nameText.text = _currentCharacter.fullName;
            }

            nameHolder.SetActive(true);
            //get the mood graphic of the character and initialize speaker picture placement
            var characterMoodGraphic = GetEmotionGraphic(_currentCharacter, line.emotion);
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
        if (choiceLength <= 0)
        {
            if (_newQuestGiven == null) return;
            
            GameManager.instance.uiManager.questUI.OpenPanelFromIcon(_newQuestGiven, _newQuestGiven.questGiver.facePicture);
            _newQuestGiven = null;
            return;
        }

        //initializing all choice for choice buttons
        for (var i = 0; i < choiceButtons.Length; i++)
        {
            if (!_dialogueChoices.TryGetValue(choiceButtons[i], out var choiceButtonGameObject))
            {
                choiceButtonGameObject = choiceButtons[i].gameObject;
                _dialogueChoices.Add(choiceButtons[i], choiceButtonGameObject);
            }
            //set objects to false in case it is already opened
            choiceButtonGameObject.SetActive(false);
            choiceButtons[i].tooltipTrigger.enabled = false;

            //initialize only when there is an instance of responseChoices[choiceNum]
            try
            {
                var choiceNum = i;
                UnityAction buttonFunc = () => AddChoiceButtonFunction(choices[choiceNum]);

                //the the action to the botton as event and set the text of the button
                choiceButtons[i].SetTextActionOnClickButton(buttonFunc, choices[i].text);

                //enable object so it can be seen
                choiceButtonGameObject.SetActive(true);

                //if the choice does not entail adding a quest to the player, then set the title and content of the tooltip to the desired title and content on the choice
                choiceButtons[i].tooltipTrigger.enabled = true;
                if (choices[i].quest != null)
                {
                    choiceButtons[i].tooltipTrigger.SetTitleContent(choices[i].quest.title, choices[i].quest.description);
                }
                else
                {
                    choiceButtons[i].tooltipTrigger.SetTitleContent(choices[i].tooltipTitle, choices[i].tooltipDescription);
                }
            }
            catch
            {
                // ignored
            }
        }

        //activate choicePanel
        if (_choicesGameObject == null)
        {
            _choicesGameObject = choiceTweener.gameObject;
        }
        _choicesGameObject.SetActive(true);
    }

    private void AddChoiceButtonFunction(Choice choice)
    {
        if (_choicesGameObject == null)
        {
            _choicesGameObject = choiceTweener.gameObject;
        }
        _choicesGameObject.SetActive(false);

        _newQuestGiven = GameManager.instance.player.playerQuestManager.GiveQuestToPlayer(choice.quest, _currentCharacter);

        choice.onClickChoice?.Invoke();

        if (choice.nextConversation != null)
        {
            StartDialogue(choice.nextConversation);
        }
        else
        {
            mainDialogueTweener.Disable();
            
            _lineCount = 0;
            _currentConversation = null;
            if (!cutscene)
            {
                GameManager.instance.inputHandler.EnterGameplay();
                if (_newQuestGiven != null)
                {
                    GameManager.instance.uiManager.questUI.OpenPanelFromIcon(_newQuestGiven, _newQuestGiven.questGiver.facePicture);
                    _newQuestGiven = null;
                }
            }
        }

        GameManager.instance.uiManager.tooltip.tooltipTweener.Disable();
    }

    public IEnumerator PlayDialogueSound()
    {
        var text = dialogueText.text;
        var index = 0;
        while (_dialogueTextJuicer.IsPlaying)
        {
            try
            {
                if (!Char.IsPunctuation(text[index]) && !Char.IsWhiteSpace(text[index]) && _currentCharacter != null)
                {
                    var dialogueSFXName = _currentCharacter.sexOfCharacter == Sex.Male
                        ? text[index] + "_MALE".ToUpperInvariant()
                        : text[index] + "_FEMALE".ToUpperInvariant();
                    GameManager.instance.audioManager.PlaySFX(dialogueSFXName, _currentCharacter.dialoguePitch);
                }
            }
            catch
            {
                //ignored
            }

            yield return new WaitForSecondsRealtime(_dialogueTextJuicer.Delay * 2.5f);
            index++;
        }
    }

    private void DisableTweener(UITweener tweener)
    {
        tweener.Disable();
    }
}
