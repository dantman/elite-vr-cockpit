using System.Collections;
using System.Collections.Generic;
using EVRC.Core.Actions;
using EVRC.Core.Overlay;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace ControlButtonTests
{
    public class StatusFlagControlButtonAssetTests
    {
        // enabled without eliteDangerousState asset assigned should throw exception
        // [UnityTest]
        // public IEnumerator ControlButton_StatusFlagAsset_ChangesTexture()
        // {
        //     // GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Overlay/Prefabs/ControlButton.prefab");
        //     // StatusFlagControlButtonAsset controlButtonAsset = AssetDatabase.LoadAssetAtPath<StatusFlagControlButtonAsset>("Assets/Actions/Assets/ControlButtons/ToggleCargoScoop_Buggy.asset");
        //     // var cb = ScriptableObject.Instantiate(prefab);
        //     //
        //     // var cbComponent = cb.GetComponent<ControlButton>();
        //     // cbComponent.controlButtonAsset = controlButtonAsset;
        //     // cbComponent.label = "TestComponentLabel";
        //     //
        //     // cb.SetActive(true);
        //     //
        //     // yield return null;
        //     // Assert.AreEqual(1,1);
        //
        // }

        
        // OnFlagsChanged with matching flag triggers refresh 
        // OnFlagsChanged with matching flag toggles isOn 
        
    }
}
