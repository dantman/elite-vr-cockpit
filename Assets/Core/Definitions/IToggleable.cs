using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC.Core
{
    public interface IToggleable
    {
        void SetEnabled();
        void SetDisabled();
        void Toggle();
    }
}
