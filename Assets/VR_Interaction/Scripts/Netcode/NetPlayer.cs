using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace BCS.CORE.VR.Network
{
    /// <summary>
    /// Определение локального игрока и хоста
    /// </summary>
    public class NetPlayer : NetworkBehaviour
    {
        [SerializeField] private GameObject playerLocal;
        [SerializeField] private List<GameObject> transformNetworks;

        public override void OnStartAuthority()
        {
            List<NetworkTransformBase> transformBases = new List<NetworkTransformBase>();
            foreach (var transformObj in transformNetworks)
            {
                if (transformObj.TryGetComponent(out NetworkTransformBase transformBase))
                {
                    transformBases.Add(transformBase);
                }
            }
            
            foreach (var component in transformBases)
            {
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
        }

        private void Start()
        {
            playerLocal.SetActive(isLocalPlayer);
        }

        public override void OnStopAuthority()
        {
            enabled = false;
        }
    }
}