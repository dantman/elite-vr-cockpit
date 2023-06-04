using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Valve.Newtonsoft.Json;
using static EVRC.Core.Overlay.LegacyOverlayStates;
using static EVRC.Core.Overlay.LegacyOverlayStates.Version4Struct;

namespace EVRC.Core.Overlay
{
    public class OverlayStateUpgradeManager
    {
        private static int currentVersion = OverlayManager.currentFileVersion;

        /// <summary>
        /// Makes necessary conversions to old file versions, then saves the new file and returns the updated SavedState.
        /// </summary>
        /// <param name="path">path to file</param>
        /// <param name="version">version number</param>
        /// <returns></returns>
        public SavedStateFile UpgradeOverlayStateFile(string path, int version)
        {
            Debug.Log("Overlay File upgrade started.");
            CreateBackupFile(path, version);

            var upgradedState = new SavedStateFile();

            if (version <=4)
            {
                Version4Struct oldState = ReadVersion4StateFile(path);

                SavedGameObject[] upgradedStaticLocations = AddKeyToStaticLocations(oldState);
                upgradedStaticLocations = RaiseStaticLocations(upgradedStaticLocations);
                upgradedState.staticLocations = upgradedStaticLocations;

                Version4Struct.SavedControlButton[] raisedButtons = RaiseControlButtons(oldState.controlButtons);
                SavedControlButton[] anchoredButtons = AddAnchorsToControlButtons(raisedButtons);
                upgradedState.controlButtons = anchoredButtons;

                upgradedState.booleanSettings = oldState.booleanSettings;
            }


            Debug.Log("---Save File Upgraded---");
            upgradedState.version = currentVersion;
            OverlayFileUtils.WriteToFile(upgradedState);
            return upgradedState;
        }

