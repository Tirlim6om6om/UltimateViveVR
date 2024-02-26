using System;
using System.Collections;
using HTC.UnityPlugin.Vive;
using HTC.UnityPlugin.Vive.SteamVRExtension;
using HTC.UnityPlugin.VRModuleManagement;
using UnityEngine;
using Valve.VR;
using Wave.Essence.Tracker;
using TrackerRole = Wave.Essence.Tracker.TrackerRole;

public class WiweRoleGetter : MonoBehaviour
{
    private ViveRole.IMap _map;
    
    private enum TypeTracker
    {
        Foot_Left,
        Foot_Right,
        Chest
    }
    
    private void Awake()
    {
        //TODO проверка что это Wiwe
    }

    private void Start()
    {
        _map = ViveRole.GetMap<BodyRole>();
        StartCoroutine(Wait());
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(1);
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

    private void OnDeviceConnected(uint deviceIndex, bool connected)
    {
        var device = VRModule.GetCurrentDeviceState(deviceIndex);
        DebugVR.Log("Add:" + deviceIndex  + " : " + device.serialNumber);
        if (device.isConnected)
        {
            if(device.deviceClass == VRModuleDeviceClass.Controller)
                return;
            if (VRModule.isWaveVRSupported)
            {
                GetTrackerIDFromNameWiwe(device.modelNumber.Split(' ')[0], out TrackerId index);
                TrackerRole role = TrackerManager.Instance.GetTrackerRole(index);
                DebugVR.Log("Try add role: " + (TrackerId) deviceIndex + " + " + role);
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

            if (VRModule.isSteamVRPluginDetected)
            {
                if(device.deviceClass != VRModuleDeviceClass.GenericTracker)
                    return;
                if(!GetTrackerIDFromNameSteamVR(device.modelNumber, out TypeTracker role))
                    return;
                
                switch (role)
                {
                    case TypeTracker.Foot_Left:
                        SetRole(device, BodyRole.LeftFoot);
                        break;
                    case TypeTracker.Foot_Right:
                        SetRole(device, BodyRole.RightFoot);
                        break;
                    case TypeTracker.Chest:
                        SetRole(device, BodyRole.Hip);
                        break;
                }
            }
        }
    }
    
    bool GetTrackerIDFromNameWiwe(string nameTracker, out TrackerId id)
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


    bool GetTrackerIDFromNameSteamVR(string serial, out TypeTracker role)
    {
        role = TypeTracker.Chest;
        uint deviceIndex = SteamVR_Actions.default_Pose.GetDeviceIndex(SteamVR_Input_Sources.LeftFoot);
        Debug.Log(OpenVR.System.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.LeftHand));
        for (uint i = 0; i < 15; i++)
        {
            if (SteamVR.instance.GetStringProperty(ETrackedDeviceProperty.Prop_ModelNumber_String, i) == serial)
            {
                string type = SteamVR.instance.GetStringProperty(ETrackedDeviceProperty.Prop_ControllerType_String, i);
                DebugVR.Log(type);
                switch (type)
                {
                    case "vive_tracker_chest":
                        role = TypeTracker.Chest;
                        break;
                    case "vive_tracker_right_foot":
                        role = TypeTracker.Foot_Right;
                        break;
                    case "vive_tracker_left_foot":
                        role = TypeTracker.Foot_Left;
                        break;
                }

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
