using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 pos = transform.position;
        float module = (m_heavy_body_pos - pos).magnitude;
        if (module < 0.1 || module > 1000)
        {
            Destroy(transform.gameObject);
        }
        float denom = module * module * module;
        Vector3 a = m_G * m_M / denom * (m_heavy_body_pos - pos);
        
        float dt = Time.deltaTime;
        transform.Translate(dt * (m_v + a * dt / 2));
        m_v += a * dt;
    }
}
