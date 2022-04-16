using System;
using System.Collections.Generic;
using System.Diagnostics;
using _Core.Others;
using Assets.Scripts.Dialogue_System;
using UnityEngine;
using Debug = UnityEngine.Debug;

[CreateAssetMenu(menuName = "Dialogue System/Character")]
public class Character : ScriptableObjectWithID
{
    public string fullName;
    public Sprite facePicture;
    public List<CharacterMood> moods;

    [SerializeField, HideInInspector] private bool _hasBeenInitialized = false;
    public Sex sexOfCharacter;
    public float dialoguePitch = 1f;

    [Conditional("UNITY_EDITOR")]
    private void OnValidate()
    {
        if(_hasBeenInitialized) return;

        var emotionMemberCount = Enum.GetNames(typeof(Emotion)).Length;

        for (var i = 0; i < emotionMemberCount; i++)
        {
            var moodToAdd = new CharacterMood(Enum.GetName(typeof(Emotion), i), (Emotion)i, null);
            moods.Add(moodToAdd);
        }

        _hasBeenInitialized = true;
    }
}

[Serializable] 
public class CharacterMood
{
    public string name;
    public Emotion emotion;
    public Sprite graphic;

    public CharacterMood(string name, Emotion emotion, Sprite graphic)
    {
        this.name = name;
        this.emotion = emotion;
        this.graphic = graphic;
    }
}