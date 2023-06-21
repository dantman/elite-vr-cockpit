using UnityEngine;

namespace EVRC.Core.Overlay
{

    /// <summary>
    ///     Interface for holographic textures
    /// </summary>
    /// <remarks>
    ///     Formerly IButtonImage
    /// </remarks>
    public interface IColorable
    {
        void SetBaseColors(Color baseColor, Color invalid, Color unavailable);
        void SetTexture(Texture texture);
    }
}
