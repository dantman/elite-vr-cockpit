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
        [Tooltip("Which GUI Focus must be present for this Object to be active. Null means any focus will work")] public EDGuiFocus activationGuiFocus;

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
            // the default(Enum) convention means nothing has been selected in the inspector, so they are (effectively) null
            if (activationGuiFocus == default(EDGuiFocus) && activationStatusFlag == default(EDStatusFlags)) return;

            // evaluate guifocus first b/c they are more specific than status flags
            if (activationGuiFocus != default(EDGuiFocus))
            {
                ActivateTargets(newGuiFocus == activationGuiFocus);
                // we only evaluate the status flag if the guifocus is null
                return;
            }

            // These 5 GuiFocus values are the ones that indicate a special view in which other modes should be deactivated
            // FSS Mode, for example, removes the traditional cockpit view in the game. So the player can't see their joystick, throttle, or any information panels.
            EDGuiFocus[] guiAnchors = new EDGuiFocus[5] {EDGuiFocus.GalaxyMap, EDGuiFocus.FSSMode, EDGuiFocus.SystemMap, EDGuiFocus.StationServices, EDGuiFocus.Orrery};

            // If a guifocus wasn't defined (checked above) AND it's one of the special ones, the targets will always be deactivated
            if (guiAnchors.Contains(newGuiFocus))                
            {
                ActivateTargets(false);
                return;
            }


            //Only after evaluating the guiFocus, check status flags for match
            if (activationStatusFlag != default(EDStatusFlags))
            {
                // Activate if the activation flag is present
                ActivateTargets(newStatusFlags.HasFlag(activationStatusFlag));
            }
        }

        private void ActivateTargets(bool shouldActivate) 
        {
            foreach (var target in targets)
                target.SetActive(shouldActivate);
        }
    }
}
