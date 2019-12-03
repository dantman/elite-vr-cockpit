using System.Collections;
using UnityEngine;

namespace EVRC
{
    /**
     * This helper is placed at the root of a tree to listen to dirty calls from inside that root
     * that indicate an OnDemand IRenderable should be rendered
     */
    public class OnDemandRenderer : MonoBehaviour
    {
        private IRenderable _renderable;
        private IRenderable renderable
        {
            get
            {
                if (_renderable == null)
                {
                    _renderable = GetComponent<IRenderable>();
                }

                return _renderable;
            }
        }
        private bool queued = false;

        private void OnEnable()
        {
            // Mark as dirty on-enable to ensure a render is done when the element becomes available
            // Otherwise a purely on-demand element will be invisible
            Dirty();
        }

        public void Dirty()
        {
            if (!queued)
            {
                queued = true;
                StartCoroutine(DelayedRender());
            }
        }

        private IEnumerator DelayedRender()
        {   
            yield return null;
            yield return new WaitForEndOfFrame();
            queued = false;
            renderable.Render();
        }

        /**
         * Find the OnDemandRenderer above a node and mark it as dirty
         */
        public static void Dirty(GameObject go)
        {
            if (!SafeDirty(go))
            {
                Debug.LogErrorFormat("Could not find an OnDemandRenderer in the ancestors of {0}", go.name);
            }
        }

        /**
         * If the node has an OnDemandRenderer, mark it as dirty
         * Don't do anything if there is no OnDemandRenderer
         */
        public static bool SafeDirty(GameObject go)
        {
            var onDemandRenderer = go.GetComponentInParent<OnDemandRenderer>();
            if (onDemandRenderer == null) return false;

            onDemandRenderer.Dirty();
            return true;
        }
    }
}
