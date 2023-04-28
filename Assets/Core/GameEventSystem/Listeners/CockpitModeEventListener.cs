using UnityEngine.Events;

namespace EVRC.Core
{
    using CockpitMode = CockpitUIMode.CockpitMode;

    // Extend GameEventListener to accept your new parameter
    [System.Serializable]
    public class CockpitModeEventListener : GameEventListener<CockpitMode>
    {

    } 

}