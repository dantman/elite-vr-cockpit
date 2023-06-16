using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EVRC.Core.Actions;
using UnityEngine;
using UnityEngine.Windows;

namespace EVRC.Core.Overlay
{
    public class ControlButtonManager : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("CockpitMode Anchors will be placed as children")] public GameObject parentObject;
        public GameObject CockpitAnchorPrefab;
        public ControlButtonAssetCatalog controlButtonCatalog;
        public GameEvent controlButtonsLoaded;


        [Header("New Button Spawn Settings")]
        public static Vector3 spawnZoneStart = new Vector3(0,0.9f,0.5f);

        private List<ControlButton> controlButtons;
        internal List<ControlButton> ControlButtons { get { return controlButtons; } }

        private List<CockpitModeAnchor> cockpitModeAnchors;
        internal List<CockpitModeAnchor> CockpitModeAnchors { get { return cockpitModeAnchors; } }

        // This is internal so it can be called for unit test setups
        internal void OnEnable()
        {
            controlButtons = new List<ControlButton>();
            cockpitModeAnchors = FindObjectsOfType<CockpitModeAnchor>(true).ToList();
        }

        /// <summary>
        /// Will place all buttons as soon as the necessary conditions are met. ControlButtonManager relies on certain objects in the scene to be loaded before controlButtons can be placed.
        /// </summary>
        /// <param name="loadedControlButtons"></param>
        /// <returns></returns>
        public IEnumerator PlaceWhenReady(SavedControlButton[] loadedControlButtons)
        {
            //while (!ready)
            //{
            //    yield return new WaitForSeconds(1f);
            //}

            PlaceAll(loadedControlButtons);
            yield return null;
            controlButtonsLoaded.Raise();
        }

        /// <summary>
        /// Places all controlButtons in the scene based on the settings in the SavedState
        /// </summary>
        /// <param name="state"></param>
        private void PlaceAll(SavedControlButton[] loadedControlButtons)
        {
            for (var i = 0; i < loadedControlButtons.Length; i++)
            {
                PlaceSavedControlButton(loadedControlButtons[i]);
            }
        }

        /// <summary>
        /// Place a controlButton prefab in the scene based on the SavedControlButton
        /// </summary>
        /// <param name="buttonToPlace"></param>
        public void PlaceSavedControlButton(SavedControlButton buttonToPlace)
        {
            // Use the asset type to instantiate a new controlButton
            var _type = buttonToPlace.type;
            var controlButtonAsset = controlButtonCatalog.GetByName(_type);
            EDStatusFlags anchorStatusFlag = Utils.EnumUtils.ParseEnumOrDefault<EDStatusFlags>(buttonToPlace.anchorStatusFlag);
            EDGuiFocus anchorGuiFocus =  Utils.EnumUtils.ParseEnumOrDefault<EDGuiFocus>(buttonToPlace.anchorGuiFocus);

            ControlButton _button = InstantiateControlButton(controlButtonAsset, anchorGuiFocus, anchorStatusFlag);

            // Place it based on the loaded state settings
            _button.transform.localPosition = buttonToPlace.overlayTransform.pos;
            _button.transform.localEulerAngles = buttonToPlace.overlayTransform.rot;

            // Store a reference for getting the location later
            controlButtons.Add(_button);
        }


        /// <summary>
        /// Read the current state of each ControlButton, serialize them into a saveable state
        /// </summary>
        /// <returns>Array of SavedControlButtons</returns>
        public SavedControlButton[] GetCurrentStates()
        {
            var serializedResult = new List<SavedControlButton>();
            foreach (ControlButton button in controlButtons)
            {
                serializedResult.Add(new SavedControlButton
                {
                    type = button.controlButtonAsset.name,
                    anchorGuiFocus = button.configuredGuiFocus.ToString(),
                    anchorStatusFlag = button.configuredStatusFlag.ToString(),
                    overlayTransform = new OverlayTransform()
                    {
                        pos = button.transform.localPosition,
                        rot = button.transform.localEulerAngles
                    }
                });
            }
            return serializedResult.ToArray();
        }

        /// <summary>
        /// Add a new control button into the scene
        /// </summary>
        /// <param name="controlButtonAsset"></param>
        /// <returns>newly instantiated ControlButton</returns>
        public ControlButton InstantiateControlButton(ControlButtonAsset controlButtonAsset, EDGuiFocus anchorGuiFocus, EDStatusFlags anchorStatusFlag)
        {
            var prefab = controlButtonCatalog.controlButtonPrefab;
            prefab.SetActive(false);
            var controlButtonInstance = Instantiate(prefab);
            var controlButton = controlButtonInstance.GetComponent<ControlButton>();
            controlButtonInstance.name = controlButtonAsset.name;
            controlButton.label = controlButtonAsset.GetLabelText();
            controlButton.controlButtonAsset = controlButtonAsset;
            controlButton.configuredStatusFlag = anchorStatusFlag;
            controlButton.configuredGuiFocus = anchorGuiFocus;

            var matchingAnchor = cockpitModeAnchors
                .Where(anchor => anchor.activationGuiFocus == anchorGuiFocus)
                .Where(anchor => anchor.activationStatusFlag == anchorStatusFlag)
                .FirstOrDefault();

            if (matchingAnchor == null)
            {
                matchingAnchor = CreateCockpitModeAnchor(anchorGuiFocus, anchorStatusFlag);
            }

            matchingAnchor.AddControlButton(controlButton);

            controlButtonInstance.SetActive(true);
            return controlButton;
        }


        /// <summary>
        /// Create a new CockpitModeAnchor and set it as a child of the parent object that's defined in the ControlButtonManager.
        /// </summary>
        /// <param name="anchorGuiFocus"></param>
        /// <param name="anchorStatusFlag"></param>
        /// <returns></returns>
        public CockpitModeAnchor CreateCockpitModeAnchor(EDGuiFocus anchorGuiFocus, EDStatusFlags anchorStatusFlag)
        {
            // Create a new anchor and parent gameObject
            var anchorObject = Instantiate(CockpitAnchorPrefab);
            anchorObject.name = $"{anchorGuiFocus}|{anchorStatusFlag}";
            anchorObject.transform.SetParent(parentObject.transform, false);
            anchorObject.SetActive(true);

            var newAnchor = anchorObject.GetComponent<CockpitModeAnchor>();

            // Set it to match the required flags/guifocus
            newAnchor.activationGuiFocus = anchorGuiFocus;
            newAnchor.activationStatusFlag = anchorStatusFlag;
            newAnchor.OnEnable(); // force initialization

            cockpitModeAnchors.Add(newAnchor);

            return newAnchor;
        }


        public void Clear()
        {
            // Destroy all control Buttons in the scene
            foreach(ControlButton cb in controlButtons)
            {                
                Destroy(cb.gameObject);
            }
            controlButtons.Clear();
        }

    }
}
