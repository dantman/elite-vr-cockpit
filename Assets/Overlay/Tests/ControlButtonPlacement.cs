using EVRC.Core;
using EVRC.Core.Overlay;
using EVRC.Overlay.Tests;
using NUnit.Framework;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;


public class ControlButtonPlacement
{
    GameObject anchorParentGameObject;
    CockpitModeAnchor cockpitModeAnchor;
    GameObject controlButtonManagerInstance;
    ControlButtonManager controlButtonManager;
    private static OverlayTransform testOverlayTransform;

    [SetUp]
    public void SetUp()
    {
        // use this unless there's a reason to test a different position/rotation
        testOverlayTransform = new OverlayTransform()
        {
            pos = Vector3.zero,
            rot = Vector3.zero
        };

        // Create the prefab ControlButtonManager
#if UNITY_EDITOR
        string prefabPath = "Assets/Overlay/Prefabs/ControlButtonManager.prefab";
        GameObject controlButtonManagerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (controlButtonManagerPrefab == null) { Debug.LogError($"ControlButtonManager prefab could not be found. Expected path: {prefabPath}"); }

        // Instantiate the prefab
        controlButtonManagerInstance = PrefabUtility.InstantiatePrefab(controlButtonManagerPrefab) as GameObject;
        controlButtonManager = controlButtonManagerInstance.GetComponent<ControlButtonManager>();
        controlButtonManager.enabled = false;
#endif

        // Create a new GameObject and attach the CockpitModeAnchor component
        anchorParentGameObject = new GameObject("Anchor");
        cockpitModeAnchor = anchorParentGameObject.AddComponent<CockpitModeAnchor>();

        // For the OnEnable actions, which don't reliably run during unit tests
        cockpitModeAnchor.OnEnable();
        controlButtonManager.OnEnable();
    }

    [Test]
    public void ControlButtonManager_Instantiates_Button()
    {
        SavedControlButton savedControlButton = new SavedControlButton()
        {
            type = "Hyperspace",
            anchorStatusFlag = "InMainShip",
            anchorGuiFocus = "NoFocus",
            overlayTransform = testOverlayTransform
        };
        Assert.DoesNotThrow(() =>
        {
            controlButtonManager.PlaceSavedControlButton(savedControlButton);
        });
        Assert.AreEqual(1, controlButtonManager.ControlButtons.Count);

        ControlButton controlButton = controlButtonManager.ControlButtons[0];
        Assert.AreEqual(savedControlButton.type, controlButton.controlButtonAsset.name);
        Assert.AreEqual(EDGuiFocus.NoFocus, controlButton.configuredGuiFocus);
        Assert.AreEqual(EDStatusFlags.InMainShip, controlButton.configuredStatusFlag);

    }

    private static ControlButtonPlacementTestCase[] PlacementTestCases =
    {
    // Test a MainShip button
    new ControlButtonPlacementTestCase() {
        expectedOutcome=true,
        config = new anchorConfig() { guiFocus = EDGuiFocus.NoFocus, statusFlag = EDStatusFlags.InMainShip },
        savedControlButton = new SavedControlButton()
        {
            type = "Hyperspace",
            anchorStatusFlag = "InMainShip",
            anchorGuiFocus = "NoFocus",
            overlayTransform = testOverlayTransform
        }
    },
    // Test an SRV button
    new ControlButtonPlacementTestCase() {
        expectedOutcome=true,
        config = new anchorConfig() { guiFocus = EDGuiFocus.NoFocus, statusFlag = EDStatusFlags.InSRV },
        savedControlButton = new SavedControlButton()
        {
            type = "ToggleCargoScoop_Buggy",
            anchorStatusFlag = "InSRV",
            anchorGuiFocus = "NoFocus",
            overlayTransform = testOverlayTransform
        }
    },
    // GuiFocus not defined in file (on purpose)
    new ControlButtonPlacementTestCase() {
        expectedOutcome=true,
        config = new anchorConfig() { statusFlag = EDStatusFlags.InMainShip, guiFocus = EDGuiFocus.PanelOrNoFocus },
        savedControlButton = new SavedControlButton()
        {
            type = "Hyperspace",
            anchorStatusFlag = "InMainShip",
            anchorGuiFocus = "",
            overlayTransform = testOverlayTransform
        }
    },
    // StatusFlag not defined in file (on purpose)
    new ControlButtonPlacementTestCase() {
        expectedOutcome=true,
        config = new anchorConfig() { guiFocus = EDGuiFocus.FSSMode },
        savedControlButton = new SavedControlButton()
        {
            type = "Hyperspace",
            anchorStatusFlag = "",
            anchorGuiFocus = "FSSMode",
            overlayTransform = testOverlayTransform
        }
    },
};

    [Test, TestCaseSource(nameof(PlacementTestCases))]
    public void ControlButton_Has_Correct_Parent_Anchor(ControlButtonPlacementTestCase testCase)
    {
        cockpitModeAnchor.activationSettings.Add(new CockpitModeAnchor.AnchorSetting()
        {
            activationGuiFocus = testCase.config.guiFocus,
            activationStatusFlag = testCase.config.statusFlag
        });

        // Make sure the ControlButtonManager actually contains a reference to the Anchor
        Assert.Contains(cockpitModeAnchor, controlButtonManager.CockpitModeAnchors);


        // Place the controlbutton through the ControlButtonManager
        controlButtonManager.PlaceSavedControlButton(testCase.savedControlButton);
        // The CockpitModeAnchor should become the parent of the new ControlButton
        ControlButton newestButton = controlButtonManager.ControlButtons.Last();
        var newButtonParent = newestButton.transform.parent.gameObject;
        Assert.AreEqual(anchorParentGameObject, newButtonParent);
    }

