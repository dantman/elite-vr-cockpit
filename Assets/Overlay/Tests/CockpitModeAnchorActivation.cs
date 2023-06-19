using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EVRC.Core;
using EVRC.Core.Overlay;
using EVRC.Overlay.Tests;

public class CockpitModeAnchorActivation
{
    GameObject parentGameObject;
    CockpitModeAnchor cockpitModeAnchor;
    GameObject childGameObject;
    EliteDangerousState eliteState;



    [SetUp]
    public void SetUp()
    {
        // Create a new GameObject and attach the CockpitModeAnchor component
        parentGameObject = new GameObject("Parent");
        cockpitModeAnchor = parentGameObject.AddComponent<CockpitModeAnchor>();

        // Create a state object
        eliteState = ScriptableObject.CreateInstance<EliteDangerousState>();
        cockpitModeAnchor.eliteDangerousState = eliteState;

        // Create a child GameObject
        childGameObject = new GameObject("Child");
        childGameObject.transform.SetParent(parentGameObject.transform);
        childGameObject.SetActive(false);

        // For the OnEnable actions, which don't reliably run during unit tests
        cockpitModeAnchor.OnEnable();
    }

    private static StatusFlagActivationTestCase[] SingleAnchorActivationCases =
    {
        // No configuration provided. Default values should make the object stay inactive
        new StatusFlagActivationTestCase() {
            expectedOutcome=false,
            config = new anchorConfig() { statusFlag = default(EDStatusFlags), guiFocus = EDGuiFocus.PanelOrNoFocus},
            raisedStatusFlag = EDStatusFlags.InSRV,
            eliteStatusGuiFocus = EDGuiFocus.NoFocus
        },     
        // Elite State Object has a guifocus of FSS Mode when the StatusFlags changed
        // Anchor is configured for FSS Mode, so the activation status will not change if a new status Flag is raised
        new StatusFlagActivationTestCase() {
            expectedOutcome=false,
            config = new anchorConfig() { statusFlag = default(EDStatusFlags), guiFocus = EDGuiFocus.FSSMode },
            raisedStatusFlag = EDStatusFlags.LowFuel,
            eliteStatusGuiFocus = EDGuiFocus.FSSMode
        },
        // Elite State Object has a guifocus of ExternalPanel when the StatusFlags changed to InFighter
        // Anchor is configured for InFighter with PanelOrNoFocus, so we would expect this to be active
        new StatusFlagActivationTestCase() {
            expectedOutcome=true,
            config = new anchorConfig() { statusFlag = EDStatusFlags.InFighter, guiFocus = EDGuiFocus.PanelOrNoFocus },
            raisedStatusFlag = EDStatusFlags.InFighter,
            eliteStatusGuiFocus = EDGuiFocus.ExternalPanel
        },
        // Elite State Object has a guifocus of NoFocus when the StatusFlags changed to InMainShip
        // Anchor is configured for InFighter with PanelOrNoFocus, so we would expect this to be active
        new StatusFlagActivationTestCase() {
            expectedOutcome=true,
            config = new anchorConfig() { statusFlag = EDStatusFlags.InMainShip, guiFocus = EDGuiFocus.PanelOrNoFocus },
            raisedStatusFlag = EDStatusFlags.InMainShip,
            eliteStatusGuiFocus = EDGuiFocus.NoFocus
        },

    };

    /// <summary>
    /// Test to confirm that objects which start deactivated are correctly activated when matching status/guifocus are raised
    /// </summary>
    [Test, TestCaseSource(nameof(SingleAnchorActivationCases))]
    public void OnStatusChangeEvent_Activates_AnchorTargets(StatusFlagActivationTestCase testCase)
    {
        // Ensure that the child GameObject is initially inactive
        if (childGameObject.activeSelf) childGameObject.SetActive(false);

        // Set the activationStatusFlag and activationGuiFocuses fields for the test
        cockpitModeAnchor.activationSettings.Add(new CockpitModeAnchor.AnchorSetting()
        {
            activationStatusFlag = testCase.config.statusFlag,
            activationGuiFocus = testCase.config.guiFocus
        });
        //cockpitModeAnchor.activationStatusFlag = testCase.config.statusFlag;
        //cockpitModeAnchor.activationGuiFocus = testCase.config.guiFocus;


        // Simulate the state object for this scenario
        eliteState.guiFocus = testCase.eliteStatusGuiFocus;

        // Call the OnEDStatusAndGuiChanged method with the raised parameters
        cockpitModeAnchor.OnEDStatusFlagsChanged(testCase.raisedStatusFlag);
        Assert.AreEqual(testCase.expectedOutcome, childGameObject.activeSelf);
    }

