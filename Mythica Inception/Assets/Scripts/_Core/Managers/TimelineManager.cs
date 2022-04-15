
using _Core.Managers;
using UnityEngine;
using UnityEngine.Playables;

namespace Assets.Scripts._Core.Managers
{
    public class TimelineManager : MonoBehaviour
    {
        private PlayableDirector _activeDirector;

        public void SwitchActiveDirector(PlayableDirector newDirector)
        {
            _activeDirector = newDirector;
        }
        
        public void PauseTimelineForDialogue(PlayableDirector director)
        {
            _activeDirector = director;
            if(director == null) return;

            _activeDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);
            GameManager.instance.gameStateController.TransitionToState(GameManager.instance.cutsceneState);
            GameManager.instance.inputHandler.SwitchActionMap("Dialogue");
        }

        public void ResumeTimelineForDialogue()
        {
            if(_activeDirector == null) return;
            _activeDirector.playableGraph.GetRootPlayable(0).SetSpeed(1d);
        }
    }
}
