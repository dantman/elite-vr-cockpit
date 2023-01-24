using UnityEngine;

namespace EVRC
{

    /// <summary>
    ///     Interface for holographic textures
    /// </summary>
    /// <remarks>
    ///     Formerly IButtonImage
    /// </remarks>
    public interface IHolographic
    {
        void SetColor(Color color);
        void SetTexture(Texture texture);
    }
}
