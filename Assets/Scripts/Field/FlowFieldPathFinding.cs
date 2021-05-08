using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Field
{
    public class FlowFieldPathFinding
    {
        private Grid m_Grid;
        private Vector2Int m_Target;
        private Vector2Int m_Start;
        private static float root2 = (float)Math.Sqrt(2);
        private bool m_NeedUpdate = true;
        
        // comparer for nodes in Dijkstra algorithm
        // compares by path length then lexically by coords (to distinguish nodes with equal paths)
        private class DijkstraCompare: IComparer<Vector2Int>
        {
            public DijkstraCompare(Grid grid)
            {
                this.grid = grid;
            }
            public int Compare(Vector2Int lhs, Vector2Int rhs)
            {
                Node a = grid.GetNode(lhs);
                Node b = grid.GetNode(rhs);
                int res1 = a.PathWeight.CompareTo(b.PathWeight);
                if (res1 != 0)
                {
                    return res1;
                }

                int res2 = lhs.x.CompareTo(rhs.x);
                if (res2 != 0)
                {
                    return res2;
                }

                return lhs.y.CompareTo(rhs.y);
            }

            private Grid grid;
        }

        private struct Connection
        {
            public Vector2Int coord;
            public float weight;

            public Connection(Vector2Int coord, float weight)
            {
                this.coord = coord;
                this.weight = weight;
            }
        }

        public FlowFieldPathFinding(Grid mGrid, Vector2Int mStart, Vector2Int mTarget)
        {
            m_Grid = mGrid;
            m_Start = mStart;
            m_Target = mTarget;
        }

        // Dijkstra algorithm
        public void UpdateField()
        {
            if (!m_NeedUpdate)
            {
                // no need to update anything
                return;
            }
            foreach (Node node in m_Grid.AllNodes())
            {
                node.ResetWeight();
            }
            
            var heap = new SortedSet<Vector2Int>(new DijkstraCompare(m_Grid));
            m_Grid.GetNode(m_Target).PathWeight = 0f;
            heap.Add(m_Target);

            while (heap.Count > 0)
            {
                Vector2Int current = heap.Min;
                heap.Remove(current);
                Node currentNode = m_Grid.GetNode(current);

                foreach (Connection neighbour in GetNeighbours(current))
                {
                    Node neighbourNode = m_Grid.GetNode(neighbour.coord);
                    float weightToTarget = currentNode.PathWeight + neighbour.weight;
                    if (weightToTarget < neighbourNode.PathWeight)
                    {
                        if (heap.Contains(neighbour.coord))
                        {
                            heap.Remove(neighbour.coord);
                        }

                        neighbourNode.NextNode = currentNode;
                        neighbourNode.PathWeight = weightToTarget;
                        heap.Add(neighbour.coord);
                    }
                }
            }
            
            ResetCache();
        }

        public void ResetCache()
        {
            Debug.Log("Cache reset!");
            for (int y = 0; y < m_Grid.Height; ++y)
            {
                for (int x = 0; x < m_Grid.Width; ++x)
                {
                    m_Grid.GetNode(x, y).OccupationAvailability = EOccupationAvailability.CanOccupy;
                }
            }
            
            var current_node = m_Grid.GetNode(m_Start);

            while (current_node.Coords != m_Target)
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
            
            m_Grid.GetNode(m_Start).OccupationAvailability = EOccupationAvailability.CannotOccupy; 
            m_Grid.GetNode(m_Target).OccupationAvailability = EOccupationAvailability.CannotOccupy;
        }


        // BFS from target
        public bool CanOccupy(Vector2Int coord)
        {
            Node targetNode = m_Grid.GetNode(coord);
            bool? canOccupy = targetNode.CanOccupy();
            if (canOccupy.HasValue)
            {
                m_NeedUpdate = false; // nothing important changed
                return canOccupy.Value;
            }
            m_NeedUpdate = true;

            // see if with the node occupied a path still exist
            targetNode.IsOccupied = true;
            
            var queue = new Queue<Vector2Int>();
            queue.Enqueue(m_Target);
            
            var visited = new bool[m_Grid.Width, m_Grid.Height];
            for (int x = 0; x < m_Grid.Width; ++x)
            {
                for (int y = 0; y < m_Grid.Height; ++y)
                {
                    visited[x, y] = false;
                }
            }
            visited[m_Target.x, m_Target.y] = true;
            
            while (queue.Count > 0)
            {
                Vector2Int index = queue.Dequeue();
                if (index == m_Start)
                {
                    targetNode.IsOccupied = false;
                    // cache will be reset anyway
                    targetNode.OccupationAvailability = EOccupationAvailability.CanOccupy;
                    return true;
                }

                foreach (Connection connection in GetNeighbours(index))
                {
                    Vector2Int neighbour = connection.coord;
                    if(!visited[neighbour.x, neighbour.y])
                    {
                        queue.Enqueue(neighbour);
                        visited[neighbour.x, neighbour.y] = true;
                    }
                }
            }
            
            // remove temporary occupation
            targetNode.IsOccupied = false;
            targetNode.OccupationAvailability = EOccupationAvailability.CannotOccupy;
            m_NeedUpdate = false; // grid will not change. No update necessary
            return false;
        }

        private bool IsOnGrid(Vector2Int coord)
        {
            return (0 <= coord.x && coord.x < m_Grid.Width) && (0 <= coord.y && coord.y < m_Grid.Height);
        }

        private IEnumerable<Connection> GetNeighbours(Vector2Int coordinate)
        {
            for (int dx = -1; dx <= 1; ++dx)
            {
                for (int dy = -1; dy <= 1; ++dy)
                {
                    if (dx == 0 && dy == 0)
                    {
                        continue;
                    }

                    var dX = dx * Vector2Int.right;
                    var dY = dy * Vector2Int.up;
                    var neighbour = coordinate + dX + dY;
                    if (IsOnGrid(neighbour) && !m_Grid.GetNode(neighbour).IsOccupied)
                    {
                        int absSqr = (coordinate - neighbour).sqrMagnitude;
                        if (absSqr == 1)
                        {
                            yield return new Connection(neighbour, 1f);
                        } else if (!m_Grid.GetNode(coordinate + dX).IsOccupied && !m_Grid.GetNode(coordinate + dY).IsOccupied)
                        {
                            yield return new Connection(neighbour, root2);
                        }
                    }
                }
            }
        }
    }
}