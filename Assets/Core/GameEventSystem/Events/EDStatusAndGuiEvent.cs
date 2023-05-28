using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EVRC.Core
{
    [CreateAssetMenu(menuName = Constants.GAME_EVENT_PATH + "/EDStatus Flags and Gui Focus"), System.Serializable]
    public class EDStatusAndGuiEvent : GameEvent<EDStatusFlags, EDGuiFocus>
    {
		
    }
}