using System.Collections.Generic;
using UnityEngine;

namespace EVRC.Core.Actions
{
    [CreateAssetMenu(fileName = "ControlButtonCatalog", menuName = Constants.CONTROL_BUTTON_PATH +"/ControlButtonAssetCatalog", order = 1)]
    public class ControlButtonAssetCatalog : ScriptableObject
    {
        public ControlButtonAsset[] controlButtons;
        public Dictionary<string, ControlButtonAsset> nameMap = new Dictionary<string, ControlButtonAsset>();
        public Texture defaultTexture;
        [Tooltip("The alt texture (off) - where applicable")] public Texture defaultOffTexture;

        private void OnEnable()
        {
            foreach (ControlButtonAsset controlButtonAsset in controlButtons)
            {
                // If a texture is missing, set the default texture(s) 
                if (controlButtonAsset.GetTexture() == null)
                {
                    SetDefaultTextures(controlButtonAsset);
                }

                // Add the ControlButtonAsset to the namemap
                nameMap.Add(controlButtonAsset.name, controlButtonAsset);
            }
        }

        private void SetDefaultTextures(ControlButtonAsset cba)
        {
            if (defaultTexture == null || defaultOffTexture == null) { Debug.LogError("default Texture(s) must be set"); return; }
            if (cba.GetType() == typeof(SimpleControlButtonAsset))
            {
                cba.SetTexture(defaultTexture);
            }
            else if (cba.GetType() == typeof(StatusFlagControlButtonAsset))
            {
                cba.SetTexture(defaultTexture, defaultOffTexture);
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
