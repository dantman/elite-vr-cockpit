using UnityEngine.Events;

namespace EVRC.Core
{
    // Extend GameEventListener to accept your new parameter
    [System.Serializable]
    public class EDStateEventListener : GameEventListener<EliteDangerousState>
    {
        public EDStateEvent EventSource;
        public EDStateUnityEvent EventResponse;

        protected override GameEvent<EliteDangerousState> Source
        {
            get { return EventSource; }
        }

        protected override UnityEvent<EliteDangerousState> Response
        {
            get { return EventResponse; }
        }
    } 

}