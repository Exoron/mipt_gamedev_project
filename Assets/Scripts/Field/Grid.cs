using System.Collections.Generic;
using UnityEngine;

namespace Field
{
    public class Grid
    {
        private Node[,] m_Nodes;

        private int m_Width;
        private int m_Height;

        // script for finding shortest paths to target node
        private FlowFieldPathFinding m_Pathfinding;

        public int Width => m_Width;
        public int Height => m_Height;
        public FlowFieldPathFinding Pathfinding => m_Pathfinding;

        public Grid(int width, int height, Vector3 offset, float nodeSize, Vector2Int start, Vector2Int target)
        {
            m_Width = width;
            m_Height = height;

            m_Nodes = new Node[m_Width, m_Height];

            for (int x = 0; x < m_Width; ++x)
            {
                for (int y = 0; y < m_Height; ++y)
                {
                    m_Nodes[x, y] = new Node(offset + new Vector3(x + .5f, 0, y + .5f) * nodeSize,
                    new Vector2Int(x, y));
                }
            }

            m_Pathfinding = new FlowFieldPathFinding(this, start, target);
            m_Pathfinding.UpdateField();
        }

        public Node GetNode(Vector2Int coord)
        {
            return GetNode(coord.x, coord.y);
        }

        public Node GetNode(int x, int y)
        {
            if (x < 0 || x >= m_Width || y < 0 || y >= m_Height)
            {
                return null;
            }
            return m_Nodes[x, y];
        }

        public IEnumerable<Node> AllNodes()
        {
            foreach (Node node in m_Nodes)
            {
                yield return node;
            }
        }

        public void UpdateField()
        {
            m_Pathfinding.UpdateField();
        }
    }
}