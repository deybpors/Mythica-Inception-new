using _Core.Others;
using MyBox;
using Quest_System;
using UnityEngine;

namespace Assets.Scripts.Dialogue_System
{
    [CreateAssetMenu(menuName = "Dialogue System/Conversation")]
    public class Conversation : ScriptableObjectWithID
    {
        public Line[] lines;
        [Tooltip("Displays with the last line of the nextConversation. Note: Please limit responseChoices to at most 5.")]
        public Choice[] choices;
    }

    [System.Serializable]
    public struct Line
    {
        public Character character;
        [TextArea(2, 5)]
        public string text;
        public Emotion emotion;
        public SpeakerDirection speakerDirection;
    }

    [System.Serializable]
    public struct Choice
    {
        public string text;
        public Conversation nextConversation;
        public bool addAQuest;
        [ConditionalField(nameof(addAQuest))] public Quest quest;
    }

    public enum SpeakerDirection
    {
        Left,
        Right
    }

    public enum Emotion
    {
        Normal,
        Happy,
        Sad,
        Angry,
        Surprised,
        Skeptical,
        Scared,
        Aggrivated
    }
}
