using System.Collections.Generic;
using _Core.Managers;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneSequence : MonoBehaviour
{
    [SerializeField] private string _saveKey;
    public List<PlayableDirector> timelineDirectors;
    private int _currentDirectorNum = 0;
    [HideInInspector] private bool _triggered = false;

    void Start()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.saveManager.LoadDataObject(_saveKey, out bool isTrig);
            _triggered = isTrig;
        }
        
        if (timelineDirectors.Count <= 0 || _triggered) return;
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
            if (gameObject == null) return;

            gameObject.SetActive(false);
            _triggered = true;
            if (GameManager.instance == null) return;
            
            GameManager.instance.saveManager.SaveOtherData(_saveKey, _triggered);
            GameManager.instance.inputHandler.EnterGameplay();
            return;
        }

        timelineDirectors[_currentDirectorNum].gameObject.SetActive(true);
    }
}
