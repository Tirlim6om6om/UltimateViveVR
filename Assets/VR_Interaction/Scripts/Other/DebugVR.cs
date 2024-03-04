using UnityEngine;
using UnityEngine.UI;

namespace BCS.CORE.VR
{
    /// <summary>
    /// Класс дебага для вр (выводит на текст, если он есть)
    /// </summary>
    public class DebugVR : MonoBehaviour
    {
        public Text textLog;
        private static DebugVR _instance;
        
        private void Awake()
        {
            if (_instance)
            {
                Destroy(this);
            }
            else
            {
                _instance = this;
            }
        }

        public static void Log(string message)
        {
            Debug.Log("DebugVR: " + message);
            if(_instance && _instance.textLog)
                _instance.textLog.text = message + '\n' + _instance.textLog.text;
        }
    }
}