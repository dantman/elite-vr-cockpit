using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC
{
    using Hand = ActionsController.Hand;
    using BtnAction = ActionsController.BtnAction;
    using ButtonActionsPress = ActionsController.ButtonActionsPress;
    using Direction = ActionsController.Direction;

    public class MapController : MonoBehaviour
    {
        public Transform origin;
        public vJoyInterface output;
        private HashSet<Hand> grabbingHands = new HashSet<Hand>();
        private Coroutine running;

        void OnEnable()
        {
            ActionsController.GrabPress.Listen(OnGrabPress);
            ActionsController.GrabUnpress.Listen(OnGrabUnpress);
        }

        void OnDisable()
        {
            ActionsController.GrabPress.Remove(OnGrabPress);
            ActionsController.GrabUnpress.Remove(OnGrabUnpress);
        }

        private void OnGrabPress(ActionsController.ButtonPress ev)
        {
            if (ev.hand == Hand.Unknown) return;

            grabbingHands.Add(ev.hand);

            if (running == null)
            {
                StartCoroutine(GrabLoop());
            }
        }

        private void OnGrabUnpress(ActionsController.ButtonPress ev)
        {
            grabbingHands.Remove(ev.hand);
        }

        private IEnumerator GrabLoop()
        {
            var lastPosition = new Dictionary<Hand, Vector3>();

            while (grabbingHands.Count > 0)
            {
                var positions = new Dictionary<Hand, Vector3>();
                var deltas = new Dictionary<Hand, Vector3>();
                foreach (var hand in grabbingHands)
                {
                    var trackedHand = TrackedHand.Get(hand.ToTrackedHand());
                    positions[hand] = origin.InverseTransformPoint(trackedHand.transform.position);

                    if (lastPosition.ContainsKey(hand))
                    {
                        deltas[hand] = positions[hand] - lastPosition[hand];
                    }
                }
                lastPosition = positions;

                var posDelta = deltas.Values.Aggregate(Vector3.zero, (res, delta) => res + delta) / deltas.Count;

                var maxMetersPerSecond = .001f;
                var axis = (posDelta / maxMetersPerSecond * Time.deltaTime);
                
                // Debug.Log(axis.ToString("R"));

                if(output)
                {
                    output.SetThrusters(new Virtual6DOFController.ThrusterAxis(axis, 1f).WithDeadzone(0.05f));
                }

                yield return null;
            }

            running = null;
        }
    }
}
