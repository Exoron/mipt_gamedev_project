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
        // used for ray casting
        private Camera m_Camera;
        // coords of left bottom grid corner
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
            m_Grid = new Grid(m_GridWidth, m_GridHeight, m_Offset, m_Nodesize, m_StartCoordinate, m_TargetCoordinate);
        }

        // for editor preview
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
                // mouse doesn't point to anything
                return;
            }

            if (hit.transform != transform)
            {
                // mouse doesn't point to the grid
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                // click on grid
                Vector3 hitPosition = hit.point;
                Vector3 difference = hitPosition - m_Offset;

                int x = (int) (difference.x / m_Nodesize);
                int y = (int) (difference.z / m_Nodesize);
                Vector2Int coords = new Vector2Int(x, y);

                TryOccupyNode(coords, m_StartCoordinate);
                m_Grid.UpdateField();
            }
        }

        private void TryOccupyNode(Vector2Int coords, Vector2Int start)
        {
            Node node = m_Grid.GetNode(coords.x, coords.y);
            if (node == null)
            {
                // not on grid
                return;
            }

            if (node.IsOccupied || m_Grid.Pathfinding.CanOccupy(coords))
            {
                node.IsOccupied = !node.IsOccupied;
            }
        }

        // coords of centre of a node the mouse points to
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
        
        // coords of the point on grid the cursor point to
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
            
            foreach (Node node in m_Grid.AllNodes())
            {
                if (node.NextNode == null)
                {
                    continue;
                }

                Vector3 start = node.Position;
                if (node.IsOccupied)
                {
                    Gizmos.color = Color.blue;
                    Vector3 size = Vector3.one * 0.5f;
                    Gizmos.DrawCube(start, size);
                    continue;
                }
                Gizmos.color = Color.red;
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