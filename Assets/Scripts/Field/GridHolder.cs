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

        [SerializeField]
        private Vector2Int m_TargetCoordinate;
        [SerializeField]
        private Vector2Int m_StartCoordinate;

        public Grid Grid => m_Grid;

        public Vector2Int StartCoordinate => m_StartCoordinate;

        private Camera m_Camera;

        private Vector3 m_Offset;

        [SerializeField]
        private float m_Nodesize;

        private void Awake()
        {
            m_Camera = Camera.main;

            float width = m_GridWidth * m_Nodesize;
            float height = m_GridHeight * m_Nodesize;
            transform.localScale = new Vector3(
                width * 0.1f, 
                1f, 
                height * 0.1f);

            m_Offset = transform.position - 
                       new Vector3(width, 0f, height) * 0.5f;
            m_Grid = new Grid(m_GridWidth, m_GridHeight, m_Offset, m_Nodesize, m_TargetCoordinate);
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
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = m_Camera.ScreenPointToRay(mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit))
            {
                return;
            }

            if (hit.transform != transform)
            {
                return;
            }

            Vector3 hitPosition = hit.point;
            Vector3 difference = hitPosition - m_Offset;

            int x = (int) (difference.x / m_Nodesize);
            int y = (int) (difference.z / m_Nodesize);

            if (Input.GetMouseButtonDown(0))
            {
                Node node = m_Grid.GetNode(x, y);
                node.IsOccupied = !node.IsOccupied;
                m_Grid.UpdateField();
            }
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
            if (m_Grid == null)
            {
                return;
            }

            Gizmos.color = Color.red;
            
            foreach (Node node in m_Grid.AllNodes())
            {
                if (node.NextNode == null)
                {
                    continue;
                }

                if (node.IsOccupied)
                {
                    continue;
                }
                Vector3 start = node.Position;
                Vector3 end = node.NextNode.Position;

                Vector3 dir = end - start;
                start -= dir * .25f;
                end -= dir * .75f;

                Gizmos.DrawLine(start, end);
                Gizmos.DrawSphere(end, .1f);
            }            
        }
    }
}