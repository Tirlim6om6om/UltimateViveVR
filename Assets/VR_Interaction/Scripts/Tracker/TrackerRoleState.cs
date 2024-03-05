using HTC.UnityPlugin.Vive;
using UnityEngine;
using UnityEngine.Events;

namespace BCS.CORE.VR
{
    /// <summary>
    /// Состояние локальных трекеров
    /// </summary>
    public class TrackerRoleState
    {
        public readonly BodyRole role;
        public delegate void RoleActiveState(bool active);
        public event RoleActiveState OnChangeActive;
        public string modelName;
        private readonly GameObject _obj;
        
        public TrackerRoleState(GameObject obj, BodyRole role)
        {
            _obj = obj;
            this.role = role;
        }

        public void SetActive(bool active)
        {
            _obj.SetActive(active);
            OnChangeActive?.Invoke(active);
        }

        public bool GetActive() => _obj.activeSelf;
    }
}