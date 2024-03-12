using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Test_2 : MonoBehaviour
{
    public int framesToTrack = 20;
    private Queue<Vector3> positions = new Queue<Vector3>();
    public float slowSpeedThreshold = 0.1f;
    public float highSpeedThreshold = 0.5f;
    public float impactForceCoefficient = 1f; // Коэффициент силы удара, настраиваемый в инспекторе
    private float totalRayLength = 0f; // Общая сумма длин лучейы
    [SerializeField]
    private float UskorenieIzKrivoy = 2f; 
    [SerializeField]
    private float SmeshcheniePurpleRay = 2f; //по умолчанию это среднее значение положения фиолетового луча

    void Start()
    {
        positions = new Queue<Vector3>(framesToTrack);
    }

    void Update()
    {
        TrackMovement();
        DrawDebugRays();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Debug.Log("Удар");
            Vector3 hitDirection = CalculateHitDirection();
            Vector3 contactPoint = collision.contacts[0].point;
            Vector3 centerOfBall = collision.gameObject.GetComponent<Collider>().bounds.center;
            float impactMagnitude = totalRayLength * impactForceCoefficient;
            

            // Рисуем синий луч
            Vector3 blueRayEndPoint = contactPoint + hitDirection.normalized * impactMagnitude;
            Debug.DrawRay(contactPoint, hitDirection.normalized * impactMagnitude, Color.blue, 10f);

            // Рисуем белый луч
            Vector3 whiteRayDirection = (centerOfBall - contactPoint).normalized;
            Vector3 whiteRayEndPoint = contactPoint + whiteRayDirection * impactMagnitude;
            Debug.DrawRay(contactPoint, whiteRayDirection * impactMagnitude, Color.white, 10f);

            // Рисуем фиолетовый луч
            Vector3 purpleRayEndPoint = ((blueRayEndPoint + whiteRayEndPoint) / 2) * SmeshcheniePurpleRay;
            Debug.DrawRay(contactPoint, purpleRayEndPoint - contactPoint, Color.magenta, 10f);
            
            
            
            // Контрольная точка для Безье кривой - это будет где-то вдоль синего луча
             Vector3 controlPointForBezier = blueRayEndPoint + (whiteRayEndPoint - blueRayEndPoint) * 0.25f; // Четверть пути к белому лучу
         
            // Рисуем Безье кривую
            DrawBezierCurve(contactPoint, purpleRayEndPoint, controlPointForBezier, impactMagnitude, Color.cyan);

            // Сброс общей суммы длин лучей после удара
            totalRayLength = 0f;
            
            
            // Вызываем модифицированный метод для получения траектории Безье
            List<Vector3> bezierPath = CalculateBezierPath(contactPoint, purpleRayEndPoint, controlPointForBezier, impactMagnitude);
            
            // Запускаем корутину для перемещения мяча по траектории
            StartCoroutine(MoveBallAlongPath(collision.gameObject, bezierPath));
            
            
        }
    }

    
    
    
    
    
    
    
    
    List<Vector3> CalculateBezierPath(Vector3 start, Vector3 end, Vector3 controlPoint, float length)
    {
        List<Vector3> path = new List<Vector3>();
        int segments = 20; // Количество сегментов для кривой

        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector3 point = (1 - t) * (1 - t) * start + 2 * (1 - t) * t * controlPoint + t * t * end;
            path.Add(point);
        }

        return path;
    }

    
    /*
    IEnumerator MoveBallAlongPath(GameObject ball, List<Vector3> path)
    {
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.isKinematic = true; // Отключаем физику, чтобы управлять перемещением вручную

        int targetIndex = (int)(path.Count * 0.66f); // Две трети пути

        for (int i = 0; i < targetIndex; i++)
        {
            ball.transform.position = path[i];
            yield return null; // Ждем следующий кадр
        }

        rb.isKinematic = false; // Возвращаем управление физикой объекту
        // Применяем силу для имитации продолжения движения в последнем направлении
        Vector3 lastDirection = (path[targetIndex] - path[targetIndex - 1]).normalized;
        rb.AddForce(lastDirection * 10f, ForceMode.VelocityChange); // Можно настроить силу
    }
    */
    
    
    
    
    IEnumerator MoveBallAlongPath(GameObject ball, List<Vector3> path)
    {
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.isKinematic = true; // Отключаем физику, чтобы управлять перемещением вручную

        float accelerationFactor = UskorenieIzKrivoy; // Коэффициент ускорения, можно настроить для изменения поведения ускорения
        Vector3 previousPosition = ball.transform.position;
        Vector3 velocity = Vector3.zero;

        for (int i = 0; i < path.Count; i++)
        {
            float t = (float)i / (path.Count - 1);
            // Применяем функцию ускорения (в этом случае линейную для упрощения)
            t = Mathf.Pow(t, accelerationFactor);
        
            // Ограничиваем t двумя третями пути
            if (t > 0.66f) break;
        
            ball.transform.position = Vector3.Lerp(previousPosition, path[i], t);
            velocity = (ball.transform.position - previousPosition) / Time.deltaTime; // Вычисляем текущую скорость
            previousPosition = ball.transform.position;
        
            yield return null; // Ждем следующий кадр
        }

        // Переключаемся на стандартную физику и применяем последнюю скорость
        rb.isKinematic = false;
        rb.velocity = velocity;

        // Опционально, можно добавить дополнительную силу для имитации броска
        // rb.AddForce(velocity * launchForceMultiplier, ForceMode.VelocityChange);
    }


    
    
    
    
    
    
    

    void DrawBezierCurve(Vector3 start, Vector3 end, Vector3 controlPoint, float length, Color color)
    {
        Vector3 previousPoint = start;
        int segments = 20; // Количество сегментов для кривой

        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments;
            // Квадратичная Безье кривая: B(t) = (1-t)^2 * P0 + 2(1-t)t*P1 + t^2*P2, где P0 - начальная точка, P1 - контрольная точка, P2 - конечная точка
            Vector3 point = (1 - t) * (1 - t) * start + 2 * (1 - t) * t * controlPoint + t * t * end;
            Debug.DrawLine(previousPoint, point, color, 10f);
            previousPoint = point;
        }
    }



    void TrackMovement()
    {
        if (positions.Count >= framesToTrack)
        {
            positions.Dequeue();
        }
        positions.Enqueue(transform.position);
    }

    void DrawDebugRays()
    {
        Vector3 previousPosition = Vector3.zero;
        totalRayLength = 0f; // Сбрасываем сумму перед вычислением

        foreach (Vector3 position in positions)
        {
            if (previousPosition != Vector3.zero)
            {
                float distance = (position - previousPosition).magnitude;
                float speed = distance / Time.deltaTime;
                Color lineColor = speed < slowSpeedThreshold ? Color.green : (speed > highSpeedThreshold ? Color.red : Color.yellow);
                Debug.DrawLine(previousPosition, position, lineColor, 0.1f);
                totalRayLength += distance; // Добавляем длину луча к общей сумме
            }
            previousPosition = position;
        }
    }

    Vector3 CalculateHitDirection()
    {
        if (positions.Count < 2) return transform.forward;

        Vector3[] positionsArray = positions.ToArray();
        Vector3 lastPosition = positionsArray[positionsArray.Length - 1];
        Vector3 previousPosition = positionsArray[positionsArray.Length - 2];

        return (lastPosition - previousPosition).normalized;
    }
}