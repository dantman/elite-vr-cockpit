using UnityEngine;


namespace EVRC
{
    using CockpitMode = CockpitUIMode.CockpitMode;

    [CreateAssetMenu(menuName = Constants.GAME_EVENT_PATH + "/CockpitMode"), System.Serializable]
    public class CockpitModeEvent : GameEvent<CockpitMode>
    {
		
    }
}