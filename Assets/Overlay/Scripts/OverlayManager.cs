using EVRC.Core.Actions;
using EVRC.Core.Overlay;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EVRC.Core
{
    /// <summary>
    /// Singleton manager of all overlay elements in the scene. 
    /// </summary>
    public class OverlayManager : MonoBehaviour
    {
        internal static int currentFileVersion = 5;

        [Header("State Assets")]
        public ControlButtonAssetCatalog controlButtonCatalog;

        public StaticLocationsManager staticLocationsManager;
        public ControlButtonManager controlButtonManager;

        void Awake()
        {
            staticLocationsManager = GetComponentInChildren<StaticLocationsManager>();
            controlButtonManager = GetComponentInChildren<ControlButtonManager>();
        }

        void OnEnable()
        {
            OverlayState loadedState = OverlayFileUtils.LoadFromFile();

            controlButtonManager.PlaceAll(loadedState);
            staticLocationsManager.PlaceAll(loadedState);

        }

        void OnDisable()
        {
            
        }



        public void OnEditLockChanged(bool editLocked)
        {
            // When unlocking, no need to do anything
            if (!editLocked) { return; }

            // Get Current State and save to file
            OverlayState currentState = new OverlayState();
            currentState.version = currentFileVersion;
            currentState.staticLocations = staticLocationsManager.GetCurrentStates();
            currentState.controlButtons = controlButtonManager.GetCurrentStates();
            OverlayFileUtils.WriteToFile(currentState);
        }

    }
}
