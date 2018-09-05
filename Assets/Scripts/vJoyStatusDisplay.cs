using UnityEngine;

namespace EVRC
{
    using VJoyStatus = vJoyInterface.VJoyStatus;

    /**
     * Behaviour that toggles display of error panels depending on the vJoy status
     */
    public class vJoyStatusDisplay : MonoBehaviour
    {
        public GameObject notInstalled;
        public GameObject versionMismatch;
        public GameObject deviceUnavailable;
        public GameObject deviceOwned;
        public GameObject deviceError;
        public GameObject deviceMisconfigured;
        public GameObject deviceNotAquired;

        private void OnEnable()
        {
            vJoyInterface.VJoyStatusChange.Listen(OnStatusChange);
            Refresh(vJoyInterface.vJoyStatus);
        }

        private void OnDisable()
        {
            vJoyInterface.VJoyStatusChange.Remove(OnStatusChange);
        }

        private void OnStatusChange(VJoyStatus status)
        {
            Refresh(status);
        }

        private void Refresh(VJoyStatus status)
        {
            if (notInstalled)
            {
                notInstalled.SetActive(status == VJoyStatus.NotInstalled);
            }
            if (versionMismatch)
            {
                versionMismatch.SetActive(status == VJoyStatus.VersionMismatch);
            }
            if (deviceUnavailable)
            {
                deviceUnavailable.SetActive(status == VJoyStatus.DeviceUnavailable);
            }
            if (deviceOwned)
            {
                deviceOwned.SetActive(status == VJoyStatus.DeviceOwned);
            }
            if (deviceError)
            {
                deviceError.SetActive(status == VJoyStatus.DeviceError);
            }
            if (deviceMisconfigured)
            {
                deviceMisconfigured.SetActive(status == VJoyStatus.DeviceMisconfigured);
            }
            if (deviceNotAquired)
            {
                deviceNotAquired.SetActive(status == VJoyStatus.DeviceNotAquired);
            }
        }
    }
}
