using UnityEngine;
using UnityEngine.Events;

namespace EVRC.Core
{
    
    /// <summary>
    /// Abstract base class for a GameEventListener that receives a single
    /// argument as part of its event invocation.
    /// </summary>
    /// <typeparam name="T">The type of the event argument</typeparam>
    public abstract class GameEventListener<T> : MonoBehaviour
    {
        public GameEvent<T> Source;
        public UnityEvent<T> Response;
        
        private void OnEnable()
        {
            Source.Event += Response.Invoke;
        }

        private void OnDisable()
        {
            Source.Event -= Response.Invoke;
        }

    }

    /// <summary>
    /// Abstract base class for a GameEventListener that receives two 
    /// arguments as part of its event invocation.
    /// </summary>
    /// <typeparam name="T1">The type of the event argument</typeparam>
    /// <typeparam name="T2">The second type of the event argument</typeparam>
    public abstract class GameEventListener<T1, T2> : MonoBehaviour
    {
        public GameEvent<T1, T2> Source;
        public UnityEvent<T1, T2> Response;

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
