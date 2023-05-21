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
        public static int currentFileVersion = 5;

        [Header("Game Events")]
        public GameEvent overlayStateLoaded;


        private StaticLocationsManager staticLocationsManager;
        private ControlButtonManager controlButtonManager;

        private SavedStateFile loadedState;

        void Awake()
        {
            staticLocationsManager = GetComponentInChildren<StaticLocationsManager>();
            controlButtonManager = GetComponentInChildren<ControlButtonManager>();
        }

        void OnEnable()
        {
            LoadAndPlace();
        }

        private IEnumerator PlaceLoadedObjects(System.Action callback)
        {
            yield return StartCoroutine(controlButtonManager.PlaceWhenReady(loadedState.controlButtons));
            yield return StartCoroutine(staticLocationsManager.PlaceWhenReady(loadedState.staticLocations));

            callback?.Invoke();
        }


        public void OnEditLockChanged(bool editLocked)
        {
            // When unlocking, no need to do anything
            if (!editLocked) { return; }

            // Get Current State and save to file
            SavedStateFile currentState = new SavedStateFile();
            currentState.version = currentFileVersion;
            currentState.staticLocations = staticLocationsManager.GetCurrentStates();
            currentState.controlButtons = controlButtonManager.GetCurrentStates();
            OverlayFileUtils.WriteToFile(currentState);
        }

        public void LoadAndPlace()
        {
            loadedState = OverlayFileUtils.LoadFromFile();

            //Start all of the placement Coroutines, raise the loaded GameEvent when done.
            StartCoroutine(PlaceLoadedObjects(() => overlayStateLoaded.Raise()));
        }

        public void Rebuild()
        {
            controlButtonManager.Clear();
            LoadAndPlace();
        }

    }
}
