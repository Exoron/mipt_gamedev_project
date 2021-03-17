using System.Collections.Generic;
using UnityEngine;

namespace Field
{
    public class Grid
    {
        public int Width => m_Width;

        public int Height => m_Height;

        private Node[,] m_Nodes;

        private int m_Width;
        private int m_Height;

        private FlowFieldPathFinding m_Pathfinding;

        public Grid(int width, int height, Vector3 offset, float nodeSize, Vector2Int target)
        {
            m_Width = width;
            m_Height = height;

            m_Nodes = new Node[m_Width, m_Height];

            for (int i = 0; i < m_Width; ++i)
            {
                for (int j = 0; j < m_Height; ++j)
                {
                    m_Nodes[i, j] = new Node(offset + new Vector3(i + .5f, 0, j + .5f) * nodeSize);
                }
            }

            m_Pathfinding = new FlowFieldPathFinding(this, target);
            m_Pathfinding.UpdateField();
        }

        public Node GetNode(Vector2Int coord)
        {
            return GetNode(coord.x, coord.y);
        }

        public Node GetNode(int i, int j)
        {
            if (i < 0 || i >= m_Width || j < 0 || j >= m_Height)
            {
                return null;
            }
            return m_Nodes[i, j];
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