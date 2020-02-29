using UnityEngine;

namespace EVRC
{
    enum ThrottleState
    {
        Forward,
        ForwardIdleDetent,
        Idle,
        ReverseIdleDetent,
        Reverse,
    }

    /**
     * A virtual 1-axis movable throttle that outputs to vJoy when grabbed
     */
    public class VirtualThrottle : MonoBehaviour, IGrabable, IHighlightable
    {

        public Color color;
        public Color highlightColor;
        [Range(0f, 1f)]
        public float magnitudeLength = 1f;
        public Transform handle;
        public HolographicRect line;
        public VirtualThrottleButtons buttons;
        public vJoyInterface output;
        protected CockpitStateController controller;
        private bool highlighted = false;
        private ControllerInteractionPoint attachedInteractionPoint;
        private Transform attachPoint;
        private ThrottleState state = ThrottleState.Idle;

        public GrabMode GetGrabMode()
        {
            return GrabMode.VirtualControl;
        }

        void Start()
        {
            controller = CockpitStateController.instance;

            Refresh();
        }

        void OnEnable()
        {
            EDStateManager.EliteDangerousStarted.Listen(OnGameStart);
        }

        void OnDisable()
        {
            EDStateManager.EliteDangerousStarted.Listen(OnGameStart);

            // Auto-release controls when they are hidden
            if (attachedInteractionPoint)
            {
                attachedInteractionPoint.ForceUngrab(this);
            }
        }

        void OnGameStart()
        {
            if (attachedInteractionPoint == null)
            {
                // Reset the throttle on game start so we don't start with a full throttle
                // @todo Perhaps listening to game start / docking events would be better
                handle.localPosition = Vector3.zero;
            }
        }

        public bool Grabbed(ControllerInteractionPoint interactionPoint)
        {
            if (attachedInteractionPoint != null) return false;
            // Don't allow throttle use when editing is unlocked, so the movable surface can be used instead
            if (!controller.editLocked) return false;

            attachedInteractionPoint = interactionPoint;

            var attachPointObject = new GameObject("[AttachPoint]");
            attachPointObject.transform.SetParent(attachedInteractionPoint.transform);
            attachPointObject.transform.SetPositionAndRotation(handle.position, handle.rotation);
            attachPoint = attachPointObject.transform;

            if (buttons)
            {
                buttons.Grabbed(interactionPoint.Hand);
            }

            return true;
        }

        public void Ungrabbed(ControllerInteractionPoint interactionPoint)
        {
            if (interactionPoint == attachedInteractionPoint)
            {
                attachedInteractionPoint = null;
                Destroy(attachPoint.gameObject);
                attachPoint = null;

                if (buttons)
                {
                    buttons.Ungrabbed();
                }

                var throttle = handle.localPosition.z / magnitudeLength;
                if (Mathf.Abs(throttle) < output.throttleDeadzonePercentage)
                {
                    handle.localPosition = Vector3.zero;
                    state = ThrottleState.Idle;
                }
            }
        }

        void OnValidate()
        {
            Refresh();
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

        public void Refresh()
        {
            if (line)
            {
                line.width = magnitudeLength * 2;
                if (highlighted)
                {
                    line.color = highlightColor;
                }
                else
                {
                    line.color = color;
                }
            }

            var collider = GetComponent<CapsuleCollider>();
            if (collider)
            {
                collider.height = magnitudeLength * 2 + 0.04f;
            }
        }

        void LateUpdate()
        {
            if (attachedInteractionPoint == null) return;
            if (!handle) return;

            var t = attachPoint;
            handle.position = t.position;

            var localMagnitude = Mathf.Clamp(handle.localPosition.z, -magnitudeLength, magnitudeLength);
            var throttle = localMagnitude / magnitudeLength;
            if (Mathf.Abs(throttle) < detentSize)
            {
                MoveThrottleToIdleDetent();
            }
            else
            {
                handle.localPosition = new Vector3(0, 0, localMagnitude);
            }

            CheckStateChange(throttle);
        }

        void Update()
        {
            if (attachedInteractionPoint == null) return;
            if (!output) return;

            var throttle = handle.localPosition.z / magnitudeLength;
            output.SetThrottle(throttle);
        }

        private float detentSize
        {
            get
            {
                return output.throttleDeadzonePercentage / 100f;
            }
        }

        private float reverseDetentSize
        {
            get
            {
                return 2 * detentSize;
            }
        }


        private void CheckStateChange(float throttle)
        {
            switch (state)
            {
                case ThrottleState.Forward:
                    if (throttle < detentSize)
                    {
                        state = ThrottleState.ForwardIdleDetent;
                        EmitHapticDetent();
                    }
                    break;

                case ThrottleState.ForwardIdleDetent:
                    if (throttle < -reverseDetentSize)
                    {
                        state = ThrottleState.Reverse;
                        EmitHapticDetent();
                    }
                    else if (throttle > detentSize)
                    {
                        state = ThrottleState.Forward;
                        EmitHapticDetent();
                    }
                    else
                    {
                        MoveThrottleToIdleDetent();
                    }
                    break;

                case ThrottleState.Idle:
                    if (throttle > detentSize)
                    {
                        state = ThrottleState.Forward;
                        EmitHapticDetent();
                    }
                    else if (throttle < -detentSize)
                    {
                        state = ThrottleState.Reverse;
                        EmitHapticDetent();
                    }
                    else
                    {
                        MoveThrottleToIdleDetent();
                    }
                    break;

                case ThrottleState.ReverseIdleDetent:
                    if (throttle > reverseDetentSize)
                    {
                        state = ThrottleState.Forward;
                        EmitHapticDetent();
                    }
                    else if (throttle < -detentSize)
                    {
                        state = ThrottleState.Reverse;
                        EmitHapticDetent();
                    }
                    else
                    {
                        MoveThrottleToIdleDetent();
                    }
                    break;

                case ThrottleState.Reverse:
                    if (throttle > -detentSize)
                    {
                        state = ThrottleState.ReverseIdleDetent;
                        EmitHapticDetent();
                    }
                    break;
            }
        }

        private void MoveThrottleToIdleDetent()
        {
            handle.localPosition = Vector3.zero;
        }

        private void EmitHapticDetent()
        {
            // TODO emit a haptic pulse for the attached controller
        }
    }
}
