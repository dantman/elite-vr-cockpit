using UnityEngine;
using UnityEngine.UI;

namespace EVRC.Core.Overlay
{
    [RequireComponent(typeof(Image))]
    public class AlphaRaycastButtonImage : MonoBehaviour
    {
        [Range(0f, 1f)]
        public float alphaHitTestMinimumThreshold = 0f;

        private void Start()
        {
            Image image = GetComponent<Image>();
            if (image)
            {
                image.alphaHitTestMinimumThreshold = alphaHitTestMinimumThreshold;
            }
        }
    }
}
