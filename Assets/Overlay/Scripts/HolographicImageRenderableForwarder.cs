using UnityEngine;

namespace EVRC.Core.Overlay
{
    /**
     * Acts as an IRenderable to forward those render calls to a HolographicImage
     */
    public class HolographicImageRenderableForwarder : MonoBehaviour, IRenderable
    {
        public HolographicImage holoImage;

        public void Render()
        {
            holoImage.Render();
        }
    }
}
