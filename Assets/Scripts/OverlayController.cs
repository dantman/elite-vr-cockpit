using System.Text;
using UnityEngine;
using Valve.VR;

namespace EVRC
{
    public class OverlayController : MonoBehaviour
    {
        public string VRDriver
        {
            get
            {
                if (OpenVR.System == null)
                {
                    return "No Driver";
                }

                return GetStringTrackedDeviceProperty(ETrackedDeviceProperty.Prop_TrackingSystemName_String);
            }
        }

        private string VRDisplay
        {
            get
            {

                if (OpenVR.System == null)
                {
                    return "No Display";
                }

                return GetStringTrackedDeviceProperty(ETrackedDeviceProperty.Prop_SerialNumber_String);
            }
        }

        void OnEnable()
        {
            Init();
            SteamVR_Events.System(EVREventType.VREvent_Quit).Listen(OnQuit);
        }

        void OnDisable()
        {
            Shutdown();
            SteamVR_Events.System(EVREventType.VREvent_Quit).Remove(OnQuit);
        }

        void OnQuit(VREvent_t ev)
        {
            enabled = false;

            Debug.Log("OpenVR Quit event received, quitting");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
		    Application.Quit();
#endif
        }

        private bool Init()
        {
            if (!ConnectToVRRuntime())
            {
                enabled = false;
                return false;
            }

            Debug.Log("Connected to VR Runtime");
            Debug.Log(
                "VR Driver: " + VRDriver + "\n" +
                "VR Display: " + VRDisplay
            );

            return true;
        }

        public void Shutdown()
        {
            DisconnectFromVRRuntime();
        }

        private bool ConnectToVRRuntime()
        {
            var error = EVRInitError.None;
            OpenVR.Init(ref error, EVRApplicationType.VRApplication_Overlay);
            if (error != EVRInitError.None)
            {
                Debug.LogWarning(error);
                return false;
            }

            SteamVR_Events.Initialized.Send(true);

            return true;
        }

        private void DisconnectFromVRRuntime()
        {
            SteamVR_Events.Initialized.Send(false);
            OpenVR.Shutdown();
        }

        public string GetStringTrackedDeviceProperty(ETrackedDeviceProperty prop, uint deviceId = OpenVR.k_unTrackedDeviceIndex_Hmd)
        {
            var error = ETrackedPropertyError.TrackedProp_Success;
            // @todo Use a per-thread shared stringbuilder
            var result = new StringBuilder((int)OpenVR.k_unMaxPropertyStringSize);
            var capacity = OpenVR.System.GetStringTrackedDeviceProperty(deviceId, prop, result, OpenVR.k_unMaxPropertyStringSize, ref error);
            if (error == ETrackedPropertyError.TrackedProp_Success)
            {
                return result.ToString(0, (int)capacity - 1);
            }
            else
            {
                return "Error Getting String: " + error.ToString();
            }
        }
    }
}
