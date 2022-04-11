
using System;
using _Core.Managers;
using MyBox;
using SoundSystem;
using UnityEngine;

public class PlayWorldMusic : MonoBehaviour
{
    [Foldout("Background Music", true)]
    [SerializeField] private MusicTypePlayed _typeToPlay;

    [ConditionalField(nameof(_typeToPlay), false, MusicTypePlayed.Name)]
    [SerializeField] private string _musicName;

    [ConditionalField(nameof(_typeToPlay), false, MusicTypePlayed.Mood)]
    [SerializeField] private MusicMood _musicMood = MusicMood.Calm;

    [ConditionalField(nameof(_typeToPlay), false, MusicTypePlayed.Situation)]
    [SerializeField] private MusicSituation _musicSituation = MusicSituation.Wild;

    [Foldout("Ambience", true)]
    [SerializeField] private string[] _ambienceNames;

    void OnEnable()
    {
        var audioManager = GameManager.instance.audioManager;
        switch (_typeToPlay)
        {
            case MusicTypePlayed.Name:
                audioManager.PlayMusic(_musicName);
                break;
            case MusicTypePlayed.Mood:
                audioManager.PlayMusic(_musicMood);
                break;
            case MusicTypePlayed.Situation:
                audioManager.PlayMusic(_musicSituation);
                break;
        }

        var ambienceLength = _ambienceNames.Length;
        for (var i = 0; i < ambienceLength; i++)
        {
            audioManager.PlayAmbience(_ambienceNames[i]);
        }
    }
}