    /// <summary>
    /// Test adding the same controlButton to two different anchors (one at a time) to ensure there isn't conflict in matching SavedControlButton info to Anchors
    /// </summary>
    [Test]
    public void SameControlButton_Multiple_Anchors()
    {
        #region --------------ARRANGE---------------
        // Create a new GameObject and attach the CockpitModeAnchor component
        GameObject anchorParentGameObject_Two = new GameObject("Anchor2");
        CockpitModeAnchor cockpitModeAnchor_Two = anchorParentGameObject_Two.AddComponent<CockpitModeAnchor>();

        // For the OnEnable actions, which don't reliably run during unit tests
        cockpitModeAnchor_Two.OnEnable();


        // Set Anchor One to be Mainship
        cockpitModeAnchor.activationSettings.Add(new CockpitModeAnchor.AnchorSetting()
        {
            activationGuiFocus = default(EDGuiFocus),
            activationStatusFlag = EDStatusFlags.InMainShip
        });
        //cockpitModeAnchor.activationGuiFocus = default(EDGuiFocus);
        //cockpitModeAnchor.activationStatusFlag = EDStatusFlags.InMainShip;


        // Set Anchor TWO to be SRV
        cockpitModeAnchor_Two.activationSettings.Add(new CockpitModeAnchor.AnchorSetting()
        {
            activationGuiFocus = default(EDGuiFocus),
            activationStatusFlag = EDStatusFlags.InSRV
        });
        //cockpitModeAnchor_Two.activationGuiFocus = default(EDGuiFocus);
        //cockpitModeAnchor_Two.activationStatusFlag = EDStatusFlags.InSRV;

        //Create the SavedControlButtons
        //One for Mainship
        SavedControlButton SpeedZero_Mainship = new SavedControlButton()
        {
            type = "Hyperspace",
            anchorStatusFlag = "InMainShip",
            anchorGuiFocus = "",
            overlayTransform = testOverlayTransform
        };
        //One for Mainship
        SavedControlButton SpeedZero_SRV = new SavedControlButton()
        {
            type = "Hyperspace",
            anchorStatusFlag = "InSRV",
            anchorGuiFocus = "",
            overlayTransform = testOverlayTransform
        };
        #endregion

        //// Act
        // Place the controlbuttons through the ControlButtonManager
        controlButtonManager.PlaceSavedControlButton(SpeedZero_Mainship);
        controlButtonManager.PlaceSavedControlButton(SpeedZero_SRV);

        //// Assert
        // There should be two ControlButton Instances, one for each of the newly added SavedControlButtons
        Assert.AreEqual(2, controlButtonManager.ControlButtons.Count);
        // The two instances should not have the same parent GameObject
        Assert.AreNotEqual(controlButtonManager.ControlButtons[0].transform.parent.gameObject, controlButtonManager.ControlButtons[1].transform.parent.gameObject);
    }

    [Test]
    public void Anchor_IsCreated_IfNotPresent()
    {
        #region ------------ Arrange ---------------
        // create SavedControlButton that doesn't match the existing anchor
        SavedControlButton NightVision = new SavedControlButton()
        {
            type = "NightVisionToggle",
            anchorStatusFlag = "InFighter",
            anchorGuiFocus = "",
            overlayTransform = testOverlayTransform
        };
        // Assert that scene doesn't have any matching Anchors
        Assert.AreEqual(0, controlButtonManager.CockpitModeAnchors
            .Where(anchor => anchor.activationSettings.Any(x => x.activationGuiFocus == EDGuiFocus.PanelOrNoFocus))
            .Where(anchor => anchor.activationSettings.Any(y => y.activationStatusFlag == EDStatusFlags.InFighter))
            .Count());

        controlButtonManager.parentObject = new GameObject("SeatedOrigin");

#if UNITY_EDITOR
        string prefabPath = "Assets/Overlay/Prefabs/CockpitAnchor.prefab";
        GameObject cockpitAnchorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (cockpitAnchorPrefab == null) { Debug.LogError($"ControlButtonManager prefab could not be found. Expected path: {prefabPath}"); }
#endif

        controlButtonManager.CockpitAnchorPrefab = cockpitAnchorPrefab;
        #endregion

        #region ------------ Act ---------------
        int initialAnchorCount = controlButtonManager.CockpitModeAnchors.Count;

        // Place the controlbutton through the ControlButtonManager
        controlButtonManager.PlaceSavedControlButton(NightVision);

        #endregion

        #region ------------ Assert ---------------
        // No warnings or errors raised
        LogAssert.NoUnexpectedReceived();
        // Should have exactly one more CockpitModeAnchor in the list
        Assert.AreEqual(initialAnchorCount + 1, controlButtonManager.CockpitModeAnchors.Count);
        #endregion
    }
    // CockpitModeAnchor is created if necessary
    // new gameObject with expected properties is present in the scene
    // gameObject has the correct parent object (cockpitAnchor)
    // Target property updated to include new object


}

