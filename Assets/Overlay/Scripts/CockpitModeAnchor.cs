using System;
using EVRC.Core.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace EVRC.Core.Overlay
{
    /// <summary>
    /// Identifies a GameObject as related to a specific CockpitMode. 
    /// </summary>
    /// <remarks>
    /// For example, when placing a controlButton, we need to know the root cockpit
    /// object, so we can place it as a child, so that it appears when the correct CockpitUI
    /// is active.
    /// </remarks>
    public class CockpitModeAnchor : MonoBehaviour
    {
        public CockpitMode cockpitUiMode = CockpitMode.Cockpit;
        [Tooltip("The single status flag that must be present for this Object to be active")] public EDStatusFlags activationStatusFlag;
        [Tooltip("Which GUI Focuses must be present for this Object to be active")] public EDGuiFocus[] activationGuiFocuses;

        [Tooltip("The object that contains the child objects for a cockpitMode"), SerializeField]
        private List<GameObject> targets;

        private void OnEnable()
        {
            if (targets == null || targets.Count == 0)
            {
                AddImmediateChildrenToList();
            }
        }

        public void AddControlButton(ControlButton controlButton)
        {
            targets.Add(controlButton.gameObject);
            controlButton.gameObject.transform.SetParent(transform.parent, false);
        }

        public void AddImmediateChildrenToList()
        {
            targets = new List<GameObject>();

            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                targets.Add(child);
            }
        }

        public void OnEDStatusAndGuiChanged(EDStatusFlags newStatusFlags, EDGuiFocus newGuiFocus)
        {
            Debug.Log($"On EDStatusAndGuiChanged activated with: {newStatusFlags} | {newGuiFocus}");

            bool shouldActivate = newStatusFlags.HasFlag(activationStatusFlag) && activationGuiFocuses.Contains(newGuiFocus);

            // activate all targets (children) if the status has a matching flag and the GUI focus list contains the current value.
            foreach (var target in targets)
                target.SetActive(shouldActivate);
        }
    }
}
