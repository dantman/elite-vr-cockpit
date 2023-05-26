using System.Collections.Generic;
using EVRC.Core.Actions;
using UnityEngine;

namespace EVRC.Core.Overlay
{
    public static class ControlButtonUtils
    {
        public static Dictionary<CockpitMode, 
            ButtonCategory> cockpitModeToButtonCategoryMap = new()
        {
            { CockpitMode.Cockpit, ButtonCategory.Cockpit },
            { CockpitMode.InShip, ButtonCategory.ShipCockpit },
            { CockpitMode.InMainShip, ButtonCategory.MainShipCockpit },
            { CockpitMode.InFighter, ButtonCategory.FighterCockpit },
            { CockpitMode.InSRV, ButtonCategory.SRVCockpit },
        };


        // @todo work with ButtonCategory and CockpitUIMode to make it so we don't need this method anymore
        public static ButtonCategory GetButtonCategoryFromCockpitMode(
            CockpitMode cockpitMode)
        {
            

            if (cockpitModeToButtonCategoryMap.TryGetValue(cockpitMode, out var mappedValue))
            {
                return mappedValue;
            }

            // Return default. This shouldn't happen, the CockpitModeAnchor class has validation to prevent this.
            Debug.LogWarning($"No Matching Button Category from CockpitModeAnchor. Returning Generic Cockpit");
            return ButtonCategory.Cockpit;
        }
    }
}