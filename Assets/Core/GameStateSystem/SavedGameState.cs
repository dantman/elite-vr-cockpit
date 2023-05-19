using EVRC.Core.Overlay;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EVRC.Core
{
    /// <summary>
    /// The current state that is used in the EVRC system
    /// </summary>
    [CreateAssetMenu(menuName = Constants.STATE_OBJECT_PATH + "/Saved Game State"), Serializable]
    public class SavedGameState : GameState
    {
        public int fileVersion;
        public List<SavedGameObject> staticLocations;
        public List<SavedControlButton> controlButtons;
        public List<SavedBooleanSetting> booleanSettings; 
        public bool loaded = false;

        public void Reset()
        {
            fileVersion = 0;
            staticLocations = new List<SavedGameObject>();
            controlButtons = new List<SavedControlButton>();
            booleanSettings = new List<SavedBooleanSetting>();
            loaded = false;
        }

        public override string GetStatusText()
        {
            return loaded ? "Loaded" : "Not Loaded";
        }

        public void Load()
        {
            SavedStateFile file = OverlayFileUtils.LoadFromFile();
            fileVersion = file.version;
            staticLocations = file.staticLocations.ToList();
            controlButtons = file.controlButtons.ToList();
            booleanSettings = file.booleanSettings.ToList();
            loaded = true;
        }

        public void Save()
        {
            SavedStateFile file = new SavedStateFile();
            file.version = fileVersion;
            file.staticLocations = staticLocations.ToArray();
            file.controlButtons = controlButtons.ToArray();
            file.booleanSettings = booleanSettings.ToArray();

            OverlayFileUtils.WriteToFile(file);
        }

        

    }
}
