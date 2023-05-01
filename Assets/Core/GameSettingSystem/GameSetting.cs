using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC.Core
{
    /// <summary>
    /// Base class for a single configurable setting. These can be used Standalone or with a SettingsManager script.
    /// </summary>
    /// <typeparam name="Value">The type of value that can be set by the user</typeparam>
    [Serializable]
    public abstract class GameSetting<Value> : ScriptableObject
    {
        public Value value;
        public string displayName;
        public string saveFileKey;

        [Tooltip("The Queryable name of the component in the UI. Required for attaching controls to the Desktop UI")]
        public string visualElementName;

    }
}
