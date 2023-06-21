using EVRC.Core;
using EVRC.Core.Overlay;

namespace EVRC.Overlay.Tests
{
    /// <summary>
    /// Configuration applied to test CockpitModeAnchor
    /// </summary>
    public struct anchorConfig
    {
        public EDStatusFlags statusFlag;
        public EDGuiFocus guiFocus;
        public bool ignoreCockpitFocusChanges;
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
    public struct StatusFlagActivationTestCase
    {
        public bool expectedOutcome;
        // Applied to the test cockpitModeAnchor
        public anchorConfig config;

        // Settings for the event that will be raised in the test case
        public EDStatusFlags raisedStatusFlag;

        // Setting for what the GuiFocus will be when the event is raised
        public EDGuiFocus eliteStatusGuiFocus;

        // Customize the display of the test case in the TestRunner Window2
        public override string ToString()
        {

            return $"Raised: =>{raisedStatusFlag} w/ {eliteStatusGuiFocus} || Anchor: {config.guiFocus}|{config.statusFlag}";
        }
    }

    /// <summary>
    /// Hold test case configuration details for tests related to activating or deactivating objects based on the current Status Flags and GuiFocus from Elite Dangerous
    /// </summary>
    public struct GuiFocusActivationTestCase
    {
        public bool expectedOutcome;
        // Applied to the test cockpitModeAnchor
        public anchorConfig config;
        
        // Setting for what the StatusFlags will be when the event is raised
        public EDStatusFlags eliteStatusFlags;

        // Settings for the guiFocus that will be raised in the test case
        public EDGuiFocus raisedGuiFocus;

        // Customize the display of the test case in the TestRunner Window2
        public override string ToString()
        {

            return $"Raised: =>{raisedGuiFocus} w/ {eliteStatusFlags} || Anchor: {config.guiFocus}|{config.statusFlag}";
        }
    }
}