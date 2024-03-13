using BCS.CORE.VR;
using HTC.UnityPlugin.VRModuleManagement;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCalibr : NetworkBehaviour
{
    [SerializeField] private Calibrator calibrator;
    [SerializeField] private Calibrator calibratorNetwork;
    [SerializeField] private GameObject model;
    [SerializeField] private GameObject modelNet;
    private bool _calibrated;

    private void Update()
    {
        var deviceState = VRModule.GetDeviceState(VRModule.GetLeftControllerDeviceIndex());

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            CallCalibrate();
        }
#endif

        if (deviceState.GetButtonPress(VRModuleRawButton.A))
        {
            if (!_calibrated)
            {
                CallCalibrate();
                _calibrated = true;
            }
        }
        else
        {
            _calibrated = false;
        }
    }

    private void CallCalibrate()
    {
        DebugVR.Log("Calibrate");
        model.SetActive(true);
        calibrator.Calibrate();
        CalibrCommand();
    }

    [Command(requiresAuthority = false)]
    private void CalibrCommand()
    {
        CalibrClient();
    }

    [ClientRpc]
    private void CalibrClient()
    {
        if (isOwned)
            return;
        calibratorNetwork.settings = calibrator.settings;
        calibratorNetwork.Calibrate();
        modelNet.SetActive(true);
    }
}
