using UnityEngine;
using UnityEngine.Events;

namespace EVRC
{
    // <summary>
    /// Abstract base class for a GameEventListener that receives a single
    /// argument as part of its event invocation.
    /// </summary>
    /// <typeparam name="T">The type of the event argument</typeparam>
    public abstract class GameEventListener<T> : MonoBehaviour
    {
        #region MonoBehaviour

        private void OnEnable()
        {
            Source.Event += Response.Invoke;
        }

        private void OnDisable()
        {
            Source.Event -= Response.Invoke;
        }

        #endregion

        protected abstract GameEvent<T> Source { get; }

        protected abstract UnityEvent<T> Response { get; }
    }

    /// <summary>
    /// Abstract base class for a GameEventListener that receives two
    /// arguments as part of its event invocation.
    /// </summary>
    /// <typeparam name="T1">The type of the first event argument</typeparam>
    public abstract class GameEventListener<T1, T2> : MonoBehaviour
    {
        #region MonoBehaviour

        private void OnEnable()
        {
            Source.Event += Response.Invoke;
        }

        private void OnDisable()
        {
            Source.Event -= Response.Invoke;
        }

        #endregion

        protected abstract GameEvent<T1, T2> Source { get; }

        protected abstract UnityEvent<T1, T2> Response { get; }
    }
}
