using System.Collections.Generic;
using UnityEngine;

namespace EVRC.Core.Overlay
{
    public class SpawnZoneCell : MonoBehaviour
    {
        protected HashSet<Collider> intersecting = new HashSet<Collider>();

        public bool Free
        {
            get
            {
                return intersecting.Count == 0;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Ignore colliders for interaction points
            if (other.GetComponent<ControllerInteractionPoint>()) return;

            intersecting.Add(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (intersecting.Contains(other))
            {
                intersecting.Remove(other);
            }
        }
    }
}
