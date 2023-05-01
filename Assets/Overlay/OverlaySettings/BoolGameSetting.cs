using System;
using UnityEngine;
using UnityEngine.UIElements;
using Valve.VR.InteractionSystem;

namespace EVRC.Core.Overlay
{

    /// <summary>
    /// Activates or Deactivates the associated Game Event when the value is toggled
    /// </summary>
    [CreateAssetMenu(menuName = Constants.SETTING_OBJECT_PATH + "/BoolGameSetting"), Serializable]
    public class BoolGameSetting : GameSetting<bool>
    {
        public BoolEvent boolGameEvent;

        /// <summary>
        /// Use for UI Callback
        /// </summary>
        /// <param name="evt">data sent when a bound element changes in the UI</param>
        public virtual void OnToggle(ChangeEvent<bool> evt)
        {
            if (evt.newValue == evt.previousValue) return;

            this.value = evt.newValue;
            boolGameEvent.Raise(value);
        }
    }
}
