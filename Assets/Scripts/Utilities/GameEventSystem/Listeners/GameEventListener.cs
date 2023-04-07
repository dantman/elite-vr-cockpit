using UnityEngine;
using UnityEngine.Events;

namespace EVRC
{
    /// <summary>
    /// A MonoBehaviour that forwards a GameEvent invocation to a UnityEvent
    /// </summary>
    public sealed class GameEventListener : MonoBehaviour
    {
        #region MonoBehaviour

        public GameEvent Event;
        public UnityEvent Response;

        private void OnEnable()
        {
            Event.Event += Response.Invoke;
        }

        private void OnDisable()
        {
            Event.Event -= Response.Invoke;
        }

        #endregion

        public void OnEventRaised()
        {
            Response.Invoke();
        }
    }
}
