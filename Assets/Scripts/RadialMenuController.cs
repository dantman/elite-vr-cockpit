using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class RadialMenuController : MonoBehaviour
{
    
    public enum MenuMode
    {
        Hold,
        Toggle
    }

    [Header("Global Radial Menu Settings")]
    public Transform activeWedgeTransform = null;

    public SteamVR_Action_Vector2 selectorPosition = null;
    public SteamVR_Action_Boolean wedgeAction = null;
    private Vector2 touchPosition = Vector2.zero;
    private RadialWedge activeWedge = null;

    [Header("Configured by RadialMenuAttach Script")]
    public Texture wedgeTexture;
    public float wedgeAngle;
    public int _wedgeCount = 0;
    public List<RadialWedge> radialWedges = null;
    //[Tooltip("Hold to keep the menu open? Or Toggle on/off")]
    //public MenuMode menuMode = MenuMode.Hold;
    //public int WedgeCount
    //{
    //    get { return _wedgeCount; }
    //    set { _wedgeCount = Mathf.Clamp(value,2,6); }
    //}
    //[SerializeField, Tooltip("How many actions (slices) should the radial menu have?"), Range(2,6)]
    //private int _wedgeCount = 2;

    //[Header("Actions"), Tooltip("Wedges are numbered starting with the top and going clockwise")]
    //public RadialWedge wedgeOne;
    //public RadialWedge wedgeTwo;
    ////public RadialWedge wedgeThree;
    ////public RadialWedge wedgeFour;
    ////public RadialWedge wedgeFive;
    ////public RadialWedge wedgeSix;
   

    private void Awake()
    {
        selectorPosition.onAxis += SetPosition;
        wedgeAction.onStateUp += InvokeWedgeAction;
    }

    private void OnDestroy()
    {
        selectorPosition.onAxis -= SetPosition;
        wedgeAction.onStateUp -= InvokeWedgeAction;        
    }

    public void OnEnable()
    {
        if (_wedgeCount == 0) { Debug.LogError("No Actions have been configured"); return; }
        wedgeAngle = 360.0f / _wedgeCount;
    }

    public void OnDisable()
    {
        radialWedges.Clear();
    }

    private void Update()
    {
        if (radialWedges.Count == 0)
        {
            Debug.LogError("RadialMenu has not been configured with Actions. You must use a RadialMenuAttach script to activate and deactivate the Menu.");
        }
        Vector2 direction = Vector2.zero + touchPosition;
        if (direction == Vector2.zero)
        {
            activeWedge = null;
            return;
        };

        float angle = GetDegreeFromVector(direction);

        SetSelection(angle);
    }

    private float GetDegreeFromVector(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.x, direction.y);
        angle = angle * Mathf.Rad2Deg;

        if (angle < 0)
        {
            angle += 360.0f;
        }
        //Debug.LogWarning($"fixed angle: {angle}");
        return angle;
    }

    private int GetIndexFromAngle(float angle)
    {
        return Mathf.RoundToInt(angle / wedgeAngle);
    }

    private void SetSelection(float angle)
    {
        int index = GetIndexFromAngle(angle);
        if (index == _wedgeCount)
        {
            index = 0;
        }
        //Debug.LogWarning($"wedge index: {index}");
        activeWedge = radialWedges[index];

        float snappedAngle = index * wedgeAngle;
        //Debug.LogWarning($"snapped angle: {snappedAngle}");
        activeWedgeTransform.localEulerAngles = new Vector3(0, 0, -snappedAngle);
    }

    public void AddRadialActions(List<RadialWedge> actions)
    {
        radialWedges.AddRange(actions);
    }

    public void ClearRadialActions()
    {
        radialWedges.Clear();
    }

    private void SetPosition(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
    {
        touchPosition = axis;
        //Debug.LogWarning($"angle of joystick {touchPosition}");
    }

    private void InvokeWedgeAction(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        activeWedge.onPress.Invoke();
    }

}
