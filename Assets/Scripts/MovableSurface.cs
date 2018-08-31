using UnityEngine;

namespace EVRC
{
    public class MovableSurface : MonoBehaviour, IGrabable
    {
        public bool rotatable = true;
        protected CockpitStateController controller;
        private ControllerInteractionPoint attachedInteractionPoint;
        private Vector3 offsetPosition;
        private Quaternion offsetRotation;

        void Start()
        {
            controller = CockpitStateController.instance;
        }

        public bool Grabbed(ControllerInteractionPoint interactionPoint)
        {
            if (attachedInteractionPoint != null) return false;
            if (controller.editLocked) return false;

            attachedInteractionPoint = interactionPoint;

            var t = attachedInteractionPoint.transform;
            offsetPosition = transform.position - t.position;
            offsetRotation = transform.rotation * Quaternion.Inverse(t.rotation);

            return true;
        }

        public void Ungrabbed(ControllerInteractionPoint interactionPoint)
        {
            if (interactionPoint == attachedInteractionPoint)
            {
                attachedInteractionPoint = null;
            }
        }

        void Update()
        {
            if (attachedInteractionPoint == null) return;

            var t = attachedInteractionPoint.transform;
            if (rotatable)
            {
                transform.SetPositionAndRotation(
                    t.position + offsetPosition,
                    offsetRotation * t.rotation);
            }
            else
            {
                transform.position = t.position + offsetPosition;
            }
        }
    }
}
