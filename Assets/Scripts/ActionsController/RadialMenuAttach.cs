using EVRC;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// Upon Activation, this script sets the configuration for the Radial Menu by passing a configuration to the RadialMenuController. 
/// This script is an IActivatable, so it will be detected by the ControllerInteractionPoint
/// </summary>
///  <remarks>
///   <para>
///     This script can configure between 2 and 6 actions. Each action has an icon, a text label, and a UnityEvent
///     which will be used to generate the corresponding number of RadialAction prefabs as children of the RadialMenuController.
///   </para>
///</remarks>
public class RadialMenuAttach : MonoBehaviour, IActivateable
{
    /* Attach this script to a GameObject to configure the RadialMenu that will appear
     * when the user interacts with th
     */

    [Serializable]
    public struct RadialActionFields
    {
        // required fields to pass to the RadialMenuController
        [SerializeField] public Texture icon;
        [SerializeField] public string label;
        [SerializeField] public UnityEvent onPress;              
    }

    [Header("Settings for Radial Menu")]
    public string radialName = "Radial Menu Name";
    public int ActionCount
    {
        get { return actionCount; }
        set { 
            actionCount = Mathf.Clamp(value, 2, 6);
        }
    }

    [SerializeField, Tooltip("How many actions should the radial menu have?")]
    private int actionCount = 2;
    private int oldCount = 2;
    public float actionBoundaryAngle;
    [SerializeField]
    public List<RadialActionFields> menuActions = new List<RadialActionFields>();

    public void OnValidate()
    {
        // If the number of actions has been adjusted, add/remove as necessary
        if (actionCount != oldCount)
        {
            int actionsToAdd = 0;
            // Figure out the amount of change necessary
            if (menuActions.Count == 0)
            {
                menuActions = new List<RadialActionFields>();
                actionsToAdd = actionCount;
            }
            else
            {
                actionsToAdd = actionCount - menuActions.Count;
            }

            // Add or Remove elements
            if (actionsToAdd > 0)
            {
                for (int i = 0; i < actionsToAdd; i++)
                {
                    RadialActionFields a = new RadialActionFields();
                    menuActions.Add(a);
                }
            }
            else
            {
                var old = menuActions.GetRange(0, actionCount);
                menuActions = old;
            }
        }
        oldCount = actionCount;
        SetWedgeAngle();
    }

    private void SetWedgeAngle()
    {
        // the bottom 140 degree "wedge" is reserved to close the radial
        actionBoundaryAngle = 220.0f / actionCount;
    }

    public void SetControllerVariables(RadialMenuController controller)
    {
        controller.actionAngle = actionBoundaryAngle;
        controller.actionCount = actionCount;
        controller.ClearRadialActions();
        controller.CreateActions(menuActions);
    }

    public Action Activate(ControllerInteractionPoint interactionPoint)
    {
        RadialMenuController controller = interactionPoint.radialMenu;
        if (controller.gameObject.activeSelf == true)
        {
            controller.gameObject.SetActive(false);
        }
        SetControllerVariables(controller);
        controller.gameObject.SetActive(true);
        return () => { };
    }
}
