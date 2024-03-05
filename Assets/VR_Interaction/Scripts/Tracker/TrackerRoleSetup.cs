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

        public readonly List<TrackerRoleState> trackersRole = new List<TrackerRoleState>();
        
        [Tooltip("Локальные трекеры частей тела (кроме рук)")]
        [SerializeField] private List<ViveRoleSetter> trackersLocal;
        
        private ViveRole.IMap _map;
        private readonly List<string> _modelNames = new List<string>();
        private TrackerRoleBase _trackerRoleBase;

        private void Awake()
        {
            _trackerRoleBase = TrackerRoleDeterminant.GetTrackerRoleFramework(gameObject);
            foreach (var tracker in trackersLocal)
            {
                TrackerRoleState trackerRoleState 
                    = new TrackerRoleState(tracker.gameObject, (BodyRole) tracker.viveRole.roleValue);
                trackerRoleState.SetActive(false);
                trackersRole.Add(trackerRoleState);
            }
            _map = ViveRole.GetMap<BodyRole>();
            _trackerRoleBase.Init();
            _trackerRoleBase.OnReady += Setup;
        }

        private void Setup()
        {
            DebugVR.Log("START SETUP TRACKERS");
            for (uint deviceIndex = 0; deviceIndex < VRModule.GetDeviceStateCount(); ++deviceIndex)
            {
                if (VRModule.GetCurrentDeviceState(deviceIndex).isConnected)
                {
                    OnDeviceConnected(deviceIndex, true);
                }
            }
            VRModule.onDeviceConnected += OnDeviceConnected;
            _trackerRoleBase.OnReady -= Setup;
        }

        private void OnDeviceConnected(uint deviceIndex, bool connected)
        {
            IVRModuleDeviceState device = VRModule.GetCurrentDeviceState(deviceIndex);
            if (connected)
            {
                DebugVR.Log("Connect: " + device.modelNumber);
                SetRole(device, _trackerRoleBase.GetTrackerRoleFromName(device.modelNumber));
            }
            else
            {
                string modelName = GetModelOfLostTracker();
                BodyRole role = GetBodyRoleByModel(modelName);
                
                DebugVR.Log("Disconnect: " + modelName);
                DeleteRole(modelName, role);
            }
        }

        #region Methods of searching
        private string GetModelOfLostTracker()
        {
            List<string> currentModelsName = new List<string>();
            for (uint deviceIndex = 0; deviceIndex < VRModule.GetDeviceStateCount(); ++deviceIndex)
            {
                currentModelsName.Add(VRModule.GetCurrentDeviceState(deviceIndex).modelNumber);
            }
            
            foreach (var modelName in _modelNames)
            {
                if (currentModelsName.Contains(modelName))
                {
                    return modelName;
                }
            }
            
            return "";
        }

        private BodyRole GetBodyRoleByModel(string modelName)
        {
            foreach (var tracker in trackersRole)
            {
                if (tracker.GetActive() && tracker.modelName == modelName)
                {
                    return tracker.role;
                }
            }
            
            return BodyRole.Invalid;
        }
        #endregion

        #region Role control methods
        private void SetRole(IVRModuleDeviceState device, BodyRole role)
        {
            _map.BindDeviceToRoleValue(device.serialNumber, (int) role);
            DebugVR.Log($"Device: {device.serialNumber} role: {role}");
            _modelNames.Add(device.modelNumber);
            foreach (var tracker in trackersRole)
            {
                if (tracker.role == role)
                {
                    tracker.SetActive(true);
                    tracker.modelName = device.modelNumber;
                    break;
                }
            }
        }

        private void DeleteRole(string modelName, BodyRole role)
        {
            _map.UnbindRoleValue((int)role);
            _modelNames.Remove(modelName);
            foreach (var tracker in trackersRole)
            {
                if (tracker.modelName == modelName)
                {
                    tracker.SetActive(false);
                    break;
                }
            }
        }
        #endregion

#if  UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                trackersRole[0].SetActive(!trackersRole[0].GetActive());
            }
        }
#endif
    }
}