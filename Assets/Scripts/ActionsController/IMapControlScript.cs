using System;
using UnityEngine;


namespace EVRC
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
