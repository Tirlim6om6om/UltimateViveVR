#if VIU_OPENVR_SUPPORT
using HTC.UnityPlugin.Vive;
using Valve.VR;

namespace BCS.CORE.VR
{
    public class TrackerRoleSteam : TrackerRoleBase
    {
        public override void Init()
        {
            //Инициализация STEAMVR - не требуется
        }

        public override bool IsReady() =>  SteamVR.initializedState == SteamVR.InitializedStates.InitializeSuccess;
        
        public override bool GetTrackerRoleFromName(string modelNumber, out BodyRole role)
        {
            role = BodyRole.Invalid;
            for (uint i = 0; i < 15; i++)
            {
                if (SteamVR.instance.GetStringProperty(ETrackedDeviceProperty.Prop_ModelNumber_String, i) == modelNumber)
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
    }
}
#endif