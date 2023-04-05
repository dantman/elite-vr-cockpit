using UnityEngine;

namespace EVRC
{
    public class WristTurnActivate : MonoBehaviour
    {
        private float zLow = 100.0f;
        private float zHigh = 170.0f;
        private float yLow = 235.0f;
        private float yHigh = 295.0f;
        public GameObject target;

        void Awake()
        {
            if(target == null) 
            {
                Debug.LogError("No Target for WristTurnActivate");
                return;
            }
        }

        void Update()
        {
            // if angles match, activate the target
            var euAngle = this.gameObject.transform.localRotation.eulerAngles;
            if (euAngle.z > zLow && euAngle.z <= zHigh && euAngle.y > yLow && euAngle.y < yHigh)
            {
                if (!target.activeInHierarchy)
                {
                    target.SetActive(true);
                }
                return;
            }

            // turn off when angles don't match
            if (target.activeInHierarchy)
            {
                target.SetActive(false);
                return;
            }
        }
    }
}