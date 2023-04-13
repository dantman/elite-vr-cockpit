using System;

namespace EVRC.Core.Actions
{
    using Direction = ActionsController.Direction;

    public interface IMapControlScript
    {
        Action POV1Direction(Direction direction);
        Action POV1Press();

        Action POV3Direction(Direction direction);
        Action POV3Press();

    }
}
