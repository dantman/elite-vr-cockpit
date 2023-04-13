using UnityEngine;

namespace EVRC.Core
{
    public abstract class GameState : ScriptableObject
    {
        /// <summary>
        /// Flexible method for displaying the current state of an object/system.
        /// </summary>
        /// <returns>String representation of the Current Status</returns>
        public abstract string GetStatusText();
    }
}
