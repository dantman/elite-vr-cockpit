using UnityEngine;

namespace EVRC
{
    [RequireComponent(typeof(IWristObject))]
    public class WristTurnActivate : MonoBehaviour
    {
        private float zLow = 100.0f;
        private float zHigh = 170.0f;
        private float yLow = 235.0f;
        private float yHigh = 295.0f;
        public GameObject target;
        private IWristObject wristObject;

        void Awake()
        {
            wristObject = target.GetComponent<IWristObject>();

            if(wristObject == null) 
            {
                Debug.LogError("The targeted GameObject must have an IWristObject component attached.");
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