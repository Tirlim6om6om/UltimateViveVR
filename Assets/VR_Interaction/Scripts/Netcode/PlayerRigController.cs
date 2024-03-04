using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;
using Mirror;
using UnityEngine;

namespace BCS.CORE.VR.Network
{
    /// <summary>
    /// Синхронизация нетворк и локального игрока
    /// </summary>
    public class PlayerRigController : NetworkBehaviour
    {
        [SerializeField] private GameObject playerLocal;
        [SerializeField] private List<PosesNet> poses;
        [SerializeField] private List<GameObject> visuals;
        [SerializeField] private TrackerRoleSetup trackerRoleSetup;
        private readonly SyncList<BodyActive> _bodyActives = new SyncList<BodyActive>();

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
                    if (pose.posNet.TryGetComponent(out RoleTrackerNetwork roleSet))
                    {
                        DebugVR.Log("Bodies: " + _bodyActives.Count);

                        int id = GetRole(roleSet.bodyRole);
                        if (id < _bodyActives.Count)
                        {
                            pose.posNet.SetActive(_bodyActives[GetRole(roleSet.bodyRole)].active);
                        }
                    }
                }
                return;
            }

            playerLocal.SetActive(isLocalPlayer);
            foreach (var visual in visuals)
            {
                Destroy(visual);
            }
            visuals.Clear();
            
            foreach (var pose in poses)
            {
                pose.SetTarget();
                if (pose.posNet.TryGetComponent(out RoleTrackerNetwork roleSet))
                {
                    _bodyActives.Add(new BodyActive((int)roleSet.bodyRole, true));   
                }

                if (pose.posNet.TryGetComponent(out NetworkTransformBase transformBase))
                {
                    if (isServer)
                    {
                        transformBase.syncDirection = SyncDirection.ServerToClient;
                        transformBase.syncMode = SyncMode.Observers;
                    }
                    else
                    {
                        transformBase.syncDirection = SyncDirection.ClientToServer;
                    }
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
                if (pose.posNet.TryGetComponent(out RoleTrackerNetwork roleSetter))
                {
                    if (roleSetter.bodyRole == role)
                    {
                        DebugVR.Log("SetActive RPC: " + pose.posNet.name + " : " + active);
                        pose.posNet.SetActive(active);
                        if(isOwned)
                            _bodyActives[GetRole(role)] = new BodyActive((int) role, active);
                    }
                }
            }
        }

        private int GetRole(BodyRole role)
        {
            for (int i = 0; i < _bodyActives.Count; i++)
            {
                if ((BodyRole) _bodyActives[i].role == role)
                {
                    return i;
                }
            }
            return 0;
        }
    }
}