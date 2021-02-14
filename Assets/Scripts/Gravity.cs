using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;


public static class Vector3Utility
{
    public static float Dot(this Vector3 u, Vector3 v)
    {
        return Vector3.Dot(u, v);
    }

    public static Vector3 Project(this Vector3 u, Vector3 v)
    {
        return Vector3.Project(u, v);
    }
}


public class Gravity : MonoBehaviour
{
    [SerializeField]
    private float m_G;
    [SerializeField]
    private Vector3 m_light_body_pos;
    [SerializeField] 
    private Vector3 m_v;
    
    [SerializeField]
    private float m_M;
    [SerializeField]
    private Vector3 m_heavy_body_pos;
    
    void Start()
    {
        transform.position = m_light_body_pos;
    }
    
    void FixedUpdate()
    {
        Vector3 pos = transform.position;
        float module = (m_heavy_body_pos - pos).magnitude;
        if (module > 1000)
        {
            Destroy(transform.gameObject);
        }

        if (module < 1)
        {
            m_v -= 2 * m_v.Project(m_heavy_body_pos - pos);
        }
        float denom = module * module * module;
        Vector3 a = m_G * m_M / denom * (m_heavy_body_pos - pos);
        
        float dt = Time.deltaTime;
        transform.Translate(dt * (m_v + a * dt / 2));
        m_v += a * dt;
    }
}
