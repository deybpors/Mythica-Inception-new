using System.Collections.Generic;
using _Core.Managers;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneSequence : MonoBehaviour
{
    public List<PlayableDirector> timelineDirectors;
    private int _currentDirectorNum = 0;

    void Start()
    {
        if(timelineDirectors.Count <= 0) return;

        foreach (var director in timelineDirectors)
        {
            director.stopped += DirectorStopped;
            director.gameObject.SetActive(false);
        }
        timelineDirectors[_currentDirectorNum].gameObject.SetActive(true);
    }

    void DirectorStopped(PlayableDirector director)
    {
        director.stopped -= DirectorStopped;
        director.gameObject.SetActive(false);
        _currentDirectorNum++;

        if (_currentDirectorNum >= timelineDirectors.Count)
        {
            gameObject.SetActive(false);
            return;
        }

        timelineDirectors[_currentDirectorNum].gameObject.SetActive(true);
    }
}
