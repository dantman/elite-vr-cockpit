using EVRC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Valve.VR;

public class RadialMenuAttach : MonoBehaviour, IActivateable
{
    /* Attach this script to a GameObject to configure the RadialMenu that will appear
     * when the user interacts with th
     */

    [Header("Settings for Radial Menu")]
    public string radialName = "Radial Menu Name";
    public int ActionCount
    {
        get { return actionCount; }
        set { 
            actionCount = Mathf.Clamp(value, 2, 6);
        }
    }
    [SerializeField, Tooltip("How many actions (slices) should the radial menu have?")]
    private int actionCount = 2;
    private int oldCount = 2;
    public Texture wedgeTexture;
    public float wedgeAngle;

    [Header("Actions")]
    public List<RadialWedge> menuActions = null;

    public void Awake() 
    {
        foreach (var wedge in menuActions)
        {
            if (wedge.label != RadialWedge.defaultLabel)
            {
                wedge.createLabelTexture();
            }
        }
    }

    public void OnValidate()
    {
        // If the number of actions has been adjusted, add/remove as necessary
        if (actionCount != oldCount)
        {
            int actionsToAdd = 0;
            // Figure out the amount of change necessary
            if (menuActions.Count == 0)
            {
                menuActions = new List<RadialWedge>();
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
                    RadialWedge w = new RadialWedge();
                    menuActions.Add(w);

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


       foreach(var wedge in menuActions)
        {
            if (wedge.label != RadialWedge.defaultLabel)
            {
                wedge.createLabelTexture();
            }
        }
    }

    private void SetWedgeAngle()
    {
        wedgeAngle = 360.0f / actionCount;
    }

    public void SetControllerVariables(RadialMenuController controller)
    {
        //RadialMenuController controller = GetComponent<RadialMenuController>();
        controller.wedgeTexture = wedgeTexture;
        controller.wedgeAngle = wedgeAngle;
        controller._wedgeCount = actionCount;
        controller.ClearRadialActions();
        controller.AddRadialActions(menuActions);
    }

    public Action Activate(ControllerInteractionPoint interactionPoint)
    {
        RadialMenuController controller = interactionPoint.radialMenu;
        SetControllerVariables(controller);
        controller.gameObject.SetActive(true);
        return () => { };
    }
}
