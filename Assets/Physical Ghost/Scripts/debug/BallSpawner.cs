using System;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private float strength;
    [SerializeField] private float returnAfter = 5;

    private GameObject _ball;
    private Transform _tf;
    private Transform _ballTf;
    private Rigidbody _ballRb;

    private float _returnTs;


    private void Awake()
    {
        _tf = transform;
        _ball = Instantiate(ballPrefab, _tf.position, _tf.rotation);
        _ballTf = _ball.transform;
        _ballRb = _ball.GetComponent<Rigidbody>();

        LaunchBall();
    }

    private void LaunchBall()
    {
        _ballTf.position = _tf.position;
        _ballRb.Sleep();
        _ballRb.velocity.Set(0, 0, 0);
        _ballRb.WakeUp();
        _ballRb.AddForce(_tf.forward * strength);
        _returnTs = Time.time + returnAfter;
    }

    private void Update()
    {
        if (_returnTs < Time.time)
            LaunchBall();
    }
}