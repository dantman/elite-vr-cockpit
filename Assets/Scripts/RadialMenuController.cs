using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Valve.VR;
using static RadialMenuAttach;
using static Valve.VR.SteamVR_Events;

namespace EVRC
{
    using Events = SteamVR_Events;    
    /// <summary>
    /// Manages the selection and activation of Actions for the RadialMenu, which is attached to the player's hand.
    /// </summary>
    ///  <remarks>
    ///   <para>
    ///     Requires another gameobject with the RadialMenuAttach script.
    ///   </para>
    ///    <para>
    ///        When the user interacts with a component that contains the RadialMenuAttach script, the ControllerInteractionPoint from the tracked
    ///        hand will Activate this MenuController and then pass the Actions, Labels, and Icons for this component to implement. 
    ///    </para>
    ///    <para>
    ///        This approach will allow flexible configurations to be used in different contexts. For example, a different number of buttons
    ///        can be supplied without disrupting the placement of the icons on the menu.
    ///    </para>
    ///</remarks>
    public class RadialMenuController : MonoBehaviour
    {
        [Header("Global Radial Menu Settings")]
        public GameObject radialActionPrefab = null;
        public RadialAction closeRadialAction = null;
        public float menuSize = 0.05f;
        public float iconSpread = 0.05f;
        public Color iconColor = Color.white;
        public Color iconHighlightColor = Color.red;
        public bool useHudColorMatrix = true;
        public float deadzone = 0.1f;


        public static SteamVR_Events.Event<int> highlightedActionChanged = new SteamVR_Events.Event<int>();
        public SteamVR_Action_Vector2 selectorPosition = null;
        public SteamVR_Action_Boolean select = null;
        private Vector2 touchPosition;
        private RadialAction highlightedAction = null;
        private readonly static float bottomActionAngle = 140.0f;
        private List<float> indexFinder;
        private float angleFromZero = 180 - (bottomActionAngle / 2);


        [Header("DEBUG: Configured by RadialMenuAttach Script")]
        public float actionAngle = 0; //the angle between sections of the radial menu
        public int actionCount = 0;
        private List<RadialAction> actionRefs = new List<RadialAction>();



        private void Awake()
        {
            selectorPosition.onAxis += SetPosition;
            select.onStateUp += InvokeWedgeAction;

            // the "close" action is always straight down 
            closeRadialAction.transform.Translate(0, -iconSpread, 0);

        }

        private void OnDestroy()
        {
            selectorPosition.onAxis -= SetPosition;
            select.onStateUp -= InvokeWedgeAction;
        }

        public void OnEnable()
        {
            if (actionCount == 0) { Debug.LogError("No Actions have been configured"); return; }
            
            touchPosition = Vector2.zero;
            // A list of boundary angles that is used to identify which action the user is pointing at (joystick/trackpad)
            indexFinder = new List<float>();
            for (int i = 0; i < actionCount; i++)
            {
                indexFinder.Add(-angleFromZero + (i * actionAngle));
            }

            iconColor = EDStateManager.ConditionallyApplyHudColorMatrix(useHudColorMatrix, iconColor);
            iconHighlightColor = EDStateManager.ConditionallyApplyHudColorMatrix(useHudColorMatrix, iconHighlightColor);
        }

        public void OnDisable()
        {
            RemoveActionPrefabs();
            indexFinder.Clear();
            actionAngle = 0; 
            actionCount = 0;
            highlightedAction = null;
    }

        private void RemoveActionPrefabs()
        {
            foreach (RadialAction ra in actionRefs)
            {
                Destroy(ra.gameObject);
            }
            actionRefs.Clear();
        }

        private void Update()
        {
            if (actionRefs.Count == 0)
            {
                Debug.LogError("RadialMenu has not been configured with Actions. You must use a RadialMenuAttach script to activate and deactivate the Menu.");
            }

            Vector2 direction = Vector2.zero + touchPosition;
            if (touchPosition == Vector2.zero)
            {
                // Send the event ONCE
                if (highlightedAction != null)
                {
                    highlightedAction = null;
                    highlightedActionChanged.Send(0);
                    Debug.LogWarning($"highlighted action: NONE");
                }
                return;
            };

            float angle = GetDegreeFromVector(direction);
            
            SetSelection(angle);
        }

