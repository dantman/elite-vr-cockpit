using EVRC.Core.Overlay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC.Core.Tests
{
    /// <summary>
    /// Configuration applied to test CockpitModeAnchor
    /// </summary>
    public struct anchorConfig
    {
        public EDStatusFlags statusFlag;
        public EDGuiFocus guiFocus;
    }


    /// <summary>
    /// Hold test case configuration details for tests related to placing a ControlButton in the scene.
    /// </summary>
    public struct ControlButtonPlacementTestCase
    {
        public bool expectedOutcome;
        // Applied to the test cockpitModeAnchor
        public anchorConfig config;

        // Button info for placement
        public SavedControlButton savedControlButton;

        // Customize the display of the test case in the TestRunner Window2
        public override string ToString()
        {
                
            return $"Btn: {savedControlButton.type}=>{savedControlButton.anchorGuiFocus}/{savedControlButton.anchorStatusFlag} || Anchor: {config.guiFocus}|{config.statusFlag}";
        }
    }

    /// <summary>
    /// Hold test case configuration details for tests related to activating or deactivating objects based on the current Status Flags and GuiFocus from Elite Dangerous
    /// </summary>
    public struct ModeAnchorActivationTestCase
    {
        public bool expectedOutcome;
        // Applied to the test cockpitModeAnchor
        public anchorConfig config;

        // Settings for the event that will be raised in the test case
        public EDStatusFlags raisedStatusFlag;
        public EDGuiFocus raisedGuiFocus;
    }
}
