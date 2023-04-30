using System.Collections.Generic;
using EVRC.Core.Actions;
using UnityEngine;

namespace EVRC.Core.Overlay
{
    public static class ControlButtonUtils
    {
        public static Dictionary<CockpitUIMode.CockpitMode, 
            ControlButtonAsset.ButtonCategory> cockpitModeToButtonCategoryMap = new()
        {
            { CockpitUIMode.CockpitMode.Cockpit, ControlButtonAsset.ButtonCategory.Cockpit },
            { CockpitUIMode.CockpitMode.InShip, ControlButtonAsset.ButtonCategory.ShipCockpit },
            { CockpitUIMode.CockpitMode.InMainShip, ControlButtonAsset.ButtonCategory.MainShipCockpit },
            { CockpitUIMode.CockpitMode.InFighter, ControlButtonAsset.ButtonCategory.FighterCockpit },
            { CockpitUIMode.CockpitMode.InSRV, ControlButtonAsset.ButtonCategory.SRVCockpit },
        };


        // @todo work with ButtonCategory and CockpitUIMode to make it so we don't need this method anymore
        public static ControlButtonAsset.ButtonCategory GetButtonCategoryFromCockpitMode(
            CockpitUIMode.CockpitMode cockpitMode)
        {
            

            if (cockpitModeToButtonCategoryMap.TryGetValue(cockpitMode, out var mappedValue))
            {
                return mappedValue;
            }

            // Return default. This shouldn't happen, the CockpitModeAnchor class has validation to prevent this.
            Debug.LogWarning($"No Matching Button Category from CockpitModeAnchor. Returning Generic Cockpit");
            return ControlButtonAsset.ButtonCategory.Cockpit;
        }
    }
}