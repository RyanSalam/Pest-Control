using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner<T> where T : MonoBehaviour
{
    [SerializeField] [Range(1, 20)] public float detectionRadius = 10f;
    [SerializeField] [Range(0, 360)] public float detectionAngle = 75f;
    public LayerMask targetMask;

    private T m_currentTarget;
    private T temp;
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
        Collider[] cols = Physics.OverlapSphere(m_transform.position, detectionRadius, targetMask);

        foreach (Collider col in cols)
        {
            Vector3 forward = m_transform.forward;
            forward = Quaternion.AngleAxis(detectionAngle, m_transform.up) * forward;

            Vector3 pos = col.transform.position - m_transform.position;
            pos -= m_transform.up * Vector3.Dot(m_transform.up, pos);

            if (Vector3.Angle(forward, pos) > detectionAngle / 2)
            {
                // Assign the collider to a temp variable
                temp = col.GetComponent<T>();

                // Raycast to check for any obstructions to line of sight
                Ray lineOfSight = new Ray(m_transform.position, m_transform.forward);
                RaycastHit hit;
                if (!Physics.Raycast(lineOfSight, out hit, detectionRadius))
                {
                    if (temp != null)
                    {
                        return temp;
                    }
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
        if (temp != null)
            Debug.DrawRay(m_transform.position, m_transform.forward * detectionRadius, Color.red);
    }

#endif
}
