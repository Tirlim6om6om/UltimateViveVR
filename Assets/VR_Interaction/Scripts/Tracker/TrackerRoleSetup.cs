using System;
using System.Collections;
using System.Collections.Generic;
using BCS.CORE.VR.Network.Example;
using HTC.UnityPlugin.Vive;
using HTC.UnityPlugin.VRModuleManagement;
using UnityEngine;
using UnityEngine.Events;
#if VIU_OPENVR_SUPPORT
using HTC.UnityPlugin.Vive.SteamVRExtension;
using Valve.VR;
#endif
#if VIU_WAVEVR_SUPPORT
using Wave.Essence.Tracker;
using TrackerRole = Wave.Essence.Tracker.TrackerRole;
#endif

namespace BCS.CORE.VR
{
    public class TrackerRoleState
    {
        public BodyRole role;
        public UnityEvent<bool> changeActive = new UnityEvent<bool>();
        public string modelNumber;
        private GameObject obj;
        
        public TrackerRoleState(GameObject obj, BodyRole role)
        {
            this.obj = obj;
            this.role = role;
        }

        public void SetActive(bool active)
        {
            obj.SetActive(active);
            changeActive?.Invoke(active);
        }

        public bool GetActive() => obj.activeSelf;
    }
    
    public class TrackerRoleSetup : MonoBehaviour
    {
        [HideInInspector] public bool done;
        public List<TrackerRoleState> trackersRole = new List<TrackerRoleState>();
        [SerializeField] private List<ViveRoleSetter> trackers;
        private ViveRole.IMap _map;
        private List<string> _modelsNumber = new List<string>();

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                trackersRole[0].SetActive(true);
            }
        }

        private void Awake()
        {
            foreach (var tracker in trackers)
            {
                var trackerRoleState = new TrackerRoleState(tracker.gameObject, (BodyRole) tracker.viveRole.roleValue);
                trackerRoleState.SetActive(false);
                trackersRole.Add(trackerRoleState);
            }
        }
        
        private void Start()
        {
            _map = ViveRole.GetMap<BodyRole>();
#if VIU_WAVEVR_SUPPORT
            if (VRModule.isWaveVRSupported && TrackerManager.Instance == null)
            {
                gameObject.AddComponent<TrackerManager>().InitialStartTracker = true;
            }
            StartCoroutine(WaitInit());
        }
