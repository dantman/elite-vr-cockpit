using System;
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
            VerifyEventCallbacks();
            Source.Event += Response.Invoke;
        }

        private void OnDisable()
        {
            Source.Event -= Response.Invoke;
        }

        private bool VerifyEventCallbacks()
        {
            int persistentEventCount = Response.GetPersistentEventCount();

            for (int i = 0; i < persistentEventCount; i++)
            {
                UnityEngine.Object target = Response.GetPersistentTarget(i);
                string methodName = Response.GetPersistentMethodName(i);

                if (target == null || string.IsNullOrEmpty(methodName))
                {
                    Debug.LogError($"Callback at index {i} in the UnityEvent 'Response' is missing or invalid. Object: {gameObject.transform.parent.gameObject.name} > {gameObject.name}");
                    return false;
                }
            }

            return true;
        }


    }

}
