using UnityEngine.Events;

namespace EVRC
{
    using CockpitMode = CockpitUIMode.CockpitMode;

    // Extend GameEventListener to accept your new parameter
    [System.Serializable]
    public class CockpitModeEventListener : GameEventListener<CockpitMode>
    {
        public CockpitModeEvent EventSource;
        public CockpitModeUnityEvent EventResponse;

        protected override GameEvent<CockpitMode> Source
        {
            get { return EventSource; }
        }

        protected override UnityEvent<CockpitMode> Response
        {
            get { return EventResponse; }
        }
    } 

}