    private static StatusFlagActivationTestCase[] DeactivationTestCases =
    {
        // Elite State Object has a guifocus of NoFocus when the StatusFlags changed to InSRV
        // Anchor is configured for Mainship, so the activation anchor should deactivate
        new StatusFlagActivationTestCase() {
            expectedOutcome=false,
            config = new anchorConfig() { statusFlag = EDStatusFlags.InMainShip, guiFocus = EDGuiFocus.PanelOrNoFocus },
            raisedStatusFlag = EDStatusFlags.InSRV,
            eliteStatusGuiFocus = EDGuiFocus.NoFocus
        },
        // Hypothetical future case where we want some special stuff to display when looking at one of the panels
        // raised status/focus don't match, so should deactivate
        new StatusFlagActivationTestCase() {
            expectedOutcome=false,
            config = new anchorConfig() { statusFlag = EDStatusFlags.InMainShip, guiFocus = EDGuiFocus.InternalPanel },
            raisedStatusFlag = EDStatusFlags.InMainShip,
            eliteStatusGuiFocus = EDGuiFocus.ExternalPanel
        },

    };

    /// <summary>
    /// Test to confirm that objects which start active are correctly deactivated
    /// </summary>
    [Test, TestCaseSource(nameof(DeactivationTestCases))]
    public void OnStatusChangeEvent_Deactivates_AnchorTargets(StatusFlagActivationTestCase testCase)
    {
        // Ensure that the child GameObject is initially Active
        if (!childGameObject.activeSelf) childGameObject.SetActive(true);

        // Set the activationStatusFlag and activationGuiFocuses fields for the test
        cockpitModeAnchor.activationSettings.Add(new CockpitModeAnchor.AnchorSetting()
        {
            activationStatusFlag = testCase.config.statusFlag,
            activationGuiFocus = testCase.config.guiFocus
        });
        //cockpitModeAnchor.activationStatusFlag = testCase.config.statusFlag;
        //cockpitModeAnchor.activationGuiFocus = testCase.config.guiFocus;


        // Call the OnEDStatusAndGuiChanged method with the raised parameters
        cockpitModeAnchor.OnEDStatusFlagsChanged(testCase.raisedStatusFlag);
        Assert.AreEqual(testCase.expectedOutcome, childGameObject.activeSelf);
    }

    [Test]
    public void ExitingFSSMode_Reactivates_MainshipMode()
    {
        /// This test ensures that when someone enters FSS mode, the Main cockpit stuff should deactivate
        /// then when they return to the mainship view, the cockpit controls should reactivate. The status flags will 
        /// stay the same in this circumstance

        // Ensure that the child GameObject is initially inactive
        if (childGameObject.activeSelf) childGameObject.SetActive(false);

        // Clear the previous activationStatusFlags, just in case
        cockpitModeAnchor.activationSettings.Clear();
        // Set the activationStatusFlag and activationGuiFocuses fields for the test
        cockpitModeAnchor.activationSettings.Add(new CockpitModeAnchor.AnchorSetting()
        {
            activationStatusFlag = EDStatusFlags.InMainShip,
            activationGuiFocus = EDGuiFocus.PanelOrNoFocus
        });
        eliteState.statusFlags = EDStatusFlags.InMainShip;

        //// Set the activationStatusFlag and activationGuiFocuses fields for the test
        //cockpitModeAnchor.activationStatusFlag = EDStatusFlags.InMainShip;
        //cockpitModeAnchor.activationGuiFocus = EDGuiFocus.PanelOrNoFocus;


        // Activate FSS Mode
        cockpitModeAnchor.OnGuiFocusChanged(EDGuiFocus.FSSMode);
        // Assert that Mainship is inactive
        Assert.AreEqual(childGameObject.activeSelf, false);

        // Change back to No Focus
        cockpitModeAnchor.OnGuiFocusChanged(EDGuiFocus.NoFocus);
        // Assert that Mainship reactivates
        Assert.AreEqual(childGameObject.activeSelf, true);
    }

