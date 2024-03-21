using BCS.CORE.VR;
using HTC.UnityPlugin.VRModuleManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    private bool _spawned;

    private void Update()
    {
        var deviceState = VRModule.GetDeviceState(VRModule.GetRightControllerDeviceIndex());

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.RightAlt))
        {
            DebugVR.instance.gameObject.SetActive(!DebugVR.instance.gameObject.activeSelf);
        }
#endif

        if (deviceState.GetButtonPress(VRModuleRawButton.A))
        {
            if (!_spawned)
            {
                DebugVR.instance.gameObject.SetActive(!DebugVR.instance.gameObject.activeSelf);
                _spawned = true;
            }
        }
        else
        {
            _spawned = false;
        }
    }
}
