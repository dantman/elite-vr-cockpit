using System;
using UnityEngine;

namespace EVRC
{
    [CreateAssetMenu(menuName = Constants.GAME_EVENT_PATH + "/Base (no parameter)"), Serializable]
    /// <summary>
    /// An event implemented as a ScriptableObject, which recives no arguments
    /// </summary>
    public sealed class GameEvent : ScriptableObject
    {
        public event Action Event;

        public void Raise()
        {
            Event?.Invoke();
        }
    }
}