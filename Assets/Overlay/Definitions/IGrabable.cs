using System;

namespace EVRC.Core.Overlay
{
    [Flags]
    public enum GrabMode
    {
        None = 0,
        Grabable = 1 << 0,
        ToggleGrabable = 1 << 1,
        PinchGrabable = 1 << 2,
        SmallObject = Grabable | PinchGrabable,
        Panel = Grabable,
        VirtualControl = Grabable | ToggleGrabable,
    }

    public interface IGrabable
    {
        /// <summary>
        /// Return the currently-supported GrabMode types
        /// </summary>
        GrabMode GetGrabMode();
        /// <summary>
        /// Initiate a grab, returns whether the grab was successful
        /// </summary>
        bool Grabbed(ControllerInteractionPoint interactionPoint);
        /// <summary>
        /// Terminate a grab
        /// </summary>
        void Ungrabbed(ControllerInteractionPoint interactionPoint);
    }
}
