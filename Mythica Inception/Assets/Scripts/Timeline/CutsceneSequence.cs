using System.Collections.Generic;
using _Core.Managers;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneSequence : MonoBehaviour
{
    [SerializeField] private string _saveKey;
    public List<PlayableDirector> timelineDirectors;
    private int _currentDirectorNum = 0;
    public bool triggered = false;

    void Start()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.saveManager.LoadDataObject(_saveKey, out bool isTrig);
            triggered = isTrig;
        }
        
        if (timelineDirectors.Count <= 0 || triggered) return;
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
            triggered = true;
            if (GameManager.instance == null) return;
            
            GameManager.instance.saveManager.SaveOtherData(_saveKey, triggered);
            GameManager.instance.inputHandler.EnterGameplay();
            return;
        }

        timelineDirectors[_currentDirectorNum].gameObject.SetActive(true);
    }
}
