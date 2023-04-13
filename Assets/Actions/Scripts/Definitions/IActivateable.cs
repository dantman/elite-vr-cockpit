using System;
using EVRC.Core.Overlay;

namespace EVRC.Core.Actions
{
    public interface IActivateable
    {
        Action Activate(ControllerInteractionPoint interactionPoint);
    }
}
