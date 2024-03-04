using UnityEngine;

namespace BCS.CORE.VR
{
    public class TrackerRoleDeterminant : MonoBehaviour
    {
        public static TrackerRoleBase GetTrackerRoleFramework(GameObject obj)
        {
#if VIU_WAVEVR_SUPPORT
            return obj.AddComponent<TrackerRoleWawe>();
#endif
#if VIU_OPENVR_SUPPORT
            return obj.AddComponent<TrackerRoleSteam>();
#endif
            return null;
        }
    }
}