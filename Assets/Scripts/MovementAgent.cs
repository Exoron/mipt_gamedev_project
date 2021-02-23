using System.Collections;
using System.Collections.Generic;
using Field;
using UnityEngine;

public class MovementAgent : MonoBehaviour
{
    [SerializeField]
    private float m_Speed;
    [SerializeField]
    private Vector3 m_Target;
    
    private const float TOLERANCE = 0.1f;
    
    void Start()
    {

    }

    void Update()
    {
        if ((m_Target - transform.position).magnitude < TOLERANCE)
        {
            return;
        }
        
        Vector3 dir = (m_Target - transform.position).normalized;
        Vector3 delta = dir * (m_Speed * Time.deltaTime);
        transform.Translate(delta);
    }

    public void SetTarget(Vector3 target)
    {
        m_Target = target;
    }
}
