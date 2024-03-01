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
        public Transform posLocal;
        public GameObject posNet;
        [HideInInspector]public PlayerRigController playerRigController;

        public void Sync()
        {
            posNet.GetComponent<NetworkTransformBase>().target = posLocal;
        }
        
        public void OnChangeActive(bool active){
            if (posNet.TryGetComponent(out RoleSet roleSet))
            {
                playerRigController.SetActiveToObj(roleSet.bodyRole, active);
            }
        }
    }
    
    [Serializable]
    public struct BodyActive
    {
        public int role;
        public bool active;

        public BodyActive(int role, bool active)
        {
            this.role = role;
            this.active = active;
        }

        public override string ToString()
        {
            return "Role: " + role + " active: " + active;
        }
    }

    public class PlayerRigController : NetworkBehaviour
    {
        [SerializeField] private List<PosesNet> poses;
        [SerializeField] private List<GameObject> visuals;
        [SerializeField] private TrackerRoleSetup trackerRoleSetup;
        public SyncList<BodyActive> bodyActives = new SyncList<BodyActive>();

        public override void OnStartAuthority()
        {
            if (isServer)
            {
                syncDirection = SyncDirection.ServerToClient;
                syncMode = SyncMode.Observers;
            }
            else
            {
                syncDirection = SyncDirection.ClientToServer;
            }
        }

        public override void OnStartClient()
        {
            if (!isOwned)
            {
                foreach (var pose in poses)
                {
                    if (pose.posNet.TryGetComponent(out RoleSet roleSet))
                    {
                        DebugVR.Log("Bodies: " + bodyActives.Count.ToString());
                        int id = GetRole(roleSet.bodyRole);
                        if(id < bodyActives.Count)
                            pose.posNet.SetActive(bodyActives[GetRole(roleSet.bodyRole)].active);
                    }
                }
                return;
            }

            foreach (var visual in visuals)
            {
                Destroy(visual);
            }
            visuals.Clear();
            
            foreach (var pose in poses)
            {
                pose.Sync();
                if (pose.posNet.TryGetComponent(out RoleSet roleSet))
                {
                    bodyActives.Add(new BodyActive((int)roleSet.bodyRole, true));   
                }
            }
            StartCoroutine(WaitTrackers());
        }
        
        

        private IEnumerator WaitTrackers()
        {
            yield return new WaitUntil(() => trackerRoleSetup.done);
            foreach (var pose in poses)
            {
                if (pose.posLocal.TryGetComponent(out IViveRoleComponent roleSetter))
                {
                    DebugVR.Log(((BodyRole) roleSetter.viveRole.roleValue).ToString());
                    BodyRole role = (BodyRole) roleSetter.viveRole.roleValue;
                    foreach (var tracker in trackerRoleSetup.trackersRole)
                    {
                        if (tracker.role == role)
                        {
                            DebugVR.Log(((BodyRole) roleSetter.viveRole.roleValue +
                                         " : " + tracker.GetActive()));
                            SetActiveToObj(role, tracker.GetActive());
                            pose.playerRigController = this;
                            tracker.changeActive.AddListener(pose.OnChangeActive);
                        }
                    }
                }
            }
        }

        [Command(requiresAuthority = false)]
        public void SetActiveToObj(BodyRole role, bool active)
        {
            SetActiveToObjToClients(role, active);
        }
        
        [ClientRpc]
        private void SetActiveToObjToClients(BodyRole role, bool active)
        {
            foreach (var pose in poses)
            {
                if (pose.posNet.TryGetComponent(out RoleSet roleSetter))
                {
                    if (roleSetter.bodyRole == role)
                    {
                        DebugVR.Log("SetActive RPC: " + pose.posNet.name + " : " + active);
                        pose.posNet.SetActive(active);
                        if(isOwned)
                            bodyActives[GetRole(role)] = new BodyActive((int) role, active);
                    }
                }
            }
        }

        private int GetRole(BodyRole role)
        {
            for (int i = 0; i < bodyActives.Count; i++)
            {
                if ((BodyRole) bodyActives[i].role == role)
                {
                    return i;
                }
            }
            return 0;
        }
    }
}