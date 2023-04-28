using System;
using UnityEngine;

namespace EVRC.Core.Overlay
{
    [RequireComponent(typeof(BoolEventListener))]
    public class Movable : MonoBehaviour, IGrabable
    {
        public enum ObjectType
        {
            SmallObject,
            Panel
        }
        public ObjectType objectType = ObjectType.SmallObject;
        public bool rotatable = true;
        private ControllerInteractionPoint attachedInteractionPoint;
        private Transform attachPoint;

        [Tooltip("Optional: will default to itself if not specified")]
        public Transform targetTransform;
        public OverlayEditLockState editLockState;
        private BoolEventListener editLockBoolEventListener;

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

        void Awake()
        {
            if (targetTransform == null)
            {
                targetTransform = this.transform;
            }

            editLockBoolEventListener = GetComponent<BoolEventListener>();
            if (editLockBoolEventListener.Response == null)
            {
                Debug.LogWarning($"EditLock Listener is not configured for {targetTransform.gameObject}");
            }
        }

        void OnDisable()
        {
            // Auto-release surfaces when they are hidden
            if (attachedInteractionPoint)
            {
                attachedInteractionPoint.ForceUngrab(this);
            }
        }

        public void OnEditLockedStateChanged(bool newState)
        {
            // Auto-release surfaces when edit mode is locked
            if (editLockState.EditLocked && attachedInteractionPoint)
            {
                attachedInteractionPoint.ForceUngrab(this);
            }
        }

        public bool Grabbed(ControllerInteractionPoint interactionPoint)
        {
            if (attachedInteractionPoint != null) return false;
            if (editLockState.EditLocked) return false;

            attachedInteractionPoint = interactionPoint;

            var attachPointObject = new GameObject("[AttachPoint]");
            attachPointObject.transform.SetParent(attachedInteractionPoint.transform);
            attachPointObject.transform.SetPositionAndRotation(targetTransform.position, targetTransform.rotation);
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
                targetTransform.SetPositionAndRotation(t.position, t.rotation);
            }
            else
            {
                targetTransform.position = t.position;
            }
        }
    }
}
