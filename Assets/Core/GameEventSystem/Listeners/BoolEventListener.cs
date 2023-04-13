using UnityEngine.Events;

namespace EVRC.Core
{

    // Extend GameEventListener to accept your new parameter
    [System.Serializable]
    public class BoolEventListener : GameEventListener<bool>
    {
        public BoolEvent EventSource;
        public BoolUnityEvent EventResponse;

        protected override GameEvent<bool> Source
        {
            get { return EventSource; }
        }

        protected override UnityEvent<bool> Response
        {
            get { return EventResponse; }
        }
    } 

}