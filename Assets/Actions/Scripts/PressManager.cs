using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

namespace EVRC.Core.Actions
{
    using Events = SteamVR_Events;

    abstract public class PressManager
    {
        public delegate UnpressHandlerDelegate<T> PressHandlerDelegate<T>(T ev);
        public delegate void UnpressHandlerDelegate<T>(T ev);
        public delegate void StateChangeHandlerDelegate<T>(T ev);

        protected MonoBehaviour owner;
        protected HashSet<Action> cleanupActions = new HashSet<Action>();

        public PressManager(MonoBehaviour owner)
        {
            this.owner = owner;
        }

        public delegate bool EventComparator<PressEvent>(PressEvent pEv, PressEvent uEv);

        public void Clear()
        {
            foreach (Action cleanup in cleanupActions)
            {
                cleanup();
            }

            cleanupActions.Clear();
        }

        protected void AddHandler<PressEvent>(
            PressHandlerDelegate<PressEvent> handler,
            EventComparator<PressEvent> comparator,
            Events.Event<PressEvent> pressEvent,
            Events.Event<PressEvent> unpressEvent
        )
        {
            UnityAction<PressEvent> ephemeralHandler = (PressEvent pEv) =>
            {
                var unpressHandler = handler(pEv);
                UnityAction<PressEvent> ephemeralUnpressHandler = null;
                void cleanupEphemeralUnpressHandler()
                {
                    unpressEvent.Remove(ephemeralUnpressHandler);
                }
                ephemeralUnpressHandler = (PressEvent uEv) =>
                {
                    if (!comparator(pEv, uEv)) return;

                    unpressHandler(uEv);
                    cleanupEphemeralUnpressHandler();
                    cleanupActions.Remove(cleanupEphemeralUnpressHandler);
                };
                unpressEvent.Listen(ephemeralUnpressHandler);
                cleanupActions.Add(cleanupEphemeralUnpressHandler);
            };
            pressEvent.Listen(ephemeralHandler);

            // Cleanup
            cleanupActions.Add(() =>
            {
                pressEvent.Remove(ephemeralHandler);
            });
        }

        protected void AddHandler<ChangeEvent>(
            StateChangeHandlerDelegate<ChangeEvent> handler,
            EventComparator<ChangeEvent> comparator,
            Events.Event<ChangeEvent> changeEvent
        )
        {
            UnityAction<ChangeEvent> ephemeralHandler = (ChangeEvent pEv) =>
            {
                handler(pEv);
            };
            changeEvent.Listen(ephemeralHandler);

            // Cleanup
            cleanupActions.Add(() =>
            {
                changeEvent.Remove(ephemeralHandler);
            });
        }
    }
}
