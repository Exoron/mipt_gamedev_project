using System.Collections;
using System.Collections.Generic;
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

    // Update is called once per frame
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
}
