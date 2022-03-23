using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class DialogueClip : PlayableAsset, ITimelineClipAsset
{
    public DialogueBehavior behaviorTemplate = new DialogueBehavior();

    public ClipCaps clipCaps => ClipCaps.None;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        return ScriptPlayable<DialogueBehavior>.Create(graph, behaviorTemplate);
    }
}
