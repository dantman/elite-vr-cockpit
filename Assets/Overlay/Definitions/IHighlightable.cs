using UnityEngine;

namespace EVRC.Core.Overlay
{
    public interface IHighlightable : IColorable
    {
        void Highlight();
        void UnHighlight();
        void SetHighlightColor(Color color);
    }
}
