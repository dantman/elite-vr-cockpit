using EVRC.Core;
using EVRC.Core.Overlay;
using System;
using UnityEngine;

namespace EVRC.DesktopUI
{
    [Serializable]
    public class ControlButtonDesktopItem
    {
        public string name; //friendly name
        public string officialName; // official name found in the bindings file
        public Texture texture; // texture for button image
        public ControlButtonAsset controlButtonAsset;
        public SavedControlButton savedControlButton;

    }
}