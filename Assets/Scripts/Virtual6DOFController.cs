using UnityEngine;

namespace EVRC
{
    using StickAxis = VirtualJoystick.StickAxis;

    /**
     * A grabbable virtual control that offers 6-axis control of the ship based on the controller's movement
     */
    public class Virtual6DOFController : MonoBehaviour, IGrabable
    {
        public struct ThrusterAxis
        {
            public Vector3 Value { get; private set; }

            public static ThrusterAxis Zero
            {
                get
                {
                    return new ThrusterAxis(Vector3.zero);
                }
            }

            public ThrusterAxis(Vector3 axis)
            {
                Value = axis * 1f;
            }

            public ThrusterAxis(Vector3 axis, float maxDistance)
            {
                axis = axis / maxDistance;
                Value = new Vector3(
                    Mathf.Abs(axis.x) > 1f ? axis.x / Mathf.Abs(axis.x) : axis.x,
                    Mathf.Abs(axis.y) > 1f ? axis.y / Mathf.Abs(axis.y) : axis.y,
                    Mathf.Abs(axis.z) > 1f ? axis.z / Mathf.Abs(axis.z) : axis.z);
            }

            /**
             * Returns a new ThrusterAxis with a deadZone limit
             */
            public ThrusterAxis WithDeadzone(float deadZone)
            {
                return new ThrusterAxis(new Vector3(
                    Mathf.Abs(Value.x) < deadZone ? 0f : Value.x,
                    Mathf.Abs(Value.y) < deadZone ? 0f : Value.y,
                    Mathf.Abs(Value.z) < deadZone ? 0f : Value.z));
            }

            public override string ToString()
            {
                return Value.ToString();
            }
        }

        public float thrusterMaxDistance = 0.1f;
        public LineRenderer line;
        public SpriteRenderer marker;
        public GameObject verticalDisplay;
        public LineRenderer verticalLine;
        public SpriteRenderer rollMarker;
        public VirtualJoystickButtons buttons;
        public vJoyInterface output;
        protected CockpitStateController controller;
        private ControllerInteractionPoint attachedInteractionPoint;
        private Transform rotationZeroPoint;
        private Transform rotationPoint;
        private Transform translationZeroPoint;
        private Transform translationPoint;
        private bool upright = false;

        public GrabMode GetGrabMode()
        {
            return GrabMode.VirtualControl;
        }

        void Start()
        {
            controller = CockpitStateController.instance;

            var rotationZeroPointObject = new GameObject("[RotationZeroPoint]");
            rotationZeroPoint = rotationZeroPointObject.transform;
            rotationZeroPoint.SetParent(transform);
            rotationZeroPoint.localPosition = Vector3.zero;
            rotationZeroPoint.localRotation = Quaternion.identity;

            var rotationPointObject = new GameObject("[RotationPoint]");
            rotationPoint = rotationPointObject.transform;
            rotationPoint.SetParent(rotationZeroPoint);
            rotationPoint.localPosition = Vector3.zero;
            rotationPoint.localRotation = Quaternion.identity;

            var translationZeroPointObject = new GameObject("[TranslationZeroPoint]");
            translationZeroPoint = translationZeroPointObject.transform;
            translationZeroPoint.SetParent(transform);
            translationZeroPoint.localPosition = Vector3.zero;
            translationZeroPoint.localRotation = Quaternion.identity;

            var translationPointObject = new GameObject("[TranslationPoint]");
            translationPoint = translationPointObject.transform;
            translationPoint.SetParent(translationZeroPoint);
            translationPoint.localPosition = Vector3.zero;
            translationPoint.localRotation = Quaternion.identity;
        }

        void OnDisable()
        {
            // Auto-release controls when they are hidden
            if (attachedInteractionPoint)
            {
                attachedInteractionPoint.ForceUngrab(this);
            }
        }


