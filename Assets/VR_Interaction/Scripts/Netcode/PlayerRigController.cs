using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Mirror;
using UnityEngine;

namespace BCS.CORE.VR.Network
{
    [Serializable]
    public class PosesNet
    {
        [SerializeField] private Transform posLocal;
        [SerializeField] private NetworkTransformReliable posNet;

        public void Sync()
        {
            posNet.target = posLocal;
        }
    }
    
    public class PlayerRigController : NetworkBehaviour
    {
        [SerializeField] private List<PosesNet> poses;
        [SerializeField] private List<GameObject> visuals;

        private void Start()
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
        }
    }
}