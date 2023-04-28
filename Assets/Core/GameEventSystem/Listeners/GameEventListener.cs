using UnityEngine;
using UnityEngine.Events;

namespace EVRC.Core
{
    /// <summary>
    /// A MonoBehaviour that forwards a GameEvent invocation to a UnityEvent
    /// </summary>
    public sealed class GameEventListener : MonoBehaviour
    {
        public GameEvent Source;
        public UnityEvent Response;

        private void OnEnable()
        {
            Source.Event += Response.Invoke;
        }

        private void OnDisable()
        {
            Source.Event -= Response.Invoke;
        }

    }

}
