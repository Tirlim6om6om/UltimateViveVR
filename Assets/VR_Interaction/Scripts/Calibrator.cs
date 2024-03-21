using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calibrator : MonoBehaviour
{
    [Tooltip("Reference to the VRIK component on the avatar.")] public VRIK ik;
    [Tooltip("The settings for VRIK calibration.")] public VRIKCalibrator.Settings settings;
    [Tooltip("The HMD.")] public Transform headTracker;
    [Tooltip("(Optional) A tracker placed anywhere on the body of the player, preferrably close to the pelvis, on the belt area.")] public Transform bodyTracker;
    [Tooltip("(Optional) A tracker or hand controller device placed anywhere on or in the player's left hand.")] public Transform leftHandTracker;
    [Tooltip("(Optional) A tracker or hand controller device placed anywhere on or in the player's right hand.")] public Transform rightHandTracker;
    [Tooltip("(Optional) A tracker placed anywhere on the ankle or toes of the player's left leg.")] public Transform leftFootTracker;
    [Tooltip("(Optional) A tracker placed anywhere on the ankle or toes of the player's right leg.")] public Transform rightFootTracker;

    [Header("Data stored by Calibration")]
    public VRIKCalibrator.CalibrationData data = new VRIKCalibrator.CalibrationData();

    public void Calibrate()
    {

        VRIKCalibrator.Calibrate(ik, 
            settings, 
            headTracker, 
            bodyTracker.gameObject.activeSelf ? bodyTracker : null, 
            leftHandTracker.gameObject.activeSelf ? leftHandTracker : null, 
            rightHandTracker.gameObject.activeSelf ? rightHandTracker : null, 
            leftFootTracker.gameObject.activeSelf ? leftFootTracker : null, 
            rightFootTracker.gameObject.activeSelf ? rightFootTracker : null);
    }
}
