using _Core.Managers;
using Pluggable_AI.Scripts.Decisions;
using Pluggable_AI.Scripts.General;
using UnityEngine;

[CreateAssetMenu(menuName = "Pluggable AI/Decisions/Player is Buying")]
public class IsBuying : Decision
{
    public override bool Decide(StateController stateController)
    {
        return GameManager.instance.uiManager.merchantUi.buying;
    }
}
