using System;
using System.Collections;
using HTC.UnityPlugin.Vive;
using HTC.UnityPlugin.VRModuleManagement;
using UnityEngine;
#if VIU_OPENVR_SUPPORT
using HTC.UnityPlugin.Vive.SteamVRExtension;
using Valve.VR;
#endif
using Wave.Essence.Tracker;
using TrackerRole = Wave.Essence.Tracker.TrackerRole;

public class TrackerRoleSetup : MonoBehaviour
{
    private ViveRole.IMap _map;
    
    private void Start()
    {
        _map = ViveRole.GetMap<BodyRole>();
        if (VRModule.isWaveVRSupported && TrackerManager.Instance == null)
        {
            gameObject.AddComponent<TrackerManager>().InitialStartTracker = true;
        }
        StartCoroutine(Wait());
    }

    private IEnumerator Wait()
    {
        yield return new WaitForEndOfFrame();
        if (VRModule.isWaveVRSupported)
        {
            yield return new WaitUntil(() => TrackerManager.Instance);
        }
#if VIU_OPENVR_SUPPORT
        yield return new WaitUntil(() => SteamVR.initializedState == SteamVR.InitializedStates.InitializeSuccess);
#endif
        DebugVR.Log("Start");
        for (uint deviceIndex = 0, imax = VRModule.GetDeviceStateCount(); deviceIndex < imax; ++deviceIndex)
        {
            if (VRModule.GetCurrentDeviceState(deviceIndex).isConnected)
            {
                OnDeviceConnected(deviceIndex, true);
            }
        }
        VRModule.onDeviceConnected += OnDeviceConnected;
    }

    private void OnDeviceConnected(uint deviceIndex, bool connected)
    {
        var device = VRModule.GetCurrentDeviceState(deviceIndex);
        DebugVR.Log("Add:" + deviceIndex  + " : " + device.serialNumber);
        if (device.isConnected)
        {
            if(device.deviceClass != VRModuleDeviceClass.GenericTracker)
                return;
            if (VRModule.isWaveVRSupported)
            {
                if (!GetTrackerRoleFromNameWiwe(device.modelNumber.Split(' ')[0], out BodyRole role))
                    return;
                SetRole(device, role);
            }
#if VIU_OPENVR_SUPPORT
            if (VRModule.isOpenVRSupported)
            {
                if(!GetTrackerRoleFromNameSteamVR(device.modelNumber, out BodyRole role))
                    return;
                SetRole(device, role);
            }
#endif
        }
    }
    
    bool GetTrackerRoleFromNameWiwe(string nameTracker, out BodyRole role)
    {
        role = BodyRole.Chest;
        for (int i = 0; i < TrackerUtils.s_TrackerIds.Length; i++)
        {
            TrackerManager.Instance.GetTrackerDeviceName(TrackerUtils.s_TrackerIds[i], out string nameDevice);
            if (nameDevice == nameTracker)
            {
                DebugVR.Log(nameDevice + " == " + nameTracker);
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

#if VIU_OPENVR_SUPPORT
    bool GetTrackerRoleFromNameSteamVR(string serial, out BodyRole role)
    {
        role = BodyRole.Hip;
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
    
    public void SetRole(IVRModuleDeviceState device, BodyRole role)
    {
        _map.BindDeviceToRoleValue(device.serialNumber, (int)role);
        DebugVR.Log("Device: " + device.serialNumber + " role: " + role + "\n");
    }
}
