using EVRC.Core.Overlay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace EVRC.Core.Overlay
{
    public static class OverlayStateUpgradeManager
    {
        private static int currentVersion = OverlayManager.currentFileVersion;

        /// <summary>
        /// Saves a backup of the current file, makes necessary conversions to old file versions, then saves the new file and returns the updated OverlayState.
        /// </summary>
        /// <param name="state"></param>
        /// <returns>Updated OverlayState</returns>
        public static OverlayState UpgradeOverlayStateFile(OverlayState state)
        {
            CreateBackupFile(state);

            if (state.version <=4)
            {
                // Move StaticLocations up by 1.2 (starting version 5)
                // Further position adjustments will likely be necessary by the user
                state = RaiseStaticLocations(state);
                state = RaiseControlButtons(state);
            }


            state.version = currentVersion;
            OverlayFileUtils.WriteToFile(state);
            return state;
        }

        private static void CreateBackupFile(OverlayState state)
        {
            // Append ".version#.backup" to the standard filename
            string fileAppendString = ".v" + state.version + ".backup";
            OverlayFileUtils.WriteToFile(state, Paths.OverlayStatePath + fileAppendString);
        }

        private static OverlayState RaiseStaticLocations(OverlayState state)
        {

            for (int i = 0; i < state.staticLocations.Length; i++)
            {
                state.staticLocations[i].overlayTransform.pos.y += 1.2f;
            }
            return state;
        }

        private static OverlayState RaiseControlButtons(OverlayState state)
        {
            var raised = new List<SavedControlButton>();

            foreach (var cb in state.controlButtons)
            {
                raised.Add(new SavedControlButton()
                {
                    type = cb.type,
                    overlayTransform = new OverlayTransform()
                    {
                        pos = cb.overlayTransform.pos + new Vector3(0,1.2f,0),
                        rot = cb.overlayTransform.rot
                    }
                });
            }

            state.controlButtons = raised.ToArray();

            return state;
        }
    }
}
