using TMPro;
using UnityEngine;
using Valve.VR;

public class DebugVR : MonoBehaviour
{
    private static DebugVR instance;
    public TextMeshProUGUI textLog;
    
    private void Awake()
    {
        if (instance)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
        }
    }

    public static void Log(string text)
    {
        if(!instance)
            return;
        Debug.Log(text);
        if(instance.textLog)
            instance.textLog.text = text + "\n" + instance.textLog.text;
    }
}
