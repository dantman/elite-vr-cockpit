using EVRC.Core.Actions;
using EVRC.Core.Overlay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EVRC.Core
{
    [CreateAssetMenu(menuName = Constants.GAME_EVENT_PATH + "/ControlButton Added"), System.Serializable]
    public class ControlButtonAddedEvent : GameEvent<SavedControlButton>
    {
		
    }
}