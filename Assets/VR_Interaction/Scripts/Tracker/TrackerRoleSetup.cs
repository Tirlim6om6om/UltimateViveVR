using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;
using HTC.UnityPlugin.VRModuleManagement;
using UnityEngine;

namespace BCS.CORE.VR
{
    /// <summary>
    /// Установка ролей трекеров
    /// </summary>
    public class TrackerRoleSetup : MonoBehaviour
    {
        [HideInInspector] public bool done;
        public List<TrackerRoleState> trackersRole = new List<TrackerRoleState>();
        [SerializeField] private List<ViveRoleSetter> trackers;
        private ViveRole.IMap _map;
        private List<string> _modelsNumber = new List<string>();
        private TrackerRoleBase _trackerRoleBase;

        private void Awake()
        {
            _trackerRoleBase = TrackerRoleDeterminant.GetTrackerRoleFramework(gameObject);
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
            _trackerRoleBase.Init();
            StartCoroutine(WaitInit());
        }
        
        private IEnumerator WaitInit()
        {
            yield return new WaitUntil(() => _trackerRoleBase.IsReady());
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
                if (!_trackerRoleBase.GetTrackerRoleFromName(device.modelNumber, out BodyRole role))
                    return;
                SetRole(device, role);
            }
            else
            {
                string modelNumber = GetModelOfLostTracker();
                DebugVR.Log("Disconnect: " + modelNumber);
                BodyRole role = GetBodyRoleByModel(modelNumber);
                DeleteRole(modelNumber, role);
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
                if (tracker.GetActive() && tracker.modelNumber == modelNumber)
                    return tracker.role;
            }
            return BodyRole.Invalid;
        }

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
        
#if  UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                trackersRole[0].SetActive(true);
            }
        }
#endif
    }
}