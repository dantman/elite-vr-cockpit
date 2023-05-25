using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.XR.Management;
using Valve.VR;

namespace EVRC.Core.Overlay
{
    /// <summary>
    /// Connects/Disconnects to Open VR Runtimes. Starts SteamVR in Standalone Mode.
    /// </summary>
    /// <remarks>
    /// Effectively starts and stops the whole overlay.
    /// </remarks>
    public class OpenVrRuntimeConnector : MonoBehaviour
    {
        public OpenVrState openVrState;

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
            // Double check that the loader isn't running already
            DeinitializeLoader();

            Init();
            SteamVR_Events.System(EVREventType.VREvent_Quit).Listen(OnQuit);
        }

        void OnDisable()
        {
            Shutdown();
            SteamVR_Events.System(EVREventType.VREvent_Quit).Remove(OnQuit);
            DeinitializeLoader();
        }

        void OnQuit(VREvent_t ev)
        {
            enabled = false;
            openVrState.running = false;

            Debug.Log("OpenVR Quit event received, quitting");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
		    Application.Quit();
#endif
        }

        private void Init()
        {
            StartCoroutine(ConnectToVRRuntime(() => openVrState.gameEvent.Raise(true)));
            // if (!ConnectToVRRuntime())
            // {
            //     enabled = false;
            //     openVrState.running = false;
            //     return false;
            // }

            openVrState.running = true;
        }

        public void Shutdown()
        {            
            openVrState.running = false;
            openVrState.gameEvent.Raise(false);
            DisconnectFromVRRuntime();
        }

        private IEnumerator ConnectToVRRuntime(System.Action callback)
        {
            // This is done instead of using "Initialize XR on Startup" in Unity
            Debug.Log("Initializing XR Loader");
            yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

            // Check if initialization is complete
            if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
            {
                // Start XR and puts application into XR Mode
                Debug.Log("XR Loader Successfully initialized - starting subsystems");
                XRGeneralSettings.Instance.Manager.StartSubsystems();
                yield return null;


                // Starts SteamVR
                Debug.Log("Initializing SteamVR Runtime");
                SteamVR.InitializeStandalone(EVRApplicationType.VRApplication_Overlay);
                yield return null;

                callback?.Invoke();
            }
            else
            {
                Debug.LogError("XR Loader Initialization failed!");
            }            
                        
        }

        private void DisconnectFromVRRuntime()
        {
            SteamVR_Events.Initialized.Send(false);
            OpenVR.Shutdown();
            SteamVR.SafeDispose();

            DeinitializeLoader();
        }

        private void DeinitializeLoader()
        {
            if (XRGeneralSettings.Instance.Manager.activeLoader != null)
            {
                XRGeneralSettings.Instance.Manager.StopSubsystems();
                XRGeneralSettings.Instance.Manager.DeinitializeLoader();
            }
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
