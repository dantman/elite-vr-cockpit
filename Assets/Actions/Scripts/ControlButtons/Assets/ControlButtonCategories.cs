using UnityEngine;

namespace EVRC.Core.Actions
{

    public enum ButtonCategory
    {
        [Tooltip("Any type of cockpit (Ships, Fighters, SRVs)")]
        Cockpit,
        [Tooltip("Only ship cockpits (Main ship and fighters) not SRVs")]
        ShipCockpit,
        [Tooltip("Only the main ship cockpit, not fighters or SRVs")]
        MainShipCockpit,
        [Tooltip("Only fighter cockpits, not the main ship or SRVs")]
        FighterCockpit,
        [Tooltip("Only SRV cockpits, not the main ship or fighters")]
        SRVCockpit,
    }
    
}