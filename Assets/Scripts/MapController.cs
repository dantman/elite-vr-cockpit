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

        public float maxMetersPerSecond = .0001f;
        public Vector3 scale = Vector3.one;
        public float zoomScale = 1f;

        private IEnumerator GrabLoop()
        {
            var lastPosition = new Dictionary<Hand, Vector3>();

            while (grabbingHands.Count > 0)
            {
                var positions = new Dictionary<Hand, Vector3>();
                var deltas = new Dictionary<Hand, Vector3>();
                // Translation calculations
                foreach (var hand in grabbingHands)
                {
                    var trackedHand = TrackedHand.Get(hand.ToTrackedHand());
                    positions[hand] = origin.InverseTransformPoint(trackedHand.transform.position);

                    if (lastPosition.ContainsKey(hand))
                    {
                        deltas[hand] = positions[hand] - lastPosition[hand];
                    }
                }
                // Zoom calculations
                float distanceDelta = 0;
                if (
                    lastPosition.ContainsKey(Hand.Left) && lastPosition.ContainsKey(Hand.Right)
                    && positions.ContainsKey(Hand.Left) && positions.ContainsKey(Hand.Right))
                {
                    var lastDistance = Vector3.Distance(lastPosition[Hand.Left], lastPosition[Hand.Right]);
                    var distance = Vector3.Distance(positions[Hand.Left], positions[Hand.Right]);
                    distanceDelta = distance - lastDistance;
                }

                lastPosition = positions;

                var posDelta = deltas.Count > 0
                    ? deltas.Values.Aggregate(Vector3.zero, (res, delta) => res + delta) / deltas.Count
                    : Vector3.zero;

                // var maxMetersPerSecond = .00001f;
                var axis = (posDelta / maxMetersPerSecond * Time.deltaTime) * -1f;
                axis = Vector3.Scale(axis, scale);// new Vector3(1f, .05f, 1f));
                
                if(output)
                {
                    output.SetThrusters(new Virtual6DOFController.ThrusterAxis(axis, 1f).WithDeadzone(0.05f));
                    output.SetThrottle(Mathf.Clamp(distanceDelta * zoomScale, -1f, 1f) * -1);
                }

                yield return null;
            }

            if (output)
            {
                output.SetThrusters(Virtual6DOFController.ThrusterAxis.Zero);
                output.SetThrottle(0);
            }

            running = null;
        }
    }
}
