using System;
using System.Collections.Generic;
using EVRC.Core.Overlay;
using UnityEngine;

namespace EVRC.Core.Actions
{
    public class ControlButtonManager : MonoBehaviour
    {
        [Serializable]
        public struct CategoryRootPair
        {
            public ControlButtonAsset.ButtonCategory category;
            public GameObject root;
        }

        public GameObject controlButtonPrefab;
        public CategoryRootPair[] rootMappings;
        protected Dictionary<ControlButtonAsset.ButtonCategory, GameObject> rootMap;

        public static ControlButtonManager _instance;
        public static ControlButtonManager instance
        {
            get
            {
                return OverlayUtils.Singleton(ref _instance, "[ControlButtonManager]", false);
            }
        }

        private void OnEnable()
        {
            rootMap = new Dictionary<ControlButtonAsset.ButtonCategory, GameObject>();
            foreach (var pair in rootMappings)
            {
                rootMap.Add(pair.category, pair.root);
            }
        }

        /**
         * Add a new control button into the scene
         */
        public ControlButton AddControlButton(ControlButtonAsset controlButtonAsset)
        {
            if (!rootMap.ContainsKey(controlButtonAsset.category))
            {
                Debug.LogErrorFormat("ControlButtonManager ({0}) does not contain mapping for the {1} category", name, controlButtonAsset.category.ToString());
                return null;
            }

            controlButtonPrefab.SetActive(false);
            var controlButtonInstance = Instantiate(controlButtonPrefab);
            var controlButton = controlButtonInstance.GetComponent<ControlButton>();
            controlButtonInstance.name = controlButtonAsset.name;
            controlButton.label = controlButtonAsset.GetLabelText();
            controlButton.controlButtonAsset = controlButtonAsset;

            //HolographicOverlay overlay = controlButtonInstance.GetComponent<HolographicOverlay>();
            //Texture tex = controlButtonAsset.GetTexture();
            //if (tex == null)
            //{

            //}


            controlButton.transform.SetParent(rootMap[controlButtonAsset.category].transform, false);

            controlButtonInstance.SetActive(true);
            return controlButton;
        }
    }
}
