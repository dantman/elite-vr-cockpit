using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC
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
