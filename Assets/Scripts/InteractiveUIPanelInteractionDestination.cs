using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace EVRC
{
    public class InteractiveUIPanelInteractionDestination : MonoBehaviour
    {
        public Canvas ui;
        public LayerMask layerMask;
        private GameObject activeObject = null;

        /**
         * Handles a hovering cursor at a point
         * Accepts a cursor point in viewport (0-1) space.
         */
        public void Hover(Vector2 viewportPoint)
        {
            PointerEventData ev;
            if (PointerRaycast(viewportPoint, out ev, Color.blue, Time.deltaTime))
            {
                if (activeObject != ev.pointerCurrentRaycast.gameObject)
                {
                    if (activeObject != null)
                    {
                        ExecuteEvents.ExecuteHierarchy(activeObject, ev, ExecuteEvents.pointerExitHandler);
                    }

                    ExecuteEvents.ExecuteHierarchy(ev.pointerCurrentRaycast.gameObject, ev, ExecuteEvents.pointerEnterHandler);
                    activeObject = ev.pointerCurrentRaycast.gameObject;
                }
            }
        }

        /**
         * Handle the removal of a hovering cursor
         */
        public void Hover()
        {
            if (activeObject != null)
            {
                var ev = new PointerEventData(EventSystem.current);
                ExecuteEvents.ExecuteHierarchy(activeObject, ev, ExecuteEvents.pointerExitHandler);
                activeObject = null;
            }
        }

        /**
         * Handles a click somewhere on the UI panel
         * Accepts a click point in viewport (0-1) space.
         */
        public void Click(Vector2 viewportPoint)
        {
            PointerEventData ev;
            if (PointerRaycast(viewportPoint, out ev, Color.red, 2f))
            {
                var element = ev.pointerCurrentRaycast.gameObject;
                ExecuteEvents.ExecuteHierarchy(element, ev, ExecuteEvents.pointerDownHandler);
                ExecuteEvents.ExecuteHierarchy(element, ev, ExecuteEvents.pointerClickHandler);
                ExecuteEvents.ExecuteHierarchy(element, ev, ExecuteEvents.pointerUpHandler);
            }
        }

        /**
         * Cast a raycast from a relative pointer location and also draw a debug line for it
         */
        protected bool PointerRaycast(Vector2 viewportPoint, out PointerEventData pointerEvent, Color debugColor, float debugDuration)
        {
            var camera = ui.worldCamera;
            var raycaster = ui.GetComponent<GraphicRaycaster>();
            pointerEvent = new PointerEventData(EventSystem.current);
            pointerEvent.position = camera.ViewportToScreenPoint(viewportPoint);

#if UNITY_EDITOR
            Debug.DrawRay(camera.ScreenToWorldPoint(pointerEvent.position), camera.transform.forward * 10f, debugColor, debugDuration, true);
#endif

            var result = new List<RaycastResult>();
            raycaster.Raycast(pointerEvent, result);
            foreach (var firstHit in result)
            {
                pointerEvent.pointerCurrentRaycast = firstHit;
                return true;
            }

            return false;
        }
    }
}
