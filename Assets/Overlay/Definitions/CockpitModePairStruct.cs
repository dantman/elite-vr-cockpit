using System;
using EVRC.Core.Actions;
using UnityEngine;

namespace EVRC.Core.Overlay
{
    /// <summary>
    /// Connect button categories to Cockpit GameObjects, so they will activate/deactivate with other similar objects
    /// </summary>
    [Serializable] public struct CategoryCockpitModePair
    {
        public ControlButtonAsset.ButtonCategory category;
        public GameObject root;
    }
}