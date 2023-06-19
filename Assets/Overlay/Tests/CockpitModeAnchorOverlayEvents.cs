using EVRC.Core;
using EVRC.Core.Overlay;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CockpitModeAnchorOverlayEvents
{
    GameObject cockpitModeAnchorInstance;
    CockpitModeAnchor cockpitModeAnchor;
    GameObject childGameObject;
    GameObject cockpitModeAnchorPrefab;

    [SetUp]
    public void SetUp()
    {
        // Create the prefab ControlButtonManager
#if UNITY_EDITOR
        string prefabPath = "Assets/Overlay/Prefabs/CockpitAnchor.prefab";
        cockpitModeAnchorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (cockpitModeAnchorPrefab == null) { Debug.LogError($"ControlButtonManager prefab could not be found. Expected path: {prefabPath}"); }
#endif

        var createdReturn = CreatePrefab();
        cockpitModeAnchorInstance = createdReturn.Item1;
        cockpitModeAnchor = createdReturn.Item2;
        childGameObject = createdReturn.Item3;


    }

    private (GameObject,CockpitModeAnchor, GameObject) CreatePrefab()
    {
        // Instantiate the prefab
        var instance = PrefabUtility.InstantiatePrefab(cockpitModeAnchorPrefab) as GameObject;
        var anchor = instance.GetComponent<CockpitModeAnchor>();

        // Create a child GameObject
        GameObject childObj = new GameObject("Child");
        childObj.transform.SetParent(instance.transform);
        childObj.SetActive(false);

        // For the OnEnable actions, which don't reliably run during unit tests
        anchor.OnEnable();

        return (instance, anchor, childObj);
    }

    /// <summary>
    /// MenuMode should deactivate all CockpitModeAnchors in the scene
    /// </summary>
    [Test]
    public void ActivateMenuMode_Deactivates_AnchorTargets()
    {
        // Arrange
        if (childGameObject.activeSelf) childGameObject.SetActive(true); // make sure the child object starts enabled

        // Act
        cockpitModeAnchor.OnMenuModeActive(true); // simulate menuMode activation

        // Assert
        Assert.IsFalse(childGameObject.activeSelf);
    }

    [Test]
    public void DeactivateMenuMode_Activates_CorrectAnchorOnly()
    {
        #region -----------------Arrange--------------------------
        cockpitModeAnchor.activationSettings.Add(
            new CockpitModeAnchor.AnchorSetting()
            {
                activationGuiFocus = EDGuiFocus.FSSMode,
                activationStatusFlag = default(EDStatusFlags)
            });

        //Create a second anchor that is configured for System Map
        var anchorTwo = CreatePrefab();
        GameObject anchorTwoParent = anchorTwo.Item1;
        CockpitModeAnchor anchorTwoAnchor = anchorTwo.Item2;
        GameObject anchorTwoChild = anchorTwo.Item3;
        anchorTwoChild.SetActive(false);
        //anchorTwoAnchor.activationGuiFocus = EDGuiFocus.SystemMap;
        anchorTwoAnchor.activationSettings.Add(
            new CockpitModeAnchor.AnchorSetting()
            {
                activationGuiFocus = EDGuiFocus.SystemMap,
                activationStatusFlag = default(EDStatusFlags)
            });

        //Create a third anchor that is configured for SRV Mode
        var anchorThree = CreatePrefab();
        GameObject anchorThreeParent = anchorThree.Item1;
        CockpitModeAnchor anchorThreeAnchor = anchorThree.Item2;
        GameObject anchorThreeChild = anchorThree.Item3;
        anchorThreeChild.SetActive(false);
        //anchorThreeAnchor.activationStatusFlag = EDStatusFlags.InSRV;
        anchorThreeAnchor.activationSettings.Add(
            new CockpitModeAnchor.AnchorSetting()
            {
                activationGuiFocus = EDGuiFocus.PanelOrNoFocus,
                activationStatusFlag = EDStatusFlags.InSRV
            });

        //Configure the simulated EliteDangerousState as if we're in FSS mode when we deactivate
        string eliteStatePath = "Assets/Core/Assets/GameState/Elite Dangerous State.asset";
        var eliteDangerousState = AssetDatabase.LoadAssetAtPath<EliteDangerousState>(eliteStatePath);
        if (eliteDangerousState == null) { Debug.LogError($"EliteDangerous State ScriptableObject could not be found. Expected path: {eliteStatePath}"); }
        eliteDangerousState.guiFocus = EDGuiFocus.FSSMode;
        eliteDangerousState.statusFlags = EDStatusFlags.InMainShip;
        #endregion


        // Act
        cockpitModeAnchor.OnMenuModeActive(false); //simulate leaving menu mode


        // Assert
        // the FSS anchor child should be active, but the others should not
        Assert.IsTrue(childGameObject.activeSelf);
        Assert.IsFalse(anchorTwoChild.activeSelf);
        Assert.IsFalse(anchorThreeChild.activeSelf);
    }

    [Test]
    public void DisableOpenVR_Deactivates_AnchorTargets()
    {
        // Arrange
        childGameObject.SetActive(true);
        
        // Act 
        cockpitModeAnchor.OnOpenVRStart(false);

        // Assert
        Assert.IsFalse(childGameObject.activeSelf);
    }

    [Test]
    public void StopEliteDangerous_Deactivates_AnchorTargets()
    {
        // Arrange
        childGameObject.SetActive(true);

        // Act 
        cockpitModeAnchor.OnGameStart(false);

        // Assert
        Assert.IsFalse(childGameObject.activeSelf);
    }
}

