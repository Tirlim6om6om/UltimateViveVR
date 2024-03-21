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
        public static DebugVR instance;
        
        private void Awake()
        {
            if (instance)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
            }
        }

        public static void Log(string message)
        {
            Debug.Log("DebugVR: " + message);
            if (instance && instance.textLog)
            {
                instance.textLog.text = message + '\n' + instance.textLog.text;
            }
        }
    }
}