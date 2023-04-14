using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EVRC.Core
{
	// @todo Uncomment this line if you need to import a type 
    // using CockpitMode = CockpitUIMode.CockpitMode;


	// @todo Use Find & Replace for EDState to set your required parameter

    [CreateAssetMenu(menuName = Constants.GAME_EVENT_PATH + "/Elite Status Event"), System.Serializable]
    public class EDStateEvent : GameEvent<EliteDangerousState>
    {
		
    }
}