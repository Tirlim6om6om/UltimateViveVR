using System;
using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.VRModuleManagement;
using Mirror;
using UnityEngine;
using Wave.Essence;
using Wave.Native;

public class Spawner : NetworkBehaviour
{
    [SerializeField] private GameObject ball;
    private bool _spawned;
    
    
    
    [Command]
    public void Spawn(Vector3 pos)
    {
        GameObject spawnBall = Instantiate(ball, pos, Quaternion.identity);
        NetworkServer.Spawn(spawnBall);
    }

    private void Update()
    {
        var deviceState = VRModule.GetDeviceState(VRModule.GetRightControllerDeviceIndex());
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Spawn(transform.position);
        }
#endif
        if (deviceState.GetButtonPress(VRModuleRawButton.A))
        {
            if (!_spawned)
            {
                Spawn(transform.position);
                _spawned = true;
            }
        }
        else
        {
            _spawned = false;
        }
    }
}
