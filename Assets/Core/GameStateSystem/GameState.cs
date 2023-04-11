using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC
{
    public abstract class GameState : ScriptableObject
    {
        public abstract string GetStatusText();
    }
}
