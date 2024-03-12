using BCS.CORE.VR;
using HTC.UnityPlugin.VRModuleManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCalibr : MonoBehaviour
{
    [SerializeField] private Calibrator calibrator;
    private bool _calibrated;

    private void Update()
    {
        var deviceState = VRModule.GetDeviceState(VRModule.GetLeftControllerDeviceIndex());

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            calibrator.Calibrate();
        }
#endif

        if (deviceState.GetButtonPress(VRModuleRawButton.A))
        {
            if (!_calibrated)
            {
                DebugVR.Log("Calibrate");
                calibrator.Calibrate();
                _calibrated = true;
            }
        }
        else
        {
            _calibrated = false;
        }
    }
}
