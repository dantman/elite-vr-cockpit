using EVRC.Core.Overlay;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EVRC.Core;

public class CockpitModeAnchorTests
{
    private static object[] ActivationCombinations =
    {
        new object[] { true, EDStatusFlags.InMainShip, new EDGuiFocus[] { EDGuiFocus.NoFocus, EDGuiFocus.InternalPanel, EDGuiFocus.ExternalPanel} },
        new object[] { false, EDStatusFlags.InMainShip, new EDGuiFocus[] { EDGuiFocus.FSSMode } },
        // Add more combinations as needed
    };

    //[Test]
    [Test, TestCaseSource(nameof(ActivationCombinations))]
    public void OnEDStatusAndGuiChanged_ChildGameObjectBecomesActive(bool expectedOutcome, EDStatusFlags activationStatusFlag, EDGuiFocus[] activationGuiFocuses)
    //public void OnEDStatusAndGuiChanged_ChildGameObjectBecomesActive()
    {
        // Create a new GameObject and attach the CockpitModeAnchor component
        GameObject parentGameObject = new GameObject("Parent");
        CockpitModeAnchor cockpitModeAnchor = parentGameObject.AddComponent<CockpitModeAnchor>();        

        // Create a child GameObject
        GameObject childGameObject = new GameObject("Child");
        childGameObject.transform.SetParent(parentGameObject.transform);
        childGameObject.SetActive(false);
        // This happens in OnEnable, but we need to force it here. Just make sure the child object is targeted
        cockpitModeAnchor.AddImmediateChildrenToList();
        // Assert that the child GameObject is initially inactive
        Assert.IsFalse(childGameObject.activeSelf);

        // Set the activationStatusFlag and activationGuiFocuses fields
        cockpitModeAnchor.activationStatusFlag = activationStatusFlag;
        cockpitModeAnchor.activationGuiFocuses = activationGuiFocuses;


        // Call the OnEDStatusAndGuiChanged method with matching parameters and Assert that the child GameObject becomes active
        cockpitModeAnchor.OnEDStatusAndGuiChanged(EDStatusFlags.InMainShip, EDGuiFocus.NoFocus);        
        Assert.AreEqual(expectedOutcome, childGameObject.activeSelf);


    }
}
