using System;
using UnityEngine;

namespace EVRC
{
    [CreateAssetMenu(fileName = "ControlButtonCatalog", menuName = "EVRC/ControlButtonAssets/ControlButtonAssetCatalog", order = 9999)]
    public class ControlButtonAssetCatalog : ScriptableObject
    {
        public ControlButtonAsset[] controlButtons;
    }
}
