#if VIU_WAVEVR_SUPPORT
using HTC.UnityPlugin.Vive;
using HTC.UnityPlugin.VRModuleManagement;
using Wave.Essence.Tracker;
using TrackerRole = Wave.Essence.Tracker.TrackerRole;

namespace BCS.CORE.VR
{
    public class TrackerRoleWawe : TrackerRoleBase
    {
        public override void Init()
        {
            base.Init();
            gameObject.AddComponent<TrackerManager>().StartTracker();
        }


        public override bool IsReady()
        {
            return TrackerManager.Instance;
        }
    
        public override BodyRole GetTrackerRoleFromName(IVRModuleDeviceState device)
        {
            string nameTracker = device.serialNumber.Split(' ')[1];
            uint i = (uint)( nameTracker[nameTracker.Length - 1] - '0');
            DebugVR.Log(device.serialNumber + " ID: " + i);
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
}
#endif