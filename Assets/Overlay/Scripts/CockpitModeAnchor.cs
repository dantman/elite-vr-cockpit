using System;
using EVRC.Core.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI.Extensions;

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
    public class CockpitModeAnchor : MonoBehaviour
    {
        [Tooltip("The single status flag that must be present for this Object to be active")] public EDStatusFlags activationStatusFlag;
        [Description("The default value: 'Panel Or No Focus' will make this anchor stay active if the guiFocus is any of the 'panels' (internal, comms, etc.)")]
        [Tooltip("Which GUI Focus must be present for this Object to be active.")] public EDGuiFocus activationGuiFocus = EDGuiFocus.PanelOrNoFocus;
        [SerializeField] internal EliteDangerousState eliteDangerousState;

        // These objects will be activated/deactivated when the statusFlag or GuiFocus changes
        [SerializeField] private List<GameObject> targets;
        public List<GameObject> TargetList { get { return targets; } }
        

        // This is internal so that the test assembly can call it during unit tests
        internal void OnEnable()
        {
            if (targets == null || targets.Count == 0)
            {
                AddImmediateChildrenToList();
            }
        }

        internal void OnDisable()
        {
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


        private void Refresh()
        {
            OnGuiFocusChanged(eliteDangerousState.guiFocus);
        }

        private bool ShouldStatusFlagActivate(EDStatusFlags statusFlags)
        {
            return activationStatusFlag == default(EDStatusFlags) ? false : statusFlags.HasFlag(activationStatusFlag);
        }

        private bool ShouldGuiFocusActivate(EDGuiFocus guiFocus)
        {
            if (activationGuiFocus == EDGuiFocus.PanelOrNoFocus && (int)guiFocus <= 4)
            {
                return true;
            }
            return guiFocus == activationGuiFocus;
        }


        public void OnEDStatusFlagsChanged(EDStatusFlags newStatusFlags)
        {
            Debug.Log($"On OnEDStatusFlagsChanged evaluating: {newStatusFlags}");
            // GuiFocus values above 4 are "mode" types, so status flag changes won't affect the UI
            if ((int)eliteDangerousState.guiFocus > 4) return;

            var a = ShouldStatusFlagActivate(newStatusFlags);
            var b = ShouldGuiFocusActivate(eliteDangerousState.guiFocus);
            ActivateTargets(a && b);
        }

        public void OnGuiFocusChanged(EDGuiFocus newFocus) 
        {
            Debug.Log($"On OnGuiFocusChanged evaluating: {newFocus}");
            // GuiFocus values above 4 are "mode" types, so status flag changes won't affect the UI
            if ((int)newFocus > 4)
            {
                ActivateTargets(ShouldGuiFocusActivate(newFocus));
                return;
            }
            ActivateTargets(ShouldStatusFlagActivate(eliteDangerousState.statusFlags) && ShouldGuiFocusActivate(newFocus));
        }
        

        private void ActivateTargets(bool shouldActivate)
        {
            foreach (var target in targets)
                target.SetActive(shouldActivate);
        }

        #region --------------Event Listener Methods-----------------
        public void OnMenuModeActive(bool menuModeActive)
        {
            // inverted from others b/c we want anchor targets disabled when MenuMode is on
            Activate(!menuModeActive);
        }

        public void OnGameStart(bool started)
        {
            Activate(started);
        }

        public void OnOpenVRStart(bool started)
        {
            Activate(started);
        }

        private void Activate(bool activate)
        {
            if (activate)
            {
                Refresh();
                return;
            }
            ActivateTargets(false);
            
        }
        #endregion
    }
}
