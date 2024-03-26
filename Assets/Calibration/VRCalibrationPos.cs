using BCS.CORE.VR;
using HTC.UnityPlugin.VRModuleManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRCalibrationPos : MonoBehaviour
{
    [SerializeField] private Calibration calibration;
    [SerializeField] private Transform head;

    private bool clicked;

    private void Update()
    {
        var deviceState = VRModule.GetDeviceState(VRModule.GetLeftControllerDeviceIndex());

#if UNITY_EDITOR
        //DebugVR.Log("Pos = " + VRModule.GetDeviceState(0).pose.pos);
        if (Input.GetKeyDown(KeyCode.M))
        {
            StartCalibr();
        }
#endif

        if (deviceState.GetButtonPress(VRModuleRawButton.Grip))
        {
            if (!clicked)
            {
                StartCalibr();
                clicked = true;
            }
        }
        else
        {
            clicked = false;
        }
    }

    private void StartCalibr()
    {
        calibration.Calibrate(head);
    }
}
