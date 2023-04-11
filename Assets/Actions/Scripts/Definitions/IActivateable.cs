using System;

namespace EVRC
{
    public interface IActivateable
    {
        Action Activate(ControllerInteractionPoint interactionPoint);
    }
}
