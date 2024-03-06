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
            base.Init();
            gameObject.AddComponent<TrackerManager>().InitialStartTracker = true;
        }

        public override bool IsReady()
        {
            return TrackerManager.Instance;
        }
        
        public override BodyRole GetTrackerRoleFromName(string modelNumber)
        {
            modelNumber = modelNumber.Split(' ')[0];
            for (int i = 0; i < TrackerUtils.s_TrackerIds.Length; ++i)
            {
                TrackerManager.Instance.GetTrackerDeviceName(TrackerUtils.s_TrackerIds[i], out string nameDevice);
                if (nameDevice == modelNumber)
                {
                    TrackerRole trackerRole = TrackerManager.Instance.GetTrackerRole(TrackerUtils.s_TrackerIds[i]);
                    switch (trackerRole)
                    {
                        case TrackerRole.Foot_Left:
                            return BodyRole.LeftFoot;
                        case TrackerRole.Foot_Right:
                            return BodyRole.RightFoot;
                        case TrackerRole.Chest:
                            return BodyRole.Chest;
                        case TrackerRole.Knee_Right:
                            return BodyRole.RightKnee;
                        case TrackerRole.Knee_Left:
                            return BodyRole.LeftKnee;
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