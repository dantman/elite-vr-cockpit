using UnityEngine;

namespace EVRC
{
    /**
     * A virtual 3-axis joystick that outputs to vJoy when grabbed
     */
    public class VirtualJoystick : MonoBehaviour, IGrabable, IHighlightable
    {
        public struct StickAxis
        {
            public float Pitch;
            public float Roll;
            public float Yaw;

            public static StickAxis Zero
            {
                get
                {
                    return new StickAxis(0, 0, 0);
                }
            }

            public StickAxis(float pitch, float roll, float yaw)
            {
                if (pitch > 180f) pitch -= 360f;
                if (roll > 180f) roll -= 360f;
                if (yaw > 180f) yaw -= 360f;

                Pitch = pitch;
                Roll = roll;
                Yaw = yaw;
            }

            public StickAxis(Vector3 angles) : this(angles.x, angles.y, angles.z) { }

            /**
             * Returns a new StickAxis with a deadZone limit
             */
            public StickAxis WithDeadzone(float deadZone)
            {
                return new StickAxis(
                    Mathf.Abs(Pitch) < deadZone ? 0f : Pitch,
                    Mathf.Abs(Roll) < deadZone ? 0f : Roll,
                    Mathf.Abs(Yaw) < deadZone ? 0f : Yaw);
            }
        }

        public Color color;
        public Color highlightColor;
        public HolographicRect line;
        public vJoyInterface output;
        protected CockpitStateController controller;
        private bool highlighted = false;
        private ControllerInteractionPoint attachedInteractionPoint;
        private Transform zeroPoint;
        private Transform rotationPoint;

        void Start()
        {
            controller = CockpitStateController.instance;

            var zeroPointObject = new GameObject("[ZeroPoint]");
            zeroPoint = zeroPointObject.transform;
            zeroPoint.SetParent(transform);
            zeroPoint.localPosition = Vector3.zero;
            zeroPoint.localRotation = Quaternion.identity;

            var rotationPointObject = new GameObject("[RotationPoint]");
            rotationPoint = rotationPointObject.transform;
            rotationPoint.SetParent(zeroPoint);
            rotationPoint.localPosition = Vector3.zero;
            rotationPoint.localRotation = Quaternion.identity;

            Refresh();
        }

        public bool Grabbed(ControllerInteractionPoint interactionPoint)
        {
            if (attachedInteractionPoint != null) return false;
            // Don't allow joystick use when editing is unlocked, so the movable surface can be used instead
            if (!controller.editLocked) return false;

            attachedInteractionPoint = interactionPoint;

            zeroPoint.rotation = attachedInteractionPoint.transform.rotation;
            rotationPoint.rotation = attachedInteractionPoint.transform.rotation;

            return true;
        }

        public void Ungrabbed(ControllerInteractionPoint interactionPoint)
        {
            if (interactionPoint == attachedInteractionPoint)
            {
                attachedInteractionPoint = null;

                if (output)
                {
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
            if (line)
            {
                if (highlighted)
                {
                    line.color = highlightColor;
                }
                else
                {
                    line.color = color;
                }
            }
        }

        void Update()
        {
            if (attachedInteractionPoint == null) return;

            rotationPoint.rotation = attachedInteractionPoint.transform.rotation;

            var axis = new StickAxis(rotationPoint.localEulerAngles);
            
            if (output)
            {
                output.SetStickAxis(axis);
            }
        }
    }
}
