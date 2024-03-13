using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using BCS.CORE.VR;

public class BallKick : NetworkBehaviour
{
    public void MoveBallCMD(List<Vector3> path, float UskorenieIzKrivoy)
    {
        DebugVR.Log("Kick!");
        MoveBallClient(path, UskorenieIzKrivoy);
    }

    public void MoveBallClient(List<Vector3> path, float UskorenieIzKrivoy)
    {
        DebugVR.Log("Kick RPC!");
        StartCoroutine(MoveBallAlongPath(path, UskorenieIzKrivoy));
    }

    IEnumerator MoveBallAlongPath(List<Vector3> path, float UskorenieIzKrivoy)
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = true; // ��������� ������, ����� ��������� ������������ �������

        float accelerationFactor = UskorenieIzKrivoy; // ����������� ���������, ����� ��������� ��� ��������� ��������� ���������
        Vector3 previousPosition = transform.position;
        Vector3 velocity = Vector3.zero;

        for (int i = 0; i < path.Count; i++)
        {
            float t = (float)i / (path.Count - 1);
            // ��������� ������� ��������� (� ���� ������ �������� ��� ���������)
            t = Mathf.Pow(t, accelerationFactor);

            // ������������ t ����� ������� ����
            if (t > 0.66f) break;


            transform.position = Vector3.Lerp(previousPosition, path[i], t);
            velocity = (transform.position - previousPosition) / Time.deltaTime; // ��������� ������� ��������
            previousPosition = transform.position;

            yield return null; // ���� ��������� ����
        }

        // ������������� �� ����������� ������ � ��������� ��������� ��������
        rb.isKinematic = false;
        rb.velocity = velocity;
    }
}
