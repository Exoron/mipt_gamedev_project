using System;
using UnityEngine;

namespace Field
{
    public class GridHolder : MonoBehaviour
    {
        [SerializeField]
        private int m_GridWidth;
        [SerializeField]
        private int m_GridHeight;

        private Grid m_Grid;

        private Camera m_Camera;

        private Vector3 m_Offset;

        [SerializeField]
        private float m_Nodesize;

        private void Awake()
        {
            m_Grid = new Grid(m_GridWidth, m_GridHeight);
            m_Camera = Camera.main;
            
            float width = m_GridWidth * m_Nodesize;
            float height = m_GridHeight * m_Nodesize;
            transform.localScale = new Vector3(
                width * 0.1f, 
                1f, 
                height * 0.1f);

            m_Offset = transform.position - 
                       new Vector3(width, 0f, height) * 0.5f;
        }

        private void OnValidate()
        {
            float width = m_GridWidth * m_Nodesize;
            float height = m_GridHeight * m_Nodesize;
            transform.localScale = new Vector3(
                width * 0.1f, 
                1f, 
                height * 0.1f);

            m_Offset = transform.position - 
                       new Vector3(width, 0f, height) * 0.5f;
        }

        private void Update()
        {
            
        }

        // Coordinates of centre of a node the mouse points on
        public Vector3? GetNodePosition()
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = m_Camera.ScreenPointToRay(mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit))
            {
                return null;
            }

            if (hit.transform != transform)
            {
                return null;
            }

            Vector3 hitPosition = hit.point;
            Vector3 difference = hitPosition - m_Offset;

            float x = (int) (difference.x / m_Nodesize);
            float z = (int) (difference.z / m_Nodesize);
            
            return new Vector3(x + m_Nodesize * 0.5f, 0, z + m_Nodesize * 0.5f) + m_Offset;
        }
        
        public Vector3? GetMousePosition()
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = m_Camera.ScreenPointToRay(mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit))
            {
                return null;
            }

            if (hit.transform != transform)
            {
                return null;
            }

            return hit.point;
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Vector3 height = Vector3.up * 0.1f;
            for (int i = 0; i <= m_GridWidth; ++i)
            {
                Gizmos.DrawLine(m_Nodesize * i * Vector3.right + height + m_Offset,
                    m_Nodesize * i * Vector3.right + m_Offset + height + Vector3.forward * m_GridHeight);
            }
            for (int j = 0; j <= m_GridHeight; ++j)
            {
                Gizmos.DrawLine(m_Nodesize * j * Vector3.forward + height + m_Offset,
                    m_Nodesize * j * Vector3.forward + m_Offset + height + Vector3.right * m_GridWidth);            }
        }
    }
}