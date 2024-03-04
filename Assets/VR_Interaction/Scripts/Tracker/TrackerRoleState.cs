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
        public readonly UnityEvent<bool> changeActive = new UnityEvent<bool>();
        public string modelNumber;
        private readonly GameObject _obj;
        
        public TrackerRoleState(GameObject obj, BodyRole role)
        {
            _obj = obj;
            this.role = role;
        }

        public void SetActive(bool active)
        {
            _obj.SetActive(active);
            changeActive?.Invoke(active);
        }

        public bool GetActive() => _obj.activeSelf;
    }
}