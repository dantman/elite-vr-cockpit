using UnityEngine;

namespace EVRC
{
    using StickAxis = VirtualJoystick.StickAxis;

    /**
     * A grabbable virtual control that offers 6-axis control of the ship based on the controller's movement
     */
    public class Virtual6DOFController : MonoBehaviour, IGrabable, IHighlightable
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
        public VirtualJoystickButtons buttons;
        public vJoyInterface output;
        protected CockpitStateController controller;
        private bool highlighted = false;
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

            Refresh();
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

                if (output)
                {
                    output.SetThrusters(ThrusterAxis.Zero);
                    output.SetStickAxis(StickAxis.Zero);
                }
            }
        }

        public void OnHover()
        {
            highlighted = true;
            Refresh();
        }

        public void OnUnhover()
        {
            highlighted = false;
            Refresh();
        }

        void Refresh()
        {
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

            if (output)
            {
                output.SetThrusters(thrusterAxis);
                output.SetStickAxis(rotationAxis);
            }
        }
    }
}
