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
                if (pose.posNet.TryGetComponent(out RoleTrackerNetwork roleSet))
                {
                    _bodyActives.Add(roleSet.bodyRole, false);   
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
                            pose.playerRigController = this;
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
                if (pose.posNet.TryGetComponent(out RoleTrackerNetwork roleSet))
                {
                    DebugVR.Log("Bodies: " + _bodyActives.Count);
                    if (_bodyActives.ContainsKey(roleSet.bodyRole))
                    {
                        pose.posNet.SetActive(_bodyActives[roleSet.bodyRole]);
                    }
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
                if (pose.posNet.TryGetComponent(out RoleTrackerNetwork roleSetter))
                {
                    if (roleSetter.bodyRole == role)
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
        }

        #endregion
    }
}