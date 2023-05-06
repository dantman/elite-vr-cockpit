using System.Collections;
using System.Collections.Generic;
using EVRC.Core;
using EVRC.Core.Overlay;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    /// <summary>
    /// Registers callbacks for settings view to allow Overlay Settings to be updated from the desktop view (through
    /// ScriptableObjects
    /// </summary>
    public class SettingsViewController : MonoBehaviour
    {
        // State Object (shared by Overlay)
        [Tooltip("Boolean Settings objects that will be set through the UI")]
        public List<BoolGameSetting> boolGameSettings;

        public VisualElement root;

        public void RegisterCallbacks()
        {
            Debug.LogWarning("SettingsView not configured (temporarily)");

        }
    }
}
