using System;
using _Core.Managers;
using Assets.Scripts.Dialogue_System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class DialogueBehavior : PlayableBehaviour
{
    public bool hasToPause;
    public bool displayCharPic = false;
    public Line line;
    public Choice[] responseChoices;


    private bool _clipPlayed = false;
    private bool _pauseScheduled = false;
    private PlayableDirector _director;

    public override void OnPlayableCreate(Playable playable)
    {
        _director = (playable.GetGraph().GetResolver() as PlayableDirector);
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if(GameManager.instance == null) return;
        if (_clipPlayed || !(info.weight > 0f))
        {
            _pauseScheduled = Application.isPlaying && hasToPause && !GameManager.instance.uiManager.dialogueUI.mainDialogueTweener.disabled;
            return;
        }
        
        InitiateDialogueUI();
        InitiateInputHandler();
        GameManager.instance.uiManager.dialogueUI.StartDialogue(line, responseChoices, displayCharPic);
        if (Application.isPlaying && hasToPause)
        {
            _pauseScheduled = true;
        }
        else
        {
            _pauseScheduled = false;
        }
        _clipPlayed = true;
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (_pauseScheduled)
        {
            _pauseScheduled = false;
            GameManager.instance.timelineManager.PauseTimelineForDialogue(_director);
        }
        _clipPlayed = false;

        //if application ends return
        if (!Application.isPlaying) return;


        //Check if its the end of clip
        var duration = playable.GetDuration();
        var time = playable.GetTime();
        var count = time + info.deltaTime;

        if ((info.effectivePlayState != PlayState.Paused || !(count > duration)) &&
            !Mathf.Approximately((float) time, (float) duration)) return;
        
        if(!hasToPause) GameManager.instance.uiManager.dialogueUI.OnDialogueEnd();
    }

    private void InitiateDialogueUI()
    {
        if(GameManager.instance == null) return;
        GameManager.instance.timelineManager.SwitchActiveDirector(_director);
        GameManager.instance.uiManager.dialogueUI.cutscene = true;
        var dialogueUiGameObject = GameManager.instance.uiManager.dialogueUI.mainDialogueTweener.gameObject;
        if (!dialogueUiGameObject.activeInHierarchy)
            dialogueUiGameObject.SetActive(true);
    }

    private static void InitiateInputHandler()
    {
        if(GameManager.instance == null) return;
        if (!GameManager.instance.inputHandler.activate)
        {
            GameManager.instance.inputHandler.ActivatePlayerInputHandler(null, Camera.current);
        }

        GameManager.instance.gameStateController.TransitionToState(GameManager.instance.cutsceneState);
        GameManager.instance.inputHandler.SwitchActionMap("Dialogue");
    }
}
