using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BCS.CORE.VR.Network.Example
{
    public class DebugVR : MonoBehaviour
    {
        public Text textLog;
        private static DebugVR instance;
        
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
            if(instance && instance.textLog)
                instance.textLog.text = message + '\n' + instance.textLog.text;
        }
    }
}