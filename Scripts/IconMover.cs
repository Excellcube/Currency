using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IconMover : MonoBehaviour
{
    public Vector3[] m_ControlPoints = new Vector3[3];
    public float m_Duration = 1.0f;
    private float t = 0f;

    public UnityEvent onFinish = new UnityEvent();
    private UnityEvent m_OnFinish {
        get => m_OnFinish;
        set => m_OnFinish = value;
    }

    private Rigidbody2D m_RigidBody;

    private Vector3     m_Start;
    private Vector3     m_End;
    private Vector3     m_Direction;


    public void Move(Transform target, float duration)
    {
        m_RigidBody = transform.GetComponent<Rigidbody2D>();

        m_Start       = transform.position;
        m_End         = target.position;
        m_Direction   = (m_End - m_Start).normalized;

        m_ControlPoints[0] = m_Start;
        m_ControlPoints[1] = GetRandomControlPoint(m_Start, m_End);
        m_ControlPoints[2] = m_End;
        m_Duration = duration;
        t = 0.2f;  // 애니메이션이 느리게 시작됨. 중간 부분부터 애니메이션 실행.

        StartCoroutine( MoveInternal() );
    }

    private Vector3 GetRandomControlPoint(Vector3 start, Vector3 end)
    {
        Vector3 baseDir = new Vector3(0, 1, 0);
        Vector3 fromTargetDir = (start - end).normalized;
        float angle = Vector3.Angle(baseDir, fromTargetDir);
        float gap = 25.0f;

        float randRad = Random.Range(angle - gap, angle + gap) * 3.1415f / 180.0f;
        Vector3 dir = Vector3.zero;
        dir.x = Mathf.Sin(randRad);
        dir.y = Mathf.Cos(randRad);

        Vector3 controlPoint = m_ControlPoints[0] + dir * 500f;
        return controlPoint;
    }

    private IEnumerator MoveInternal()
    {
        while(t < 1)
        {
            transform.position = CalculateBezierPoint(t);

            t += Time.deltaTime / m_Duration;

            yield return null;
        }
        transform.position = m_ControlPoints[2];
        onFinish?.Invoke();

        // TODO. Pooling 사용.
        Destroy(transform.gameObject);
    }

    private Vector3 CalculateBezierPoint(float t)
    {
        float easedT = easeInCubic(t);

        float u = 1 - easedT;
        float tt = easedT * easedT;
        float uu = u * u;
        
        Vector3 p = uu * m_ControlPoints[0];        // 첫 번째 항 (1-t)^2 * P0
        p += 2 * u * easedT * m_ControlPoints[1];   // 두 번째 항 2(1-t)t * P1
        p += tt * m_ControlPoints[2];               // 세 번째 항 t^2 * P2

        return p;
    }

    private float easeInCubic(float t)
    {
        return t * t * t;
    }
}
