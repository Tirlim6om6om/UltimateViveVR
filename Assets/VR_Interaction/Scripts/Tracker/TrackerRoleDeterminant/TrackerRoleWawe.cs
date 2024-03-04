#if VIU_WAVEVR_SUPPORT
using HTC.UnityPlugin.Vive;
using Wave.Essence.Tracker;
using TrackerRole = Wave.Essence.Tracker.TrackerRole;

namespace BCS.CORE.VR
{
    public class TrackerRoleWawe : TrackerRoleBase
    {
        public override void Init()
        {
            gameObject.AddComponent<TrackerManager>().InitialStartTracker = true;
        }

        public override bool IsReady() => TrackerManager.Instance;
        
        public override bool GetTrackerRoleFromName(string modelNumber, out BodyRole role)
        {
            modelNumber = modelNumber.Split(' ')[0];
            role = BodyRole.Chest;
            for (int i = 0; i < TrackerUtils.s_TrackerIds.Length; i++)
            {
                TrackerManager.Instance.GetTrackerDeviceName(TrackerUtils.s_TrackerIds[i], out string nameDevice);
                if (nameDevice == modelNumber)
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
    }
}
#endif