        private SavedControlButton[] AddAnchorsToControlButtons(Version4Struct.SavedControlButton[] buttons)
        {
            List<SavedControlButton> anchoredButtons = new List<SavedControlButton>();
            foreach(var button in buttons)
            {
                var newButton = 
                    new SavedControlButton()
                    {
                        type = button.type,
                        anchorGuiFocus = "",
                        anchorStatusFlag = "InMainShip", // default everything into the mainship (upon upgrade)
                        overlayTransform =
                        {
                            pos = button.loc.pos,
                            rot = button.loc.rot,
                        }
                    };

                // Check for SRV controlButtons
                if (button.type.IndexOf("Buggy", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    newButton.anchorStatusFlag = "InSRV";
                } else if (button.type.IndexOf("ToggleDriveAssist", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    newButton.anchorStatusFlag = "InSRV";
                }

                // Check for FSS Mode buttons
                if (button.type.IndexOf("ExplorationFSS", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (button.type != "ExplorationFSSEnter") // the one exception to this rule...this needs to be visible in the Mainship 
                    {
                        newButton.anchorGuiFocus = "FSSMode";
                    }
                    
                }

                anchoredButtons.Add(newButton);
            }
            
            return anchoredButtons.ToArray();
        }

        public Version4Struct ReadVersion4StateFile(string path)
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<Version4Struct>(File.ReadAllText(path));
        }


        /// <summary>
        /// In version 4, staticLocations did not contain a "key" value. They were stored in a dict. From
        /// version 5 forward, they were switched to an array of values, each containing a value called "key"
        /// </summary>
        private SavedGameObject[] AddKeyToStaticLocations(Version4Struct data)
        {
            Debug.Log("Reformatting StaticLocations (controls) save format");
            var returnObjects = new List<SavedGameObject>();

            returnObjects.Add(
                new SavedGameObject()
                {
                    key = "srvThrottle",
                    overlayTransform = new OverlayTransform()
                    {
                        pos = data.staticLocations.srvThrottle.pos,
                        rot = data.staticLocations.srvThrottle.rot,
                    }
                }
            );

            returnObjects.Add(
                new SavedGameObject()
                {
                    key = "shipThrottle",
                    overlayTransform = new OverlayTransform()
                    {
                        pos = data.staticLocations.shipThrottle.pos,
                        rot = data.staticLocations.shipThrottle.rot,
                    }
                }
            );
            returnObjects.Add(
                new SavedGameObject()
                {
                    key = "srvJoystick",
                    overlayTransform = new OverlayTransform()
                    {
                        pos = data.staticLocations.srvJoystick.pos,
                        rot = data.staticLocations.srvJoystick.rot,
                    }
                }
            );
            returnObjects.Add(
                new SavedGameObject()
                {
                    key = "shipJoystick",
                    overlayTransform = new OverlayTransform()
                    {
                        pos = data.staticLocations.shipJoystick.pos,
                        rot = data.staticLocations.shipJoystick.rot,
                    }
                }
            );
            returnObjects.Add(
                new SavedGameObject()
                {
                    key = "metaPanel",
                    overlayTransform = new OverlayTransform()
                    {
                        pos = data.staticLocations.metaPanel.pos,
                        rot = data.staticLocations.metaPanel.rot,
                    }
                }
            );
            returnObjects.Add(
                new SavedGameObject()
                {
                    key = "shipPowerDeliveryPanel",
                    overlayTransform = new OverlayTransform()
                    {
                        pos = data.staticLocations.shipPowerDeliveryPanel.pos,
                        rot = data.staticLocations.shipPowerDeliveryPanel.rot,
                    }
                }
            );
            returnObjects.Add(
                new SavedGameObject()
                {
                    key = "srvPowerDeliveryPanel",
                    overlayTransform = new OverlayTransform()
                    {
                        pos = data.staticLocations.srvPowerDeliveryPanel.pos,
                        rot = data.staticLocations.srvPowerDeliveryPanel.rot,
                    }
                }
            );
            returnObjects.Add(
                new SavedGameObject()
                {
                    key = "mapPlaneController",
                    overlayTransform = new OverlayTransform()
                    {
                        pos = data.staticLocations.mapPlaneController.pos,
                        rot = data.staticLocations.mapPlaneController.rot,
                    }
                }
            );
            returnObjects.Add(
                new SavedGameObject()
                {
                    key = "sixDofController",
                    overlayTransform = new OverlayTransform()
                    {
                        pos = data.staticLocations.sixDofController.pos,
                        rot = data.staticLocations.sixDofController.rot,
                    }
                }
            );

            return returnObjects.ToArray();
        }

        private static void CreateBackupFile(string filePath, int version)
        {
            //Create a file path like originalName.json.v4.backup
            string backupPath = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + ".v" + version +".backup" + Path.GetExtension(filePath));
            Debug.Log($"Creating backup of previous overlay file at: {backupPath}");
            File.Copy(filePath, backupPath, true);
        }

        private static SavedGameObject[] RaiseStaticLocations(SavedGameObject[] staticLocations)
        {
            Debug.Log("Raising StaticLocations to new origin");
            for (int i = 0; i < staticLocations.Length; i++)
            {
                staticLocations[i].overlayTransform.pos.y += 1.2f;
            }
            return staticLocations;
        }

        private static Version4Struct.SavedControlButton[] RaiseControlButtons(Version4Struct.SavedControlButton[] controlButtons)
        {
            Debug.Log("Raising ControlButtons to new origin");

            List<Version4Struct.SavedControlButton> returnList = new List<Version4Struct.SavedControlButton>();

            for (int i = 0; i < controlButtons.Length; i++)
            {
                // raise by 1.2
                controlButtons[i].loc.pos.y += 1.2f;

                // restructure and add to return list
                returnList.Add(new Version4Struct.SavedControlButton()
                {
                    type = controlButtons[i].type,
                    loc = new SavedTransform()
                    {
                        pos = controlButtons[i].loc.pos, 
                        rot = controlButtons[i].loc.rot
                    }
                });
                
            }
            return returnList.ToArray();
        }
    }
}
