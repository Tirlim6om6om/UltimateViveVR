using System;
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
        [HideInInspector] public PlayerRigController playerRigController;

        public void Sync()
        {
            posNet.GetComponent<NetworkTransformBase>().target = posLocal;
        }
        
        public void OnChangeActive(bool active){
            if (posNet.TryGetComponent(out RoleTrackerNetwork roleSet))
            {
                playerRigController.SetActiveToObj(roleSet.bodyRole, active);
            }
        }
    }
}