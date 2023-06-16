using EVRC.Core.Actions;
using System;
using UnityEngine.Events;

namespace EVRC.Core
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
    public sealed class EDStatusAndGuiUnityEvent : UnityEvent<EDStatusFlags, EDGuiFocus>
    {
    }
    
    [Serializable]
    public sealed class EDStateUnityEvent : UnityEvent<EliteDangerousState>
    {
    }

    
    [Serializable]
    public sealed class ControlButtonAssetUnityEvent : UnityEvent<ControlButtonAsset>
    {
    }
}