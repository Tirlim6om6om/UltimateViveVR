using UnityEngine;

public class SpawnBall : MonoBehaviour
{
    public GameObject ballPrefab; // Префаб мяча для спавна
    public Transform spawnPoint; // Точка, в которой будет спавниться мяч

    void Update()
    {
        // Проверяем нажатие клавиши пробела
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Spawn();
        }
    }

    // Метод для спавна мяча
    public void Spawn()
    {
        if (ballPrefab == null || spawnPoint == null)
        {
            Debug.LogError("Ball prefab or spawn point not set.");
            return;
        }

        Instantiate(ballPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}