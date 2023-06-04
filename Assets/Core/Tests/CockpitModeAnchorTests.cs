using EVRC.Core.Overlay;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EVRC.Core;
using EVRC.Core.Tests;

public class CockpitModeAnchorTests
{
    GameObject parentGameObject;
    CockpitModeAnchor cockpitModeAnchor;
    GameObject childGameObject;

    

    [SetUp]
    public void SetUp()
    {
        // Create a new GameObject and attach the CockpitModeAnchor component
        parentGameObject = new GameObject("Parent");
        cockpitModeAnchor = parentGameObject.AddComponent<CockpitModeAnchor>();

        // Create a child GameObject
        childGameObject = new GameObject("Child");
        childGameObject.transform.SetParent(parentGameObject.transform);
        childGameObject.SetActive(false);
        
        // For the OnEnable actions, which don't reliably run during unit tests
        cockpitModeAnchor.OnEnable();
    }

    private static ModeAnchorActivationTestCase[] ActivationTestCases =
    {
    // configured for FSS Mode, raised with FSS Mode Gui Focus and Mainship
    new ModeAnchorActivationTestCase() {
        expectedOutcome=true,
        config = new anchorConfig() { guiFocus = EDGuiFocus.FSSMode },
        raisedStatusFlag = EDStatusFlags.InMainShip,
        raisedGuiFocus = EDGuiFocus.FSSMode
    },
        
    // Hypothetical future case where we want some special stuff to display when looking at one of the panels
    new ModeAnchorActivationTestCase() {
        expectedOutcome=true,
        config = new anchorConfig() { statusFlag = EDStatusFlags.InMainShip, guiFocus = EDGuiFocus.InternalPanel },
        raisedStatusFlag = EDStatusFlags.InMainShip,
        raisedGuiFocus = EDGuiFocus.InternalPanel
    },
    // No configuration provided. Should remain inactive
    new ModeAnchorActivationTestCase() {
        expectedOutcome=false,
        config = new anchorConfig() { },
        raisedStatusFlag = EDStatusFlags.InSRV,
        raisedGuiFocus = EDGuiFocus.NoFocus
    },
    // 
    new ModeAnchorActivationTestCase() {
        expectedOutcome=false,
        config = new anchorConfig() { },
        raisedStatusFlag = EDStatusFlags.InSRV,
        raisedGuiFocus = EDGuiFocus.NoFocus
    },

};

    /// <summary>
    /// Test to confirm that objects which start deactivated are correctly activated when matching status/guifocus are raised
    /// </summary>
    [Test, TestCaseSource(nameof(ActivationTestCases))]
    public void OnStatusChange_Activation(ModeAnchorActivationTestCase testCase)
    {
        // Ensure that the child GameObject is initially inactive
        if (childGameObject.activeSelf) childGameObject.SetActive(false);

        // Set the activationStatusFlag and activationGuiFocuses fields for the test
        cockpitModeAnchor.activationStatusFlag = testCase.config.statusFlag;
        cockpitModeAnchor.activationGuiFocus = testCase.config.guiFocus;


        // Call the OnEDStatusAndGuiChanged method with the raised parameters
        cockpitModeAnchor.OnEDStatusAndGuiChanged(testCase.raisedStatusFlag, testCase.raisedGuiFocus);
        Assert.AreEqual(testCase.expectedOutcome, childGameObject.activeSelf);
    }

    private static ModeAnchorActivationTestCase[] DeactivationTestCases =
    {
    // configured for FSS Mode, raised without FSS Mode Gui Focus
    new ModeAnchorActivationTestCase() {
        expectedOutcome=false,
        config = new anchorConfig() { guiFocus = EDGuiFocus.FSSMode },
        raisedStatusFlag = EDStatusFlags.InMainShip,
        raisedGuiFocus = EDGuiFocus.NoFocus
    },
    // configured for MainShip, raised with FSS Mode Gui Focus and Mainship
    new ModeAnchorActivationTestCase() {
        expectedOutcome=false,
        config = new anchorConfig() { statusFlag = EDStatusFlags.InMainShip },
        raisedStatusFlag = EDStatusFlags.InMainShip,
        raisedGuiFocus = EDGuiFocus.FSSMode
    },
    // Hypothetical future case where we want some special stuff to display when looking at one of the panels
    // raised status/focus don't match, so should deactivate
    new ModeAnchorActivationTestCase() {
        expectedOutcome=false,
        config = new anchorConfig() { statusFlag = EDStatusFlags.InMainShip, guiFocus = EDGuiFocus.InternalPanel },
        raisedStatusFlag = EDStatusFlags.InMainShip,
        raisedGuiFocus = EDGuiFocus.ExternalPanel
    },

};

    /// <summary>
    /// Test to confirm that objects which start active are correctly deactivated
    /// </summary>
    [Test, TestCaseSource(nameof(DeactivationTestCases))]
    public void OnStatusChange_Deactivation(ModeAnchorActivationTestCase testCase)
    {
        // Ensure that the child GameObject is initially Active
        if (!childGameObject.activeSelf) childGameObject.SetActive(true);

        // Set the activationStatusFlag and activationGuiFocuses fields for the test
        cockpitModeAnchor.activationStatusFlag = testCase.config.statusFlag;
        cockpitModeAnchor.activationGuiFocus = testCase.config.guiFocus;


        // Call the OnEDStatusAndGuiChanged method with the raised parameters
        cockpitModeAnchor.OnEDStatusAndGuiChanged(testCase.raisedStatusFlag, testCase.raisedGuiFocus);
        Assert.AreEqual(testCase.expectedOutcome, childGameObject.activeSelf);
    }
}

