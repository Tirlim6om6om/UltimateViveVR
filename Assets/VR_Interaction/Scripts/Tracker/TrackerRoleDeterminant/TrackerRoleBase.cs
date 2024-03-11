using System.Collections;
using HTC.UnityPlugin.Vive;
using HTC.UnityPlugin.VRModuleManagement;
using UnityEngine;
using UnityEngine.Events;

namespace BCS.CORE.VR
{
    /// <summary>
    /// Класс, определяющий роли от фреймворка
    /// </summary>
    public abstract class TrackerRoleBase : MonoBehaviour
    {
        public delegate void InitStateEvents();
        public event InitStateEvents OnReady;

        /// <summary>
        /// Инициализация фреймворка трекеров
        /// </summary>
        public virtual void Init()
        {
            StartCoroutine(WaitReady());
        }

        /// <summary>
        /// Ожидание готовности и вызов метода по окончанию
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitReady()
        {
            while (!IsReady())
            {
                yield return null;
            }
            OnReady?.Invoke();
        }
        
        /// <summary>
        /// Фреймворк готов к работе
        /// </summary>
        /// <returns></returns>
        public abstract bool IsReady();
        
        /// <summary>
        /// Получение роли трекеров по имени
        /// </summary>
        /// <param name="modelNumber">номер модели</param>
        /// <param name="role">роль на выход</param>
        /// <returns></returns>
        public abstract BodyRole GetTrackerRoleFromName(IVRModuleDeviceState device);
    }
}