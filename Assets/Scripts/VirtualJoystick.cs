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

            public StickAxis Zero
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
        }

        public Color color;
        public Color highlightColor;
        public HolographicRect line;
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
            Debug.Log("Pitch: " + axis.Pitch + " Roll: " + axis.Roll + " Yaw: " + axis.Yaw);
        }
    }
}
