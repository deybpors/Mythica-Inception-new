using System;
using _Core.Managers;
using Assets.Scripts.Dialogue_System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class DialogueBehavior : PlayableBehaviour
{
    public Line line;
    public Choice[] responseChoices;

    public bool hasToPause;

    private bool _clipPlayed = false;
    private bool _pauseScheduled = false;
    private PlayableDirector _director;

    public override void OnPlayableCreate(Playable playable)
    {
        _director = (playable.GetGraph().GetResolver() as PlayableDirector);
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (_clipPlayed || !(info.weight > 0f)) return;
        
        GameManager.instance.uiManager.dialogueUI.StartDialogue(line, responseChoices);

        //if the application is playing and hasToPause is true then pauseScheduled is true, else it will stay;
        _pauseScheduled = Application.isPlaying && hasToPause || _pauseScheduled;

        _clipPlayed = true;
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (!_pauseScheduled) return;
        
        _pauseScheduled = false;
        GameManager.instance.timelineManager.PauseTimelineForDialogue(_director);
    }
}
