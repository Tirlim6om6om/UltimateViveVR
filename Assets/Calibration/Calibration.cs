using UnityEngine;
using UnityEngine.SceneManagement;

public class Calibration : MonoBehaviour
{
    [Tooltip("виртуальный точка")]
    [SerializeField] private Transform virtualPoint;

    [Tooltip("Игрок")]
    [SerializeField] private Transform player;

    [Tooltip("Выключение калибровка высоты - высота 0")]
    [SerializeField] private bool zeroHeight;

    [Tooltip("Загрузка данных калибровки при включении")]
    [SerializeField] private bool loadCalibration;

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SceneManager.LoadScene(1);
        }
    }
#endif

    /// <summary>
    /// Калибровка по точке
    /// </summary>
    /// <param name="toPoint"></param>
    public void Calibrate(Transform toPoint)
    {
        Debug.Log("Calibration");
        Vector3 offset = player.position - toPoint.position;
        PlayerPrefs.SetFloat("CalibrationOffsetX", offset.x);
        PlayerPrefs.SetFloat("CalibrationOffsetY", offset.y);
        PlayerPrefs.SetFloat("CalibrationOffsetZ", offset.z);
        SetPlayer(offset);

        PlayerPrefs.SetFloat("CalibrationAngle", -toPoint.localEulerAngles.y);
        SetRotation(-toPoint.localEulerAngles.y);
    }

    /// <summary>
    /// Получение калибровки
    /// </summary>
    private void OnEnable()
    {
        if (!loadCalibration)
            return;
        float x = PlayerPrefs.GetFloat("CalibrationOffsetX");
        float y = PlayerPrefs.GetFloat("CalibrationOffsetY");
        float z = PlayerPrefs.GetFloat("CalibrationOffsetZ");
        SetPlayer(new Vector3(x, y, z));

        SetRotation(PlayerPrefs.GetFloat("CalibrationAngle"));
    }

    private void SetPlayer(Vector3 offset)
    {
        Vector3 newPos = virtualPoint.position + offset;
        player.position = new Vector3(newPos.x, zeroHeight ? 0 : newPos.y, newPos.z);
    }

    private void SetRotation(float angle)
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, angle, transform.eulerAngles.z);
    }
}