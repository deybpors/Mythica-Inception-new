using System.Collections;
using _Core.Managers;
using _Core.Player;
using Assets.Scripts.Timeline;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneTrigger : MonoBehaviour
{
    private enum TimeScale
    {
        Normal,
        Paused
    }

    [SerializeField] private string _saveKey;
    [SerializeField] private PlayableDirector _cutsceneToTrigger;
    [SerializeField] private CutsceneTriggerCondition[] _conditions;
    [SerializeField] private TimeScale _timeScale;
    [SerializeField] private bool _faceTrigger = true;

    private bool _triggered;
    [HideInInspector] public Transform _transform;
    [HideInInspector] public Transform playerTransform;
    [HideInInspector] public Player player;
    [HideInInspector] public GameObject _cutsceneObject;

    void Start()
    {
        _cutsceneObject = _cutsceneToTrigger.gameObject;
        _cutsceneObject.SetActive(false);
        GameManager.instance.saveManager.LoadDataObject(_saveKey, out bool isTriggered);
        _triggered = isTriggered;
        if (_triggered)
        {
            Destroy(gameObject);
        }

        _transform = transform;
        _cutsceneToTrigger.stopped += Disable;

        player = GameManager.instance.player;
        if (player == null) return;
        playerTransform = player.transform;
    }

    void Update()
    {
        if (player == null)
        {
            player = GameManager.instance.player;
            playerTransform = player.transform;
        }

        if(_triggered) return;
        if (!HandleConditions()) return;

        _triggered = true;
        if (_timeScale == TimeScale.Paused)
        {
            Time.timeScale = player.playerSettings.pauseTimeScale;
            _cutsceneToTrigger.stopped += ReturnTimeScale;
        }
        GameManager.instance.saveManager.SaveOtherData(_saveKey, _triggered);
        _cutsceneObject.SetActive(true);
        
        if(!_faceTrigger) return;
        var playerLookPosition = _transform.position - playerTransform.position;
        playerLookPosition.y = 0;
        var playerRotateTo = Quaternion.LookRotation(playerLookPosition);
        StopAllCoroutines();
        StartCoroutine(FaceToTrigger(playerRotateTo));
    }

    private IEnumerator FaceToTrigger(Quaternion playerRotateTo)
    {
        var playerRotation = playerTransform.rotation;

        var timeElapsed = 0f;

        while (timeElapsed < .25f)
        {
            playerTransform.rotation = Quaternion.Slerp(playerRotation, playerRotateTo, timeElapsed / .25f);
            timeElapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    private bool HandleConditions()
    {
        var meetsCondition = true;
        var conditionsCount = _conditions.Length;
        for (var i = 0; i < conditionsCount; i++)
        {
            meetsCondition = meetsCondition && _conditions[i].MeetConditions(this);
        }

        return meetsCondition;
    }

    private void ReturnTimeScale(PlayableDirector director)
    {
        director.stopped -= ReturnTimeScale;
        Time.timeScale = 1f;
    }

    void Disable(PlayableDirector director)
    {
        director.stopped -= Disable;
        GameManager.instance.inputHandler.EnterGameplay();
        _cutsceneObject.SetActive(false);
    }
}
