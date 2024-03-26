using System.Collections.Generic;
using HTC.UnityPlugin.Vive;
using Mirror;
using UnityEngine;

namespace BCS.CORE.VR.Network
{
    /// <summary>
    /// Настройка нетворк трекеров к ригу
    /// </summary>
    public class PlayerRigNetwork : NetworkBehaviour
    {
        [Tooltip("Локальный игрок")]
        [SerializeField] private GameObject playerLocal;

        [Tooltip("Позиции нетворк и локальный частей тела")]
        [SerializeField] private List<PosesNet> poses;

        [Tooltip("Визуальная часть нетворк игрока")]
        [SerializeField] private List<GameObject> visualsNetwork;

        [Tooltip("Компонент подключения трекеров")]
        [SerializeField] private TrackerRoleSetup trackerRoleSetup;
        
        private readonly SyncDictionary<BodyRole,bool> _bodyActives = new SyncDictionary<BodyRole,bool>();
        
        public override void OnStartClient()
        {
            if (!isOwned)
            {
                GetActiveBodies();
            }
            else
            {
                SetVisualLocal();
                InitPoses();
                SetActiveBodies();
            }
        }

        #region Methods setup player

        /// <summary>
        /// Включение локального игрока и выключение нетворк визуальной части
        /// </summary>
        private void SetVisualLocal()
        {
            DebugVR.Log(gameObject.name + " is local: " + isLocalPlayer);
            playerLocal.SetActive(isLocalPlayer);
            foreach (var visual in visualsNetwork)
            {
                Destroy(visual);
            }
            visualsNetwork.Clear();
        }

        /// <summary>
        /// Установка у нетворк частей тела их локальной части
        /// Добавление особенных костей в список для их синхонизации
        /// </summary>
        private void InitPoses()
        {
            foreach (var pose in poses)
            {
                pose.SetTarget();
                if (pose.bodyRole != BodyRole.Invalid)
                {
                    _bodyActives.Add(pose.bodyRole, false);   
                }
            }
        }

        /// <summary>
        /// Синхронизация активностей нетворк частей тела
        /// и их подписка на смену активностей
        /// </summary>
        private void SetActiveBodies()
        {
            foreach (var pose in poses)
            {
                if (pose.posLocal.TryGetComponent(out IViveRoleComponent roleSetter))
                {
                    BodyRole role = (BodyRole) roleSetter.viveRole.roleValue;
                    foreach (var tracker in trackerRoleSetup.trackersRole)
                    {
                        if (tracker.role == role)
                        {
                            SetActiveToObj(role, tracker.IsActive());
                            pose.playerRig = this;
                            tracker.OnChangeActive += pose.OnChangeActive;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Получение активностей частей тела
        /// </summary>
        private void GetActiveBodies()
        {
            foreach (var pose in poses)
            {
                if (pose.bodyRole != BodyRole.Invalid && _bodyActives.ContainsKey(pose.bodyRole))
                {
                    pose.posNet.SetActive(_bodyActives[pose.bodyRole]);
                }
            }
        }

        #endregion

        #region Network Methods

        /// <summary>
        /// Отправка команды включение/выключения
        /// </summary>
        /// <param name="role"></param>
        /// <param name="active"></param>
        [Command(requiresAuthority = false)]
        public void SetActiveToObj(BodyRole role, bool active)
        {
            SetActiveToObjToClients(role, active);
        }
        
        /// <summary>
        /// Включение части тела
        /// </summary>
        /// <param name="role"></param>
        /// <param name="active"></param>
        [ClientRpc]
        private void SetActiveToObjToClients(BodyRole role, bool active)
        {
            foreach (var pose in poses)
            {
                if (pose.bodyRole != BodyRole.Invalid && pose.bodyRole == role)
                {
                    DebugVR.Log("SetActive RPC: " + pose.posNet.name + " : " + active);
                    pose.posNet.SetActive(active);
                    if (isOwned)
                    {
                        _bodyActives[role] = active;
                    }
                }
            }
        }

        #endregion
    }
}