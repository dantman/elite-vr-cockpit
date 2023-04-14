namespace EVRC.Core.Overlay
{
    public interface IHoverable
    {
        void Hover(ControllerInteractionPoint interactionPoint);
        void Unhover(ControllerInteractionPoint interactionPoint);
    }
}
