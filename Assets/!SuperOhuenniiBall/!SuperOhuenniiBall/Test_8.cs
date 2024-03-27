using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class Test_8 : NetworkBehaviour
{
    private float totalRayLength = 0f; // Общая сумма длин лучейы

    
    
    [Header("Kol_Kadrov_Proscheta")]
    public int framesToTrack = 20;
    private Queue<Vector3> positions = new Queue<Vector3>();
    
    [Header("Visual_Leg_RayCast")]
    public float slowSpeedThreshold = 0.1f;
    public float highSpeedThreshold = 0.5f;
    
    [Header("Obchaya_Sila_Udara")]
    public float impactForceCoefficient = 1f; // Коэффициент силы удара, настраиваемый в инспектоаре
    public float impactForceCoefficientconst = 1f; // Коэффициент силы удара, настраиваемый в инспекторе
   
    [SerializeField]
    private float UskorenieIzKrivoy = 2f; 

    
    [Header("Speed_Leg")]
    
    //значения после которых происходит увеличение силы удара. разделим их на 5 отрезков
    [SerializeField]
    private float katimBall = 0f; 
    [SerializeField]
    private float pasBall = 0f; 
    [SerializeField]
    private float mediumPasBall = 0f; 
    [SerializeField]
    private float navesBall = 0f; 
    [SerializeField]
    private float udarBall = 0f; 
    
        
    [Header("Uscor_Ball")]
    [SerializeField]
    private float katimUscor = 0f; 
    [SerializeField]
    private float pasUscor = 0f; 
    [SerializeField]
    private float mediumPasUscor = 0f; 
    [SerializeField]
    private float navesUscor = 0f; 
    [SerializeField]
    private float udarUscor = 0f; 
    [SerializeField]
    private float RonaldoUscor = 0f; 
    
    public float currentSpeed = 0f; // Текущая скорость
    
    
    
    
    
    private bool continueMovement = true;
    
    
    
    
    
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
            Kick(collision);
        }
    }

    [Command(requiresAuthority = false)]
    public void Kick(Collision collision)
    {
        Vector3 hitDirection = CalculateHitDirection();
        Vector3 contactPoint = collision.contacts[0].point;
        Vector3 centerOfBall = collision.gameObject.GetComponent<Collider>().bounds.center;
        Rigidbody ballRigidbody = collision.gameObject.GetComponent<Rigidbody>();
        Vector3 pointOfContact = collision.contacts[0].point;

        // Обновляем impactForceCoefficient в соответствии с силой удара

        // Проверяем, находимся ли мы в условии низкой скорости
        if (currentSpeed <= 1.2f)
        {

            return;
            // Проверяем, выше ли точка соприкосновения центра мяча
            if (contactPoint.y > centerOfBall.y)
            {
                // Определяем направление катения мяча на основе направления движения
                Vector3 rollDirection = hitDirection;
                // Устанавливаем направление катения перпендикулярно вектору от центра мяча к точке соприкосновения
                // Это имитирует эффект "вступления на мяч"
                rollDirection = Vector3.Cross(Vector3.up, rollDirection).normalized;
                float rollForceMagnitude = 1f; // Сила катения, может потребоваться настройка

                // Применяем силу для катения мяча
                ballRigidbody.AddForce(rollDirection * rollForceMagnitude, ForceMode.Impulse);
            }
            else
            {
                return;
            }
        }
        else if (currentSpeed > 1.2f && currentSpeed <= katimBall)
        {
            impactForceCoefficient *= katimUscor;
        }
        else if (currentSpeed > katimBall && currentSpeed <= pasBall)
        {
            impactForceCoefficient *= pasUscor;
        }
        else if (currentSpeed > pasBall && currentSpeed <= mediumPasBall)
        {
            impactForceCoefficient *= mediumPasUscor;
        }
        else if (currentSpeed > mediumPasBall && currentSpeed <= navesBall)
        {
            impactForceCoefficient *= navesUscor;
        }
        else if (currentSpeed > navesBall && currentSpeed <= udarBall)
        {
            impactForceCoefficient *= udarUscor;
        }
        else if (currentSpeed > navesBall)
        {
            impactForceCoefficient *= RonaldoUscor;
        }


        float impactMagnitude = totalRayLength * impactForceCoefficient;


        // Рисуем синий луч
        Vector3 blueRayEndPoint = contactPoint + hitDirection.normalized * impactMagnitude;
        Debug.DrawRay(contactPoint, hitDirection.normalized * impactMagnitude, Color.blue, 10f);

        // Рисуем белый луч
        Vector3 whiteRayDirection = (centerOfBall - contactPoint).normalized;
        Vector3 whiteRayEndPoint = contactPoint + whiteRayDirection * impactMagnitude;
        Debug.DrawRay(contactPoint, whiteRayDirection * impactMagnitude, Color.white, 10f);

        // Рисуем фиолетовый луч
        Vector3 purpleRayEndPoint = ((blueRayEndPoint + whiteRayEndPoint) / 2);
        Debug.DrawRay(contactPoint, purpleRayEndPoint - contactPoint, Color.magenta, 10f);



        // Контрольная точка для Безье кривой - это будет где-то вдоль синего луча
        Vector3 controlPointForBezier = blueRayEndPoint + (whiteRayEndPoint - blueRayEndPoint) * 0.25f; // Четверть пути к белому лучу

        // Рисуем Безье кривую
        // DrawBezierCurve(contactPoint, purpleRayEndPoint, controlPointForBezier, impactMagnitude, Color.cyan);
        // Рисуем Безье кривую, начиная от центра мяча
        DrawBezierCurve(centerOfBall, purpleRayEndPoint, controlPointForBezier, impactMagnitude, Color.cyan);

        // Сброс общей суммы длин лучей после удара
        totalRayLength = 0f;


        // Вызываем модифицированный метод для получения траектории Безье
        //List<Vector3> bezierPath = CalculateBezierPath(contactPoint, purpleRayEndPoint, controlPointForBezier, impactMagnitude);
        // Вызываем модифицированный метод для получения траектории Безье, начиная от центра мяча
        List<Vector3> bezierPath = CalculateBezierPath(centerOfBall, purpleRayEndPoint, controlPointForBezier, impactMagnitude);

        // Запускаем корутину для перемещения мяча по траектории
        StartCoroutine(MoveBallAlongPath(collision.gameObject, bezierPath));


        //вращение мяча вокруг своей оси


        // Вектор от центра мяча к точке соприкосновения (можно использовать для определения "направления" вращения)
        Vector3 directionOfSpin = Vector3.Cross(hitDirection, pointOfContact - centerOfBall).normalized;

        // Модифицируйте эту величину, чтобы изменить "силу" вращения
        float spinPower = 10f; // Примерное значение, требуется настройка под вашу задачу

        // Применяем угловую скорость
        ballRigidbody.angularVelocity = directionOfSpin * spinPower;
    }
    
    
    List<Vector3> CalculateBezierPath(Vector3 start, Vector3 end, Vector3 controlPoint, float length)
    {
        List<Vector3> path = new List<Vector3>();
        int segments = 20; // Количество сегментов для кривой
        float groundLevel = 0.01f; // Минимально допустимый уровень над "землей" для предотвращения ухода мяча под землю

        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector3 point = Mathf.Pow(1 - t, 2) * start + 2 * (1 - t) * t * controlPoint + Mathf.Pow(t, 2) * end;

            // Корректируем точку, если она ниже уровня земли
            if (point.y < groundLevel)
            {
                point.y = groundLevel;
            }

            path.Add(point);
        }

        return path;
    }
    
    
    IEnumerator MoveBallAlongPath(GameObject ball, List<Vector3> path)
    {
        Rigidbody rb = ball.GetComponent<Rigidbody>();

        Vector3 savedAngularVelocity = rb.angularVelocity; // Сохраняем угловую скорость
    
        rb.isKinematic = true; // Отключаем физику, чтобы управлять перемещением вручную

        float accelerationFactor = UskorenieIzKrivoy; // Коэффициент ускорения
        Vector3 previousPosition = ball.transform.position;
        Vector3 velocity = Vector3.zero;

        // Добавляем переменную для угловой скорости вращения
        float angularSpeed = 20f; // Это значение можно настроить для изменения скорости вращения

        for (int i = 0; i < path.Count && continueMovement; i++)
        {
            float t = (float)i / (path.Count - 1);
            t = Mathf.Pow(t, accelerationFactor);

            if (t > 0.66f) break;

            ball.transform.position = Vector3.Lerp(previousPosition, path[i], t);
            velocity = (ball.transform.position - previousPosition) / Time.deltaTime;

            // Применяем угловую скорость вращения
            rb.angularVelocity = Vector3.up * angularSpeed; // Вращаем вокруг вертикальной оси (Y)

            previousPosition = ball.transform.position;

            yield return null;
        }

        rb.isKinematic = false;
        rb.velocity = velocity;

        // Возможно, вы захотите сбросить угловую скорость к сохранённой величине или оставить вращение, добавленное во время полёта
        rb.angularVelocity = savedAngularVelocity; // или можно использовать новое значение для angularSpeed

        impactForceCoefficient = impactForceCoefficientconst;
    }

    
    
    

    
    
    
    
    void DrawBezierCurve(Vector3 start, Vector3 end, Vector3 controlPoint, float length, Color color)
    {
        Vector3 previousPoint = start;
        int segments = 20; // Количество сегментов для кривой
        bool hasCurveGoneUnderground = false; // Флаг для проверки, ушла ли кривая под землю

        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments;
            // Квадратичная Безье кривая: B(t) = (1-t)^2 * P0 + 2(1-t)t*P1 + t^2*P2, где P0 - начальная точка, P1 - контрольная точка, P2 - конечная точка
            Vector3 point = (1 - t) * (1 - t) * start + 2 * (1 - t) * t * controlPoint + t * t * end;
            if (!hasCurveGoneUnderground && point.y < 0)
            {
                Debug.Log("Кривая ушла под землю");
                hasCurveGoneUnderground = true;
            }
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
        
        // Расчет текущей скорости
        if (positions.Count > 1)
        {
            Vector3[] positionsArray = positions.ToArray();
            Vector3 lastPosition = positionsArray[positionsArray.Length - 1];
            Vector3 previousPosition = positionsArray[positionsArray.Length - 2];

            // Скорость = расстояние / время. Time.deltaTime - это время между кадрами
            currentSpeed = (lastPosition - previousPosition).magnitude / Time.deltaTime;
        }
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