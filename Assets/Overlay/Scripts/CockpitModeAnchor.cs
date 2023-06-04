using System;
using EVRC.Core.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace EVRC.Core.Overlay
{
    /// <summary>
    /// Identifies and activates a group of gameobjects (children) as related to a specific CockpitMode. You DO NOT need to create anchors unless there are specific  
    /// conditions that would make it difficult to generate the anchor with a script. Most things that are saved in the SavedState file will automatically generate 
    /// the relevant Anchor if it doesn't exist.
    /// </summary>
    /// <remarks>
    /// For example, when placing a controlButton, we need to know the root cockpit
    /// object, so we can place the controlButton as a child
    /// is active.
    /// </remarks>
    [RequireComponent(typeof(EDStatusAndGuiListener))]
    public class CockpitModeAnchor : MonoBehaviour
    {
        public CockpitMode cockpitUiMode = CockpitMode.Cockpit;
        [Tooltip("The single status flag that must be present for this Object to be active")] public EDStatusFlags activationStatusFlag;
        [Tooltip("Which GUI Focus must be present for this Object to be active. Null means any focus will work")] public EDGuiFocus activationGuiFocus;

        private EDStatusAndGuiListener listener;
        // These objects will be activated/deactivated when the statusFlag or GuiFocus changes
        private List<GameObject> targets;
        public List<GameObject> TargetList { get { return targets; } }
        

        // This is internal so that the test assembly can call it during unit tests
        internal void OnEnable()
        {
            listener = GetComponent<EDStatusAndGuiListener>();

            if (targets == null || targets.Count == 0)
            {
                AddImmediateChildrenToList();
            }
        }

        public void AddControlButton(ControlButton controlButton)
        {
            targets.Add(controlButton.gameObject);
            controlButton.gameObject.transform.SetParent(transform, false);
        }

        private void AddImmediateChildrenToList()
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
