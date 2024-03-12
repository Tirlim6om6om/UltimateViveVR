using UnityEngine;

public class BallFriction : MonoBehaviour
{
    public float frictionCoefficient = 0.1f; // Коэффициент трения для линейного движения
    public float angularFrictionCoefficient = 0.05f; // Коэффициент трения для вращения
    private Rigidbody rb;

    void Start()
    {
        // Получаем компонент Rigidbody мяча
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Применяем линейное трение, если мяч находится в движении
        if (rb.velocity.magnitude > 0.01f)
        {
            Vector3 frictionForce = -rb.velocity.normalized * frictionCoefficient;
            rb.AddForce(frictionForce, ForceMode.Acceleration);
        }
        else if (rb.velocity.magnitude <= 0.01f) // Останавливаем мяч, если его линейная скорость становится очень маленькой
        {
            rb.velocity = Vector3.zero;
        }

        // Применяем трение к вращению, если мяч вращается
        if (rb.angularVelocity.magnitude > 0.01f)
        {
            Vector3 angularFrictionForce = -rb.angularVelocity.normalized * angularFrictionCoefficient;
            rb.AddTorque(angularFrictionForce, ForceMode.Acceleration);
        }
        else if (rb.angularVelocity.magnitude <= 0.01f) // Останавливаем вращение мяча, если его угловая скорость становится очень маленькой
        {
            rb.angularVelocity = Vector3.zero;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        // Проверяем, является ли столкновение с другим мячом или с любым объектом, который должен снизить вращение
        if (collision.gameObject.CompareTag("Ball"))
        {
            // Применяем коэффициент уменьшения угловой скорости
            rb.angularVelocity *= 0.5f; // Уменьшаем угловую скорость вдвое, значение можно настроить
        }
    }
    
}