using System;
using UnityEngine;

namespace Field
{
    public class MovementCursor : MonoBehaviour
    {
        [SerializeField]
        private MovementAgent m_MovementAgent;

        [SerializeField]
        private GridHolder m_GridHolder;

        [SerializeField]
        private GameObject m_Cursor;

        private const float m_TieThreshold = 0.15f;
        
        private void Update()
        {
            // We ask GridHolder info about coordinates
            Vector3? mousePosition = m_GridHolder.GetMousePosition();
            Vector3? nodePosition = m_GridHolder.GetNodePosition();
            if (Input.GetMouseButton(0))
            {
                // Try move the object to another point
                if (nodePosition.HasValue)
                {
                    m_MovementAgent.SetTarget(nodePosition.Value);
                }
            }

            // Draw cursor
            if (mousePosition.HasValue && nodePosition.HasValue)
            {
                // If close to node centre then pin to centre
                Vector3 newPos = ((mousePosition.Value - nodePosition.Value).sqrMagnitude < m_TieThreshold) ? 
                    nodePosition.Value : mousePosition.Value;
                m_Cursor.SetActive(true);
                m_Cursor.transform.position = newPos;
            }
            else
            {
                m_Cursor.SetActive(false);
            }
        }
    }
}