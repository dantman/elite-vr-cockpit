namespace EVRC
{
    public interface IGrabable
    {
        bool Grabbed(ControllerInteractionPoint interactionPoint);
        void Ungrabbed(ControllerInteractionPoint interactionPoint);
    }
}
