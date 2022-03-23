
using _Core.Managers;
using UnityEngine;
using UnityEngine.Playables;

namespace Assets.Scripts._Core.Managers
{
    public class TimelineManager : MonoBehaviour
    {
        public void PauseTimelineForDialogue(PlayableDirector director)
        {
            director.playableGraph.GetRootPlayable(0).SetSpeed(0d);
            GameManager.instance.gameStateController.TransitionToState(GameManager.instance.dialogueState);
            GameManager.instance.player.inputHandler.GetPlayerInputSettings().SwitchCurrentActionMap("Dialogue");
        }
    }
}
