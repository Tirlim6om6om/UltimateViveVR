using System;
using System.Collections;
using System.Collections.Generic;
using BCS.CORE.VR.Network.Example;
using HTC.UnityPlugin.Vive;
using JetBrains.Annotations;
using Mirror;
using UnityEngine;

namespace BCS.CORE.VR.Network
{
    [Serializable]
    public class PosesNet
    {
        [SerializeField] private Transform posLocal;
        public GameObject posNet;
        private PlayerRigController _playerRig;
        private BodyRole _role;
        
        public void Sync()
        {
            posNet.GetComponent<NetworkTransformBase>().target = posLocal;
        }

        public void SetTrackerRole(TrackerRoleSetup trackerRoleSetup, PlayerRigController playerRigController)
        {
            _playerRig = playerRigController;
            if (!posLocal.TryGetComponent(out VivePoseTracker roleSetter)) return;
            DebugVR.Log( ((BodyRole)roleSetter.viveRole.roleValue).ToString());
            _role = (BodyRole)roleSetter.viveRole.roleValue;
            foreach (var tracker in trackerRoleSetup._trackersRole)
            {
                if (tracker.role == _role)
                {
                    tracker.changeActive.AddListener(SetActive);
                    SetActive(tracker.GetActive());
                }
            }
        }
        
        private void SetActive(bool active)
        {
            _playerRig.SetActiveToObj(_role, active);
        }
    }

    public class PlayerRigController : NetworkBehaviour
    {
        [SerializeField] private List<PosesNet> poses;
        [SerializeField] private List<GameObject> visuals;
        [SerializeField] private TrackerRoleSetup trackerRoleSetup;

        public override void OnStartAuthority()
        {
            if(!isLocalPlayer)
                return;
            foreach (var visual in visuals)
            {
                Destroy(visual);
            }
            visuals.Clear();
            
            foreach (var pose in poses)
            {
                pose.Sync();
            }

            StartCoroutine(WaitTrackers());
        }

        private IEnumerator WaitTrackers()
        {
            yield return new WaitUntil(() => trackerRoleSetup.done);
            yield return new WaitForSeconds(0.1f);
            foreach (var pose in poses)
            {
                pose.SetTrackerRole(trackerRoleSetup, this);
            }
        }

        [TargetRpc]
        public void SetActiveToObj(BodyRole role, bool active)
        {
            foreach (var pos in poses)
            {
                if (pos.posNet.TryGetComponent(out RoleSet roleSetter))
                {
                    if (roleSetter.bodyRole == role)
                    {
                        DebugVR.Log("SetActive RPC: " + pos.posNet.name + " : " + active);
                        pos.posNet.SetActive(active);
                    }
                }
            }
        }
    }
}