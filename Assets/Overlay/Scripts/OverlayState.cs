using System;
using UnityEngine;

namespace EVRC.Core.Overlay
{
    [Serializable]
    public struct OverlayTransform
    {
        public Vector3 pos;
        public Vector3 rot;
    }

    [Serializable]
    public struct SavedGameObject
    {
        public string key;
        public OverlayTransform overlayTransform;
    }

    [Serializable]
    public struct SavedControlButton
    {
        public string type;
        public string anchorStatusFlag;
        public string anchorGuiFocus;
        public OverlayTransform overlayTransform;
    }

    [Serializable]
    public struct SavedBooleanSetting
    {
        public string name;
        public bool value;
    }

    /// <summary>
    /// Helper struct for saving/reading the SavedState file in the correct format
    /// </summary>
    [Serializable]
    public struct SavedStateFile
    {
        public int version;
        public SavedGameObject[] staticLocations;
        public SavedControlButton[] controlButtons;
        public SavedBooleanSetting[] booleanSettings;
       
        
        public SavedStateFile(bool useDefault)
        {
            this.version = 5;
            this.booleanSettings = new SavedBooleanSetting[0];
            this.controlButtons = new SavedControlButton[0];
            this.staticLocations = new SavedGameObject[0];
        }

    }
    
}