using HTC.UnityPlugin.Vive;
using UnityEngine;

namespace BCS.CORE.VR
{
    /// <summary>
    /// Класс, определяющий роли от фреймворка
    /// </summary>
    public abstract class TrackerRoleBase : MonoBehaviour
    {
        /// <summary>
        /// Инициализация фреймворка трекеров
        /// </summary>
        public abstract void Init();
        
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
        public abstract bool GetTrackerRoleFromName(string modelNumber, out BodyRole role);
    }
}