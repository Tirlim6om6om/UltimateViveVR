using TMPro;
using UnityEngine;

public class DebugVR : MonoBehaviour
{
    public static DebugVR Instance;
    public TextMeshProUGUI textLog;
    
    private void Awake()
    {
        if (Instance)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
    }

    public static void Log(string text)
    {
        Instance.textLog.text = text + "\n" + Instance.textLog.text;
    }
}
