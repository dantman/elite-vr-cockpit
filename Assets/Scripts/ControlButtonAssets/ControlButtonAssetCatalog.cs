using System;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC
{
    [CreateAssetMenu(fileName = "ControlButtonCatalog", menuName = "EVRC/ControlButtonAssets/ControlButtonAssetCatalog", order = 9999)]
    public class ControlButtonAssetCatalog : ScriptableObject
    {
        public ControlButtonAsset[] controlButtons;
        public Dictionary<string, ControlButtonAsset> nameMap = new Dictionary<string, ControlButtonAsset>();

        private void OnEnable()
        {
            foreach (var controlButton in controlButtons)
            {
                nameMap.Add(controlButton.name, controlButton);
            }
        }

        /**
         * Get a control button asset by its asset name
         */
        public ControlButtonAsset GetByName(string name)
        {
            return nameMap[name];
        }
    }
}
