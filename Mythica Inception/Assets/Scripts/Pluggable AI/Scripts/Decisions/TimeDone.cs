using Pluggable_AI.Scripts.Decisions;
using Pluggable_AI.Scripts.General;
using UnityEngine;

[CreateAssetMenu(menuName = "Pluggable AI/Decisions/Is Time Done Decision")]
public class TimeDone : Decision
{
    [SerializeField] private float _waitTimeMin;
    [SerializeField] private float _waitTimeMax = 5f;

    public override bool Decide(StateController stateController)
    {
        var timeToWait = Random.Range(_waitTimeMin, _waitTimeMax);
        return stateController.HasTimeElapsed(timeToWait);
    }
}
