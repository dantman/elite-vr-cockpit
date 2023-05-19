using System;
using System.Collections;
using System.Collections.Generic;
using EVRC.Core.Actions;
using UnityEngine;

namespace EVRC.Core.Overlay
{
    public class ControlButtonManager : MonoBehaviour
    {
        public ControlButtonAssetCatalog controlButtonCatalog;
        public GameEvent controlButtonsLoaded;
        public List<CategoryCockpitModePair> cockpitModeMappings;

        [Header("New Button Spawn Settings")]
        public static Vector3 spawnZoneStart = new Vector3(0,0.9f,0.5f);

        protected static Dictionary<ButtonCategory, GameObject> rootMap;
        private List<ControlButton> controlButtons;
        private CockpitModeAnchor[] cockpitModeAnchors;

        private bool ready = false;

        private void OnEnable()
        {
            controlButtons = new List<ControlButton>();
            rootMap = new Dictionary<ButtonCategory, GameObject>();
            cockpitModeMappings = new List<CategoryCockpitModePair>();

            StartCoroutine(SetRootMappings());
        }

        /// <summary>
        /// Use the CockpitModeAnchors from the scene to map button categories to parent gameObjects. Buttons will be placed inside their parent gameObjects, if a match is found.
        /// </summary>
        /// <remarks>This can be kinda slow, so other methods are waiting for this to complete</remarks>
        /// <returns></returns>
        private IEnumerator SetRootMappings()
        {
            cockpitModeAnchors = FindObjectsOfType<CockpitModeAnchor>(true);

            //Create a mapping for controlButton placement (parent objects) for each category of button
            foreach (var anchor in cockpitModeAnchors)
            {
                ButtonCategory btnCat = ControlButtonUtils.GetButtonCategoryFromCockpitMode(anchor.cockpitUiMode);
                rootMap.Add(btnCat, anchor.target);

                // Just so we can see it in the inspector..
                cockpitModeMappings.Add(new CategoryCockpitModePair()
                {
                    root = anchor.target,
                    category = btnCat
                });
            }

            yield return null;
            ready = true;
        }


        /// <summary>
        /// Will place all buttons as soon as the necessary conditions are met. ControlButtonManager relies on certain objects in the scene to be loaded before controlButtons can be placed.
        /// </summary>
        /// <param name="loadedControlButtons"></param>
        /// <returns></returns>
        public IEnumerator PlaceWhenReady(SavedControlButton[] loadedControlButtons)
        {
            while (!ready)
            {
                yield return new WaitForSeconds(1f);
            }

            PlaceAll(loadedControlButtons);
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
            ControlButton _button = InstantiateControlButton(controlButtonAsset);

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
        public ControlButton InstantiateControlButton(ControlButtonAsset controlButtonAsset)
        {
            if (!rootMap.ContainsKey(controlButtonAsset.category))
            {
                Debug.LogErrorFormat("ControlButtonManager ({0}) does not contain mapping for the {1} category", controlButtonAsset.name, controlButtonAsset.category.ToString());
                return null;
            }

            var prefab = controlButtonCatalog.controlButtonPrefab;
            prefab.SetActive(false);
            var controlButtonInstance = Instantiate(prefab);
            var controlButton = controlButtonInstance.GetComponent<ControlButton>();
            controlButtonInstance.name = controlButtonAsset.name;
            controlButton.label = controlButtonAsset.GetLabelText();
            controlButton.controlButtonAsset = controlButtonAsset;


            controlButton.transform.SetParent(rootMap[controlButtonAsset.category].transform, false);

            controlButtonInstance.SetActive(true);
            return controlButton;
        }

    }
}
