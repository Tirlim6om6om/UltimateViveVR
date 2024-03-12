#if VIU_OPENVR_SUPPORT
using HTC.UnityPlugin.Vive;
using HTC.UnityPlugin.VRModuleManagement;
using Valve.VR;

namespace BCS.CORE.VR
{
    public class TrackerRoleSteam : TrackerRoleBase
    {
        //16 - максимум девайсов в steamVR
        private const uint countDevices = 16;

        public override bool IsReady()
        {
            return SteamVR.initializedState == SteamVR.InitializedStates.InitializeSuccess;
        }

        public override BodyRole GetTrackerRoleFromName(IVRModuleDeviceState device)
        {
            string type;
            string modelNameSteam;
            for (uint i = 0; i < countDevices; ++i)
            {
                modelNameSteam = SteamVR.instance.GetStringProperty(ETrackedDeviceProperty.Prop_ModelNumber_String, i);
                
                if (modelNameSteam == device.modelNumber)
                {
                    type = SteamVR.instance.GetStringProperty(ETrackedDeviceProperty.Prop_ControllerType_String, i);
                    DebugVR.Log(type);
                    switch (type)
                    {
                        case "vive_tracker_chest":
                            return BodyRole.Chest;
                        case "vive_tracker_right_foot":
                            return BodyRole.RightFoot;
                        case "vive_tracker_left_foot":
                            return BodyRole.LeftFoot;
                        case "vive_tracker_right_knee":
                            return BodyRole.RightKnee;
                        case "vive_tracker_left_knee":
                            return BodyRole.LeftKnee;
                        case "vive_tracker_waist":
                            return BodyRole.Hip;
                        default:
                            return BodyRole.Invalid;
                    }
                }
            }
            return BodyRole.Invalid;
        }
    }
}
#endif