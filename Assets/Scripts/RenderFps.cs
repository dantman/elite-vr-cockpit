using System.Collections;
using UnityEngine;

namespace EVRC
{
    /**
     * Renders an IRenderable at a fixed FPS
     */
    public class RenderFps : MonoBehaviour
    {
        public int fps = 30;
        private float frameInterval;
        private IRenderable renderable;

        private void OnEnable()
        {
            renderable = GetComponent<IRenderable>();
            if (renderable == null)
            {
                Debug.LogErrorFormat("Could not find an IRenderable on {0}", name);
            }

            frameInterval = 1f / fps;
            StartCoroutine(RenderLoop());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator RenderLoop()
        {
            while (enabled)
            {
                renderable.Render();

                yield return new WaitForSecondsRealtime(frameInterval);
            }
        }
    }
}
