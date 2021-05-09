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

        private bool m_NeedUpdate;

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
            ResetCache();
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

                ProcessClickOnGrid(coords, m_StartCoordinate);
                if(m_NeedUpdate)
                {
                    m_Grid.UpdateField();
                    ResetCache();
                }
            }
        }
        
        private void ResetCache()
        {
            Debug.Log("Cache reset!");
            for (int y = 0; y < m_Grid.Height; ++y)
            {
                for (int x = 0; x < m_Grid.Width; ++x)
                {
                    m_Grid.GetNode(x, y).OccupationAvailability = EOccupationAvailability.CanOccupy;
                }
            }
            
            var current_node = m_Grid.GetNode(m_Grid.Start);

            while (current_node.Coords != m_Grid.Target)
            {
                current_node.OccupationAvailability = EOccupationAvailability.Undefined;
                /*
                 
                 * : Node
                 ? : Undefinded
                 | : Vertical edge
                 /, \ : Diagonal edge
                 
                 Case 1:
                 . . .      . . .
                 . * .      . ? .
                 . | .  =>  . | .
                 . * .      . ? .           
                 . . .      . . .
                 Same for horisontal           
                 
                 Case 2:
                 . . . . .      . . . . .
                 . * . * .      . ? . ? .
                 . . / . .  =>  . . / . .
                 . * . * .      . ? . ? .
                 . . . . .      . . . . .
                 Same for other diagonal
                 
                 */

                if (current_node.HasDiagonalEdge())
                {
                    int x1 = current_node.Coords.x;
                    int y1 = current_node.Coords.y;
                    int x2 = current_node.NextNode.Coords.x;
                    int y2 = current_node.NextNode.Coords.y;

                    m_Grid.GetNode(x1, y2).OccupationAvailability = EOccupationAvailability.Undefined;
                    m_Grid.GetNode(x2, y1).OccupationAvailability = EOccupationAvailability.Undefined;
                }

                current_node = current_node.NextNode;
            }
            
            m_Grid.GetNode(m_Grid.Start).OccupationAvailability = EOccupationAvailability.CannotOccupy; 
            m_Grid.GetNode(m_Grid.Target).OccupationAvailability = EOccupationAvailability.CannotOccupy;
            m_NeedUpdate = false;
        }

        private void ProcessClickOnGrid(Vector2Int coords, Vector2Int start)
        {
            Node node = m_Grid.GetNode(coords.x, coords.y);
            if (node == null)
            {
                // not on grid
                return;
            }

            if (node.IsOccupied)
            {
                FreeNode(node);
            }
            else
            {
                TryOccupyNode(node, coords);
            }
        }

        private void TryOccupyNode(Node node, Vector2Int coords)
        {
            if (m_Grid.Pathfinding.CanOccupy(coords, out m_NeedUpdate))
            {
                node.IsOccupied = true;
            }
        }
        
        private void FreeNode(Node node)
        {
            m_NeedUpdate = true;
            node.IsOccupied = false;
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

                if (node.OccupationAvailability == EOccupationAvailability.Undefined)
                {
                    Gizmos.color = Color.yellow;
                }
                else if(node.OccupationAvailability == EOccupationAvailability.CanOccupy)
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.red;
                }

                //Gizmos.color = Color.red;
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