        private float GetDegreeFromVector(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.x, direction.y);
            angle = angle * Mathf.Rad2Deg;
            return angle;
        }

        private int GetIndexFromAngle(float angle)
        {
            int index = indexFinder.FindLastIndex(boundary => angle > boundary);
            return index;
        }


        private void SetSelection(float angle)
        {
            if (angle <= -angleFromZero || angle >= angleFromZero)
            {
                SetHighlightedAction(closeRadialAction);               
                return;
            }

            int index = GetIndexFromAngle(angle);

            SetHighlightedAction(actionRefs[index]);
        }

        private void SetHighlightedAction(RadialAction action)
        {
            if (highlightedAction == action) { return; }
            highlightedAction = action;
            highlightedActionChanged.Send(action.GetInstanceID());
            Debug.LogWarning($"highlighted action: {highlightedAction.name}");
        }

        /// <summary>
        ///     Creates RadialAction Prefabs as children of the RadialMenuController using the supplied fields
        /// </summary>
        ///     <remarks>
        ///         <para>
        ///             Placement of the RadialActions is based on two factors: 
        ///             1) the amount of space allocated to the "bottom action" (default 140). 
        ///             2) the number of actions that are configured
        ///             
        ///             Actions are configured from left to right (clockwise).
        ///         </para>
        ///         <para>
        ///             For example, a circle contains 360 degrees and the Radial has been configured to have two actions.
        ///             The bottom 140 degrees is reserved for the "Close this Radial" action, so the remaining 220 degrees
        ///             will be sliced in half for the 2 actions (110 degrees each). 
        ///             In the example above, imagine a clock face with 0 degrees being midnight. Action 1 will be in from 
        ///             -110 to 0 degrees (generally up and left). Action 2 will be from 0 to 110 degrees (generally up and right)
        ///             "Close" will be from 110.1 to 250 degrees (down).
        ///         </para>
        ///     </remarks>
        public void CreateActions(List<RadialActionFields> newActions)
        {
            float iconAngle;
            float angleCoefficient;

            for (int i = 0; i < newActions.Count; i++)
            {
                GameObject clone = Instantiate(radialActionPrefab, transform);
                actionRefs.Add(clone.GetComponent<RadialAction>());

                // Icons are placed in the middle of the "wedge" for their corresponding action.
                angleCoefficient = ((newActions.Count - 1.0f) / -2.0f) + (i * 1.0f);
                iconAngle = angleCoefficient * actionAngle;

                RadialAction cloneAction = clone.GetComponent<RadialAction>();
                cloneAction.icon = newActions[i].icon;
                cloneAction.label = newActions[i].label;
                cloneAction.onPress = newActions[i].onPress;
                cloneAction.baseColor = iconColor;
                cloneAction.higlightColor = iconHighlightColor;
                cloneAction.iconObject.width = menuSize / 4;
                cloneAction.labelObject.size = menuSize / 2;

                clone.transform.Translate(Quaternion.Euler(0, 0, -iconAngle)* (Vector3.up * iconSpread));
                clone.name = $"RadialActionClone{i}";
            }
        }

        public void ClearRadialActions()
        {
            actionRefs.Clear();
        }
    
        private void SetPosition(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
        {
            // Reminder: this only fires when the axis is non-zero
            if (Mathf.Abs(axis.x) > deadzone || Mathf.Abs(axis.y) > deadzone)
            {   
                // NOT in deadzone, set position
                touchPosition = axis;
            }
        }

        private void InvokeWedgeAction(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            if (highlightedAction != null)
            {
                highlightedAction.onPress.Invoke();
                this.gameObject.SetActive(false);
            }
        }

    }
}
