using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC.Core
{
    public abstract class GameStateBase : ScriptableObject
    {
        /// <summary>
        /// Flexible method for displaying the current state of an object/system.
        /// </summary>
        /// <returns>String representation of the Current Status</returns>
        public abstract string GetStatusText();

    }
    
    public abstract class GameState : GameStateBase
    {
        public GameEvent gameEvent;
    }

    public abstract class GameState<T> : GameStateBase
    {
        public GameEvent<T> gameEvent;
    }
}