        public bool Grabbed(ControllerInteractionPoint interactionPoint)
        {
            if (attachedInteractionPoint != null) return false;
            // Don't allow joystick use when editing is unlocked, so the movable surface can be used instead
            if (!controller.editLocked) return false;

            attachedInteractionPoint = interactionPoint;

            rotationZeroPoint.rotation = attachedInteractionPoint.transform.rotation;
            rotationZeroPoint.position = attachedInteractionPoint.transform.position;
            rotationPoint.rotation = attachedInteractionPoint.transform.rotation;
            translationPoint.position = attachedInteractionPoint.transform.position;

            // Is the controller being held upright light a joystick, or forward like the ship
            upright = Vector3.Angle(Vector3.up, rotationPoint.transform.forward) < 40f;

            if (buttons)
            {
                buttons.Grabbed(interactionPoint.Hand == TrackedHand.Hand.Right ? ActionsController.Hand.Right : ActionsController.Hand.Left);
            }

            return true;
        }

        public void Ungrabbed(ControllerInteractionPoint interactionPoint)
        {
            if (interactionPoint == attachedInteractionPoint)
            {
                attachedInteractionPoint = null;

                if (buttons)
                {
                    buttons.Ungrabbed();
                }

                if (line)
                {
                    line.SetPosition(1, Vector3.zero);
                }
                if (marker)
                {
                    marker.gameObject.SetActive(false);
                    marker.transform.localPosition = Vector3.zero;
                    var euler = marker.transform.localEulerAngles;
                    euler.y = 0;
                    marker.transform.localEulerAngles = euler;
                }
                if (verticalDisplay)
                {
                    verticalDisplay.transform.localPosition = Vector3.zero;
                }
                if (verticalLine)
                {
                    verticalLine.transform.localPosition = Vector3.zero;
                    verticalLine.SetPosition(1, Vector3.zero);
                }
                if (rollMarker)
                {
                    rollMarker.gameObject.SetActive(false);
                    rollMarker.transform.localPosition = Vector3.zero;
                    rollMarker.transform.localEulerAngles = Vector3.zero;
                }

                if (output)
                {
                    output.SetThrusters(ThrusterAxis.Zero);
                    output.SetStickAxis(StickAxis.Zero);
                }
            }
        }

        void Update()
        {
            if (attachedInteractionPoint == null) return;

            translationPoint.position = attachedInteractionPoint.transform.position;
            var thrusterAxis = new ThrusterAxis(translationPoint.localPosition, thrusterMaxDistance);

            rotationPoint.rotation = attachedInteractionPoint.transform.rotation;
            var angles = rotationPoint.localEulerAngles;
            StickAxis rotationAxis;
            if (upright)
            {
                rotationAxis = new StickAxis(angles);
            }
            else
            {
                rotationAxis = new StickAxis(angles.x, -angles.z + 360f, angles.y);
            }

            var endpoint = thrusterAxis.Value * thrusterMaxDistance;
            if (line)
            {
                line.SetPosition(1, new Vector3(endpoint.x, 0, endpoint.z));
            }
            if (marker)
            {
                marker.gameObject.SetActive(true);
                marker.transform.localPosition = new Vector3(endpoint.x, 0, endpoint.z);
                var euler = marker.transform.localEulerAngles;
                euler.y = rotationAxis.Yaw;
                marker.transform.localEulerAngles = euler;
            }
            if (verticalDisplay)
            {
                verticalDisplay.transform.localPosition = new Vector3(0, 0, endpoint.z);
            }
            if (verticalLine)
            {
                verticalLine.transform.localPosition = new Vector3(endpoint.x, 0, 0);
                verticalLine.SetPosition(1, new Vector3(0, endpoint.y, 0));
            }
            if (rollMarker)
            {
                rollMarker.gameObject.SetActive(true);
                rollMarker.transform.localPosition = new Vector3(endpoint.x, endpoint.y, 0);
                rollMarker.transform.localEulerAngles = new Vector3(0, 0, -rotationAxis.Roll);
            }

            if (output)
            {
                output.SetThrusters(thrusterAxis);
                output.SetStickAxis(rotationAxis);
            }
        }
    }
}
