using System;
using UnityEngine.Events;
using static EVRC.CockpitUIMode;

namespace EVRC
{
    [Serializable]
    public sealed class BoolUnityEvent : UnityEvent<bool>
    {
    }

    [Serializable]
    public sealed class IntUnityEvent : UnityEvent<int>
    {
    }

    [Serializable]
    public sealed class FloatUnityEvent : UnityEvent<float>
    {
    }

    [Serializable]
    public sealed class DoubleUnityEvent : UnityEvent<double>
    {
    }

    [Serializable]
    public sealed class StringUnityEvent : UnityEvent<string>
    {
    }

    [Serializable]
    public sealed class CockpitModeUnityEvent : UnityEvent<CockpitMode>
    {
    }
}