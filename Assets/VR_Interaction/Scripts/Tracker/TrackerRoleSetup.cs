using System.Collections.Generic;
using HTC.UnityPlugin.Vive;
using HTC.UnityPlugin.VRModuleManagement;
using UnityEngine;

namespace BCS.CORE.VR
{
    /// <summary>
    /// Установка ролей трекеров
    /// </summary>
    public class TrackerRoleSetup : MonoBehaviour
    {
        public readonly List<TrackerRoleState> trackersRole = new List<TrackerRoleState>();
        
        [Tooltip("Локальные трекеры частей тела (кроме рук)")]
        [SerializeField] private List<ViveRoleSetter> trackersLocal;
        
        private ViveRole.IMap _map;
        private readonly List<string> _serialNames = new List<string>();
        private TrackerRoleBase _trackerRoleBase;

        private void OnEnable()
        {
            //инициализация компонентов
            _trackerRoleBase = TrackerRoleDeterminant.GetTrackerRoleFramework(gameObject);
            _map = ViveRole.GetMap<BodyRole>();

            //создание списка трекеров
            foreach (var tracker in trackersLocal)
            {
                TrackerRoleState trackerRoleState 
                    = new TrackerRoleState(tracker.gameObject, (BodyRole) tracker.viveRole.roleValue);
                trackerRoleState.SetActive(false);
                trackersRole.Add(trackerRoleState);
            }

            //старт инициализации фреймворка трекеров и трекеров
            _trackerRoleBase.Init();
            _trackerRoleBase.OnReady += Setup;
            if (_trackerRoleBase.IsReady())
            {
                Setup();
            }
        }

        #region Invoke methods

        /// <summary>
        /// Подписка к подключению трекеров и включение уже имеющихся
        /// </summary>
        private void Setup()
        {
            DebugVR.Log("START SETUP TRACKERS");
            for (uint deviceIndex = 0; deviceIndex < VRModule.GetDeviceStateCount(); ++deviceIndex)
            {
                if (VRModule.GetCurrentDeviceState(deviceIndex).isConnected)
                {
                    OnDeviceConnected(deviceIndex, true);
                }
            }
            VRModule.onDeviceConnected += OnDeviceConnected;
            _trackerRoleBase.OnReady -= Setup;
        }

        /// <summary>
        /// Обработка присоединения или отключения трекера
        /// </summary>
        /// <param name="deviceIndex"></param>
        /// <param name="connected"></param>
        private void OnDeviceConnected(uint deviceIndex, bool connected)
        {
            IVRModuleDeviceState device = VRModule.GetCurrentDeviceState(deviceIndex);

            if (connected)
            {
                if (device.deviceClass == VRModuleDeviceClass.GenericTracker)
                {
                    DebugVR.Log("Connect: " + device.serialNumber);
                    SetRole(device, _trackerRoleBase.GetTrackerRoleFromName(device));
                }
            }
            else
            {
                string serial = GetSerialOfLostTracker();
                BodyRole role = GetBodyRoleBySerial(serial);

                DebugVR.Log("Disconnect: " + serial);
                DeleteRole(serial, role);
            }
        }

        #endregion

        #region Methods of searching

        /// <summary>
        /// Поиск потерянного трекера, 
        /// который есть в списке существующих трекеров, но нет в сохраненных
        /// </summary>
        /// <returns>модель трекера</returns>
        private string GetSerialOfLostTracker()
        {
            List<string> currentSerialsName = new List<string>();
            for (uint deviceIndex = 0; deviceIndex < VRModule.GetDeviceStateCount(); ++deviceIndex)
            {
                currentSerialsName.Add(VRModule.GetCurrentDeviceState(deviceIndex).serialNumber);
            }
            
            foreach (var serial in _serialNames)
            {
                if (!currentSerialsName.Contains(serial))
                {
                    DebugVR.Log("Lost: " + serial);
                    return serial;
                }
            }

            DebugVR.Log("Lost: Не найден");
            return "";
        }

        /// <summary>
        /// Получение роли по названию трекера
        /// </summary>
        /// <param name="serialName"></param>
        /// <returns>роль</returns>
        private BodyRole GetBodyRoleBySerial(string serialName)
        {
            foreach (var tracker in trackersRole)
            {
                if (tracker.IsActive() && tracker.serial == serialName)
                {
                    return tracker.role;
                }
            }
            
            return BodyRole.Invalid;
        }

        #endregion

        #region Role control methods
        
        /// <summary>
        /// Установка роли трекеру
        /// </summary>
        /// <param name="device"></param>
        /// <param name="role"></param>
        private void SetRole(IVRModuleDeviceState device, BodyRole role)
        {
            _map.BindDeviceToRoleValue(device.serialNumber, (int) role);
            DebugVR.Log($"Device s: {device.serialNumber} role: {role}");
            _serialNames.Add(device.serialNumber);
            foreach (var tracker in trackersRole)
            {
                if (tracker.role == role)
                {
                    tracker.serial = device.serialNumber;
                    tracker.SetActive(true);
                    break;
                }
            }
        }

        /// <summary>
        /// Удаление роли трекера
        /// </summary>
        /// <param name="serialName"></param>
        /// <param name="role"></param>
        private void DeleteRole(string serialName, BodyRole role)
        {
            _serialNames.Remove(serialName);
            foreach (var tracker in trackersRole)
            {
                DebugVR.Log("/" + tracker.serial);
                if (tracker.serial == serialName)
                {
                    tracker.SetActive(false);
                    break;
                }
            }
        }
        #endregion

        #region Unity editor testing
#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                trackersRole[0].SetActive(!trackersRole[0].IsActive());
            }
        }
#endif
        #endregion
    }
}