#endif
        private IEnumerator WaitInit()
        {
            yield return new WaitForEndOfFrame();
#if VIU_WAVEVR_SUPPORT
            if (VRModule.isWaveVRSupported)
            {
                yield return new WaitUntil(() => TrackerManager.Instance);
            }
#endif
#if VIU_OPENVR_SUPPORT
        yield return new WaitUntil(() => SteamVR.initializedState == SteamVR.InitializedStates.InitializeSuccess);
#endif
            DebugVR.Log("START SETUP TRACKERS");
            for (uint deviceIndex = 0, imax = VRModule.GetDeviceStateCount(); deviceIndex < imax; ++deviceIndex)
            {
                if (VRModule.GetCurrentDeviceState(deviceIndex).isConnected)
                {
                    OnDeviceConnected(deviceIndex, true);
                }
            }
            done = true;
            VRModule.onDeviceConnected += OnDeviceConnected;
        }

        private void OnDeviceConnected(uint deviceIndex, bool connected)
        {
            var device = VRModule.GetCurrentDeviceState(deviceIndex);
            if (connected)
            {
                DebugVR.Log("Connected: " + device.modelNumber);
#if VIU_WAVEVR_SUPPORT
                if (VRModule.isWaveVRSupported)
                {
                    if (!GetTrackerRoleFromNameWawe(device.modelNumber.Split(' ')[0], out BodyRole role))
                        return;
                    SetRole(device, role);
                }
#endif
#if VIU_OPENVR_SUPPORT
                if (VRModule.isOpenVRSupported)
                {
                    if(!GetTrackerRoleFromNameSteamVR(device.modelNumber, out BodyRole role))
                        return;
                    SetRole(device, role);
                }
#endif
            }
            else
            {
                string modelNumber = GetModelOfLostTracker();
                DebugVR.Log("Disconnect: " + modelNumber);
                BodyRole role = GetBodyRoleByModel(modelNumber);
                if (VRModule.isWaveVRSupported)
                {
                    DeleteRole(modelNumber, role);
                }
#if VIU_OPENVR_SUPPORT
                if (VRModule.isOpenVRSupported)
                {
                     DeleteRole(modelNumber, role);
                }
#endif
            }
        }

        private string GetModelOfLostTracker()
        {
            
            foreach (var serial in _modelsNumber)
            {
                bool found = false;
                for (uint deviceIndex = 0, imax = VRModule.GetDeviceStateCount(); deviceIndex < imax; ++deviceIndex)
                {
                    if (serial == VRModule.GetCurrentDeviceState(deviceIndex).modelNumber)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    return serial;
            }

            return "";
        }

        private BodyRole GetBodyRoleByModel(string modelNumber)
        {
            foreach (var tracker in trackersRole)
            {
                if (tracker.modelNumber == modelNumber)
                    return tracker.role;
            }

            return BodyRole.Invalid;
        }
        
#if VIU_WAVEVR_SUPPORT
        bool GetTrackerRoleFromNameWawe(string nameTracker, out BodyRole role)
        {
            role = BodyRole.Chest;
            for (int i = 0; i < TrackerUtils.s_TrackerIds.Length; i++)
            {
                TrackerManager.Instance.GetTrackerDeviceName(TrackerUtils.s_TrackerIds[i], out string nameDevice);
                if (nameDevice == nameTracker)
                {
                    TrackerRole trackerRole = TrackerManager.Instance.GetTrackerRole(TrackerUtils.s_TrackerIds[i]);
                    switch (trackerRole)
                    {
                        case TrackerRole.Foot_Left:
                            role = BodyRole.LeftFoot;
                            break;
                        case TrackerRole.Foot_Right:
                            role = BodyRole.RightFoot;
                            break;
                        case TrackerRole.Chest:
                            role = BodyRole.Chest;
                            break;
                        case TrackerRole.Knee_Right:
                            role = BodyRole.RightKnee;
                            break;
                        case TrackerRole.Knee_Left:
                            role = BodyRole.LeftKnee;
                            break;
                        default:
                            return false;
                    }

                    return true;
                }
            }

            return false;
        }
#endif

#if VIU_OPENVR_SUPPORT
    bool GetTrackerRoleFromNameSteamVR(string serial, out BodyRole role)
    {
        role = BodyRole.Hip;
        uint deviceIndex = SteamVR_Actions.default_Pose.GetDeviceIndex(SteamVR_Input_Sources.LeftFoot);
        for (uint i = 0; i < 15; i++)
        {
            if (SteamVR.instance.GetStringProperty(ETrackedDeviceProperty.Prop_ModelNumber_String, i) == serial)
            {
                string type = SteamVR.instance.GetStringProperty(ETrackedDeviceProperty.Prop_ControllerType_String, i);
                switch (type)
                {
                    case "vive_tracker_chest":
                        role = BodyRole.Chest;
                        break;
                    case "vive_tracker_right_foot":
                        role = BodyRole.RightFoot;
                        break;
                    case "vive_tracker_left_foot":
                        role = BodyRole.LeftFoot;
                        break;
                    case "vive_tracker_right_knee":
                        role = BodyRole.RightKnee;
                        break;
                    case "vive_tracker_left_knee":
                        role = BodyRole.LeftKnee;
                        break;
                    case "vive_tracker_hip":
                        role = BodyRole.Hip;
                        break;
                    default:
                        return false;
                }
                return true;
            }
        }
        return false;
    }
#endif

        private void SetRole(IVRModuleDeviceState device, BodyRole role)
        {
            _map.BindDeviceToRoleValue(device.serialNumber, (int) role);
            DebugVR.Log("Device: " + device.serialNumber + " role: " + role + "\n");
            _modelsNumber.Add(device.modelNumber);
            foreach (var tracker in trackersRole)
            {
                if (tracker.role == role)
                {
                    tracker.SetActive(true);
                    tracker.modelNumber = device.modelNumber;
                    break;
                }
            }
        }

        private void DeleteRole(string modelNumber, BodyRole role)
        {
            _map.UnbindRoleValue((int)role);
            _modelsNumber.Remove(modelNumber);
            foreach (var tracker in trackersRole)
            {
                if (tracker.modelNumber == modelNumber)
                {
                    tracker.SetActive(false);
                    break;
                }
            }
        }
    }
}