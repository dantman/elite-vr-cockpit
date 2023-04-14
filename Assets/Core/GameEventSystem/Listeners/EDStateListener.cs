using UnityEngine.Events;

namespace EVRC.Core
{

	// @todo find and replace EDState in this file
	// @todo add a new Extension to UnityEvent<T> with the same EDState (look in Generics folder for UnityEventsExtended)
	

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