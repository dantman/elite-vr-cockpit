using System;
using System.Collections.Generic;
using EVRC.Core.Actions;
using UnityEngine;

namespace EVRC.Core.Overlay
{
    public class ControlButtonManager : MonoBehaviour
    {
        public ControlButtonAssetCatalog controlButtonCatalog;
        
        public List<CategoryCockpitModePair> cockpitModeMappings;

        [Header("New Button Spawn Settings")]
        public static Vector3 spawnZoneStart;

        protected static Dictionary<ControlButtonAsset.ButtonCategory, GameObject> rootMap;
        private readonly List<ControlButton> controlButtons;
        private CockpitModeAnchor[] cockpitModeAnchors;

        private void OnEnable()
        {
            rootMap = new Dictionary<ControlButtonAsset.ButtonCategory, GameObject>();
            cockpitModeMappings = new List<CategoryCockpitModePair>();

            cockpitModeAnchors = FindObjectsOfType<CockpitModeAnchor>(true);


            //Create a mapping for controlButton placement (parent objects) for each category of button
            foreach (var anchor in cockpitModeAnchors)
            {
                ControlButtonAsset.ButtonCategory btnCat = ControlButtonUtils.GetButtonCategoryFromCockpitMode(anchor.cockpitUiMode);
                rootMap.Add(btnCat,anchor.target);

                // Just so we can see it in the inspector..
                cockpitModeMappings.Add(new CategoryCockpitModePair()
                {
                    root = anchor.target,
                    category = btnCat
                });
            }
        }

        /// <summary>
        /// Places all controlButtons in the scene based on the settings in the OverlayState
        /// </summary>
        /// <param name="state"></param>
        public void PlaceAll(OverlayState state)
        {
            for (var i = 0; i < state.controlButtons.Length; i++)
            {
                // Use the asset type to instantiate a new controlButton
                var _type = state.controlButtons[i].type;
                var controlButtonAsset = controlButtonCatalog.GetByName(_type);
                ControlButton _button = InstantiateControlButton(controlButtonAsset);

                // Place it based on the loaded state settings
                _button.transform.localPosition = state.controlButtons[i].overlayTransform.pos;
                _button.transform.localEulerAngles = state.controlButtons[i].overlayTransform.rot;

                // Store a reference for getting the location later
                controlButtons.Add(_button);
            }
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


        #region ---------------New Button Spawner-------------

        /// <summary>
        /// Add a new controlButton to the scene. For loading controlButtons from a file, use PlaceAll in ControlButtonManager
        /// </summary>
        /// <param name="controlButtonAsset"></param>
        public void AddNewControlButton(ControlButtonAsset controlButtonAsset)
        {
            ControlButton _button = InstantiateControlButton(controlButtonAsset);
            Spawn(_button);
        }

        /// <summary>
        /// Places a newly created controlButton in the designated zone
        /// </summary>
        /// <param name="button"></param>
        private static void Spawn(ControlButton button)
        {
            button.transform.localPosition = OverlayUtils.GetSpawnLocation(spawnZoneStart, button.gameObject);
        }

        #endregion
    }
}