    private static GuiFocusActivationTestCase[] guiFocusActivationTestCases =
    {
        // Raising FSS should activate FSS Mode, even if StatusFlag also matches
        new GuiFocusActivationTestCase()
        {
            expectedOutcome=true,
            config = new anchorConfig() { statusFlag = EDStatusFlags.InMainShip, guiFocus = EDGuiFocus.FSSMode },
            raisedGuiFocus = EDGuiFocus.FSSMode,
            eliteStatusFlags = EDStatusFlags.InMainShip
        },
        // Raising FSS should activate FSS Mode, even if StatusFlag has the default value
        new GuiFocusActivationTestCase()
        {
            expectedOutcome=true,
            config = new anchorConfig() { statusFlag = default(EDStatusFlags), guiFocus = EDGuiFocus.FSSMode },
            raisedGuiFocus = EDGuiFocus.FSSMode,
            eliteStatusFlags = EDStatusFlags.InMainShip
        },
        // Raising InternalPanel should not deactivate if anchor is set to PanelOrNoFocus
        new GuiFocusActivationTestCase()
        {
            expectedOutcome=true,
            config = new anchorConfig() { statusFlag = EDStatusFlags.InSRV, guiFocus = EDGuiFocus.PanelOrNoFocus },
            raisedGuiFocus = EDGuiFocus.InternalPanel,
            eliteStatusFlags = EDStatusFlags.InSRV
        },
    };

    [Test, TestCaseSource(nameof(guiFocusActivationTestCases))]
    public void OnGuiFocusEvent_Activates_AnchorTargets(GuiFocusActivationTestCase testCase)
    {
        // Ensure that the child GameObject is initially inactive
        if (childGameObject.activeSelf) childGameObject.SetActive(false);

        // Clear the previous activationStatusFlags, just in case
        cockpitModeAnchor.activationSettings.Clear();
        // Set the activationStatusFlag and activationGuiFocuses fields for the test
        cockpitModeAnchor.activationSettings.Add(new CockpitModeAnchor.AnchorSetting()
        {
            activationStatusFlag = testCase.config.statusFlag,
            activationGuiFocus = testCase.config.guiFocus
        });

        //// Set the activationStatusFlag and activationGuiFocuses fields for the test
        //cockpitModeAnchor.activationStatusFlag = testCase.config.statusFlag;
        //cockpitModeAnchor.activationGuiFocus = testCase.config.guiFocus;

        // Simulate the state object for this scenario
        eliteState.statusFlags = testCase.eliteStatusFlags;

        // Call the OnEDStatusAndGuiChanged method with the raised parameters
        cockpitModeAnchor.OnGuiFocusChanged(testCase.raisedGuiFocus);
        Assert.AreEqual(testCase.expectedOutcome, childGameObject.activeSelf);
    }

    [Test]
    public void MultiAnchor_Activates_AnchorTargets()
    {
        // Ensure that the child GameObject is initially inactive
        if (childGameObject.activeSelf) childGameObject.SetActive(false);

        // Clear the previous activationStatusFlags, just in case
        cockpitModeAnchor.activationSettings.Clear();
                
        // Set the first AnchorSetting to work for StationServices
        cockpitModeAnchor.activationSettings.Add(new CockpitModeAnchor.AnchorSetting()
        {
            activationStatusFlag = default(EDStatusFlags),
            activationGuiFocus = EDGuiFocus.StationServices
        });

        // Set the second AnchorSetting to work for GalaxyMap
        cockpitModeAnchor.activationSettings.Add(new CockpitModeAnchor.AnchorSetting()
        {
            activationStatusFlag = default(EDStatusFlags),
            activationGuiFocus = EDGuiFocus.GalaxyMap
        });

        // Simulate the state object for this scenario
        eliteState.statusFlags = EDStatusFlags.InMainShip | EDStatusFlags.HardpointsDeployed | EDStatusFlags.InMainShip;


        // Raise with a non-matching guiFocus
        cockpitModeAnchor.OnGuiFocusChanged(EDGuiFocus.NoFocus);
        Assert.AreEqual(false, childGameObject.activeSelf);

        // Raise with a guiFocus for AnchorSetting 1
        cockpitModeAnchor.OnGuiFocusChanged(EDGuiFocus.StationServices);
        Assert.AreEqual(true, childGameObject.activeSelf);

        // Raise with a non-matching guiFocus
        cockpitModeAnchor.OnGuiFocusChanged(EDGuiFocus.NoFocus);
        Assert.AreEqual(false, childGameObject.activeSelf);

        // Raise with a guiFocus for AnchorSetting 2
        cockpitModeAnchor.OnGuiFocusChanged(EDGuiFocus.GalaxyMap);
        Assert.AreEqual(true, childGameObject.activeSelf);
    }
}

