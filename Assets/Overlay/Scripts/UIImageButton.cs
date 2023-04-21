using UnityEngine;
using UnityEngine.UI;

namespace EVRC.Core.Overlay
{
    using Utils = OverlayUtils;

    /**
     * A helper that allows a UI.Image to be used in place of a HolographicButton
     */
    [RequireComponent(typeof(Image))]
    public class UIImageButton : MonoBehaviour, IHolographic
    {
        public Sprite backface;
        private Texture lastTexture;
        private Sprite sprite;
        private bool lastIsFacingHmd = true;
        private Image _image;
        private Image image
        {
            get
            {
                if (_image == null)
                {
                    _image = GetComponent<Image>();
                }

                return _image;
            }
        }

        private void Awake()
        {
            sprite = image.sprite;
        }

        public void SetColor(Color setColor)
        {
            image.color = setColor;
            OnDemandRenderer.SafeDirty(gameObject);
        }

        public void SetHighlightColor(Color color)
        {
            Debug.Log("SetHighlightColor does nothing for UIImageButtons (intentionally)");
        }

        public void SetTexture(Texture texture)
        {
            var texture2d = texture as Texture2D;
            if (texture2d == null)
            {
                Debug.LogWarningFormat("UIImageButton only works with Texture2D textures, cannot use {0}", texture); ;
                return;
            }

            if (lastTexture != texture)
            {
                sprite = Sprite.Create(texture2d, new Rect(0, 0, texture.width, texture.height), Vector3.zero);
                lastTexture = texture;
                if (lastIsFacingHmd) Refresh(true);
            }
        }

        public void Highlight()
        {
            Debug.Log("Highlight does nothing for UIImageButtons (intentionally)");
        }

        public void UnHighlight()
        {
            Debug.Log("UnHighlight does nothing for UIImageButtons (intentionally)");
        }

        private void Update()
        {
            var isFacingHmd = Utils.IsFacingHmd(transform);
            if (lastIsFacingHmd != isFacingHmd)
            {
                Refresh(isFacingHmd);
            }
        }

        private void Refresh(bool isFacingHmd)
        {
            image.sprite = isFacingHmd ? sprite : backface;
            lastIsFacingHmd = isFacingHmd;
            OnDemandRenderer.SafeDirty(gameObject);
        }
    }
}
