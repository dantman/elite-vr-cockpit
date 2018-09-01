using UnityEngine;

namespace EVRC
{
    public class MovableSurface : MonoBehaviour, IGrabable
    {
        public bool rotatable = true;
        protected CockpitStateController controller;
        private ControllerInteractionPoint attachedInteractionPoint;
        private Transform attachPoint;

        void Start()
        {
            controller = CockpitStateController.instance;
        }

        public bool Grabbed(ControllerInteractionPoint interactionPoint)
        {
            if (attachedInteractionPoint != null) return false;
            if (controller.editLocked) return false;

            attachedInteractionPoint = interactionPoint;

            var attachPointObject = new GameObject("[AttachPoint]");
            attachPointObject.transform.SetParent(attachedInteractionPoint.transform);
            attachPointObject.transform.SetPositionAndRotation(transform.position, transform.rotation);
            attachPoint = attachPointObject.transform;

            return true;
        }

        public void Ungrabbed(ControllerInteractionPoint interactionPoint)
        {
            if (interactionPoint == attachedInteractionPoint)
            {
                attachedInteractionPoint = null;
                Destroy(attachPoint.gameObject);
                attachPoint = null;
            }
        }

        void LateUpdate()
        {
            if (attachedInteractionPoint == null) return;

            var t = attachPoint;
            if (rotatable)
            {
                transform.SetPositionAndRotation(t.position, t.rotation);
            }
            else
            {
                transform.position = t.position;
            }
        }
    }
}
