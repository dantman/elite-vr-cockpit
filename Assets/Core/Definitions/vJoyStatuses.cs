using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC
{
    public enum VJoyStatus
    {
        Unknown,
        NotInstalled,
        VersionMismatch,
        DeviceUnavailable,
        DeviceOwned,
        DeviceError,
        DeviceMisconfigured,
        DeviceNotAquired,
        Ready,
    }
}
