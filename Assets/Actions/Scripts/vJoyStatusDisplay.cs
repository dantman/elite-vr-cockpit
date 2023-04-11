using UnityEngine;

namespace EVRC
{

    /**
     * Behaviour that toggles display of error panels depending on the vJoy status
     */
    public class vJoyStatusDisplay : MonoBehaviour
    {
        [Header("Status Panel GameObjects")]
        public GameObject notInstalled;
        public GameObject versionMismatch;
        public GameObject deviceUnavailable;
        public GameObject deviceOwned;
        public GameObject deviceError;
        public GameObject deviceMisconfigured;
        public GameObject deviceNotAquired;

        [Header("State Objects")]
        public VJoyState vJoystate;

        private void OnEnable()
        {
            //vJoyInterface.VJoyStatusChange.Listen(OnStatusChange);
            OnStatusChange();
        }

        private void OnDisable()
        {
            //vJoyInterface.VJoyStatusChange.Remove(OnStatusChange);
        }

        public void OnStatusChange()
        {
            VJoyStatus status = vJoystate.vJoyStatus;
            Refresh(status);
        }

        private void Refresh(VJoyStatus status)
        {
            notInstalled.SetActive(status == VJoyStatus.NotInstalled);   
            versionMismatch.SetActive(status == VJoyStatus.VersionMismatch);
            deviceUnavailable.SetActive(status == VJoyStatus.DeviceUnavailable);
            deviceOwned.SetActive(status == VJoyStatus.DeviceOwned);
            deviceError.SetActive(status == VJoyStatus.DeviceError);
            deviceMisconfigured.SetActive(status == VJoyStatus.DeviceMisconfigured);
            deviceNotAquired.SetActive(status == VJoyStatus.DeviceNotAquired);
        }
    }
}
