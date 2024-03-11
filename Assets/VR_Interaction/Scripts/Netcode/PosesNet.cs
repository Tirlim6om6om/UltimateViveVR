using System;
using HTC.UnityPlugin.Vive;
using Mirror;
using UnityEngine;

namespace BCS.CORE.VR.Network
{
    /// <summary>
    /// Класс, хранящий локальный трекер с нетворк частью тела
    /// </summary>
    [Serializable]
    public class PosesNet
    {
        public Transform posLocal;
        public GameObject posNet;
        public BodyRole bodyRole = BodyRole.Invalid;

        [HideInInspector] public PlayerRigNetwork playerRig;

        public void SetTarget()
        {
            posNet.GetComponent<NetworkTransformBase>().target = posLocal;
        }
        
        public void OnChangeActive(bool active){
            playerRig.SetActiveToObj(bodyRole, active);
        }
    }
}