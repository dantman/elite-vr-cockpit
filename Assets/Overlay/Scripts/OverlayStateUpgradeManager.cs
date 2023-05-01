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
        /// Makes necessary conversions to old file versions, then saves the new file and returns the updated OverlayState.
        /// </summary>
        /// <param name="path">path to file</param>
        /// <param name="version">version number</param>
        /// <returns></returns>
        public OverlayState UpgradeOverlayStateFile(string path, int version)
        {
            Debug.Log("Overlay File upgrade started.");
            CreateBackupFile(path, version);

            var upgradedState = new OverlayState();

            if (version <=4)
            {
                Version4Struct oldState = ReadVersion4StateFile(path);

                SavedGameObject[] upgradedStaticLocations = AddKeyToStaticLocations(oldState);
                upgradedStaticLocations = RaiseStaticLocations(upgradedStaticLocations);
                upgradedState.staticLocations = upgradedStaticLocations;


                SavedControlButton[] upgradedButtons = RaiseControlButtons(oldState.controlButtons);
                upgradedState.controlButtons = upgradedButtons;

                upgradedState.booleanSettings = oldState.booleanSettings;
            }


            Debug.Log("---Save File Upgraded---");
            upgradedState.version = currentVersion;
            OverlayFileUtils.WriteToFile(upgradedState);
            return upgradedState;
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

        private static SavedControlButton[] RaiseControlButtons(Version4Struct.SavedControlButton[] controlButtons)
        {
            Debug.Log("Raising ControlButtons to new origin");

            List<SavedControlButton> returnList = new List<SavedControlButton>();

            for (int i = 0; i < controlButtons.Length; i++)
            {
                // raise by 1.2
                controlButtons[i].loc.pos.y += 1.2f;

                // restructure and add to return list
                returnList.Add(new SavedControlButton()
                {
                    type = controlButtons[i].type,
                    overlayTransform = new OverlayTransform()
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
