using UnityEngine;

namespace EVRC.Core
{
    using CockpitMode = CockpitUIMode.CockpitMode;

    [CreateAssetMenu(menuName = Constants.GAME_EVENT_PATH + "/CockpitMode"), System.Serializable]
    public class CockpitModeEvent : GameEvent<CockpitMode>
    {
		
    }
}