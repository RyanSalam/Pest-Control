using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner<T> where T : MonoBehaviour
{
    [SerializeField] [Range(1, 20)] private float detectionRadius = 10f;
    [SerializeField] [Range(0, 360)] private float detectionAngle = 75f;
    public LayerMask targetMask;

    private T m_currentTarget;
    private Transform m_transform;

    public T CurrentTarget
    {
        get { return m_currentTarget; }
    }

    public Scanner(Transform t) 
    {
        m_transform = t;
    }

    public T Detect()
    {
        Collider[] cols = Physics.OverlapSphere(m_transform.position, detectionRadius);

        foreach (Collider col in cols)
        {
            Vector3 forward = m_transform.forward;
            forward = Quaternion.AngleAxis(detectionAngle, m_transform.up) * forward;

            Vector3 pos = col.transform.position - m_transform.position;
            pos -= m_transform.up * Vector3.Dot(m_transform.up, pos);

            if (Vector3.Angle(forward, pos) > detectionAngle / 2)
            {
                T temp = col.GetComponent<T>();

                if (temp != null && temp != m_currentTarget)
                {
                    m_currentTarget = temp;
                    return temp;
                }
                               
            }
        }

        return null;
    }

#if UNITY_EDITOR
    public void EditorGizmo(Transform transform)
    {
        Color c = new Color(0, 0, 0.7f, 0.4f);

        UnityEditor.Handles.color = c;
        Vector3 rotatedForward = Quaternion.Euler(0, -detectionAngle * 0.5f, 0) * transform.forward;
        UnityEditor.Handles.DrawSolidArc(transform.position, Vector3.up, rotatedForward, detectionAngle, detectionRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + Vector3.up, 0.2f);
    }

#endif
}
