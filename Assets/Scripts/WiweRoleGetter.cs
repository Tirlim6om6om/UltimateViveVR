using System;
using HTC.UnityPlugin.Vive;
using HTC.UnityPlugin.VRModuleManagement;
using UnityEngine;
using Wave.Essence.Tracker;
using TrackerRole = Wave.Essence.Tracker.TrackerRole;

public class WiweRoleGetter : MonoBehaviour
{
    private ViveRole.IMap _map;
    
    private void Awake()
    {
        //TODO проверка что это Wiwe
        
    }

    private void Start()
    {
        _map = ViveRole.GetMap<BodyRole>();
        for (uint deviceIndex = 0, imax = VRModule.GetDeviceStateCount(); deviceIndex < imax; ++deviceIndex)
        {
            if (VRModule.GetCurrentDeviceState(deviceIndex).isConnected)
            {
                OnDeviceConnected(deviceIndex, true);
            }
        }
        VRModule.onDeviceConnected += OnDeviceConnected;
        DebugVR.Log("Start");
    }

    private void RefreshRoles(uint deviceIndex, bool connected)
    {
        
        for (uint index = 0, imax = VRModule.GetDeviceStateCount(); deviceIndex < imax; ++deviceIndex)
        {
            var device = VRModule.GetCurrentDeviceState(index);
            if (device.isConnected)
            {
                
            }
        }
    }
    
    private void OnDeviceConnected(uint deviceIndex, bool connected)
    {
        var device = VRModule.GetCurrentDeviceState(deviceIndex);
        DebugVR.Log("Add:" + deviceIndex  + " : " + device.serialNumber);
        if (device.isConnected)
        {
            if(device.deviceClass == VRModuleDeviceClass.Controller)
                return;
            GetTrackerIDFromName(device.modelNumber.Split(' ')[0], out TrackerId index);
            //TrackerRole role = TrackerManager.Instance.GetTrackerRole(TrackerUtils.s_TrackerIds[device.deviceIndex]);
            TrackerRole role = TrackerManager.Instance.GetTrackerRole(index);
            DebugVR.Log("Try add role: " + (TrackerId)deviceIndex + " + " + role);
            switch (role)
            {
                case TrackerRole.Foot_Left:
                    SetRole(device, BodyRole.LeftFoot);
                    break;
                case TrackerRole.Foot_Right:
                    SetRole(device, BodyRole.RightFoot);
                    break;
                case TrackerRole.Chest:
                    SetRole(device, BodyRole.Hip);
                    break;
            }
        }
    }
    
    bool GetTrackerIDFromName(string nameTracker, out TrackerId id)
    {
        id = TrackerId.Tracker0;

        for (int i = 0; i < TrackerUtils.s_TrackerIds.Length; i++)
        {
            TrackerManager.Instance.GetTrackerDeviceName(TrackerUtils.s_TrackerIds[i], out string nameDevice);
            if (nameDevice == nameTracker)
            {
                DebugVR.Log(nameDevice + " == " + nameTracker);
                id = TrackerUtils.s_TrackerIds[i];
                return true;
            }
        }
        return false;
    }

    public void SetRole(IVRModuleDeviceState device, BodyRole role)
    {
        _map.BindDeviceToRoleValue(device.serialNumber, (int)role);
        DebugVR.Log("Device: " + device.serialNumber + " role: " + role + "\n");
    }
}
