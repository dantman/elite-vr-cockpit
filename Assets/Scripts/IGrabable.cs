using System;

namespace EVRC
{
    [Flags]
    public enum GrabMode
    {
        None = 0,
        Hold = 1 << 0,
        Toggle = 1 << 1,
        Pinch = 1 << 2,
    }

    public interface IGrabable
    {
        GrabMode SupportedModes
        {
            /// <summary>
            /// Return the currently-supported GrabMode types
            /// </summary>
            get;
        }

        bool Grabbed(ControllerInteractionPoint interactionPoint);
        void Ungrabbed(ControllerInteractionPoint interactionPoint);
    }

    public static class IGrabableExtensions
    {
        public static bool CanGrabInMode(this IGrabable grabable, GrabMode mode)
        {
            return (grabable.SupportedModes & mode) != 0;
        }
    }
}
