using System;
using UnityEngine;

namespace EVRC
{
    public class MovableSurface : MonoBehaviour, IGrabable
    {
        public enum ObjectType
        {
            SmallObject,
            Panel
        }
        public ObjectType objectType = ObjectType.SmallObject;
        public bool rotatable = true;
        protected CockpitStateController controller;
        private ControllerInteractionPoint attachedInteractionPoint;
        private Transform attachPoint;

        public GrabMode GetGrabMode()
        {
            switch (objectType)
            {
                case ObjectType.SmallObject:
                    return GrabMode.SmallObject;
                case ObjectType.Panel:
                    return GrabMode.Panel;
            }

            throw new NotImplementedException("Missing case for a GrabMode type");
        }

        void Start()
        {
            controller = CockpitStateController.instance;
        }

        void OnEnable()
        {
            CockpitStateController.EditLockedStateChanged.Listen(OnEditLockedStateChanged);
        }

        void OnDisable()
        {
            CockpitStateController.EditLockedStateChanged.Remove(OnEditLockedStateChanged);

            // Auto-release surfaces when they are hidden
            if (attachedInteractionPoint)
            {
                attachedInteractionPoint.ForceUngrab(this);
            }
        }

        private void OnEditLockedStateChanged(bool editLocked)
        {
            // Auto-release surfaces when edit mode is locked
            if (editLocked && attachedInteractionPoint)
            {
                attachedInteractionPoint.ForceUngrab(this);
            }
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
