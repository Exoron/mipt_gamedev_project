using UnityEngine;

namespace Field
{
    public class Node
    {
        public Vector3 Position;
        
        public Node NextNode;
        public bool IsOccupied;

        public float PathWeigth;

        public Node(Vector3 position)
        {
            this.Position = position;
        }

        public void ResetWeight()
        {
            PathWeigth = float.MaxValue;
        }
    }
}