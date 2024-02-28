using System;
using Mirror;
using UnityEngine;

namespace BCS.CORE.VR.Network
{
    public class NetPlayer : NetworkBehaviour
    {
        [SerializeField] private GameObject playerLocal;
        [SerializeField] private GameObject playerNetwork;
        private bool _local;
        
        
        public override void OnStartAuthority()
        {
            _local = true;
            // playerNetwork.SetActive(!_local);
            TryGetComponent(out NetworkTransformReliable component);
            if (isServer)
            {
                component.syncDirection = SyncDirection.ServerToClient;
                component.syncMode = SyncMode.Observers;
            }
            else
            {
                component.syncDirection = SyncDirection.ClientToServer;
            }
        }

        private void Start()
        {
            playerLocal.SetActive(isLocalPlayer);
        }

        public override void OnStopAuthority()
        {
            this.enabled = false;
            _local = false;
        }
    }
}