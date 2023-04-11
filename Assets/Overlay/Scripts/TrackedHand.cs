using UnityEngine;

namespace EVRC
{
    using Hand = ActionsController.Hand;

    public class TrackedHand : MonoBehaviour
    {
        public Hand hand = Hand.Unknown;

        //void OnEnable()
        //{
        //    ActionsController.HandPoseUpdate.AddListener(OnHandPoseUpdate);
        //}

        //void OnDisable()
        //{
        //    ActionsController.HandPoseUpdate.RemoveListener(OnHandPoseUpdate);
        //}

        //private void OnHandPoseUpdate(Hand hand, Vector3? newPosition, Quaternion? newRotation)
        //{
        //    if (hand != this.hand) return;

        //    if (newPosition.HasValue && newRotation.HasValue) {
        //        transform.localPosition = newPosition.Value;
        //        transform.localRotation = newRotation.Value;
        //        //Debug.Log($"hand: {hand} ---- newRotation: {newRotation.Value}");
        //    } else
        //    {
        //        // @todo Hide controller overlay when not active
        //    }
        //}

        //void OnDrawGizmos()
        //{
        //    Gizmos.matrix = transform.localToWorldMatrix;
        //    Gizmos.color = Color.blue;
        //    Gizmos.DrawWireSphere(Vector3.zero, 0.01f);
        //    Gizmos.DrawLine(Vector3.zero, Vector3.forward * 0.05f);
        //}
    }
}
