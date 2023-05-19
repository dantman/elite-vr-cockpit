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
    }
    
}