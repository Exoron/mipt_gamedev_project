using UnityEngine;

namespace Field
{
    public enum EOccupationAvailability
    {
        CanOccupy,
        CannotOccupy,
        Undefined
    }
    public class Node
    {
        // coords in Unity space
        public Vector3 Position;
        // next node in shortest path to target
        public Node NextNode;
        public bool IsOccupied;
        // shortest distance to target
        public float PathWeight;
        // cached value of m_Grid.Pathfinding.CanOccupy(...)
        public EOccupationAvailability OccupationAvailability = EOccupationAvailability.Undefined;
        // coords on grid
        public Vector2Int Coords;

        public Node(Vector3 position, Vector2Int coords)
        {
            this.Position = position;
            this.Coords = coords;
        }

        public void ResetWeight()
        {
            PathWeight = float.MaxValue;
        }

        public bool? CanOccupy()
        {
            return OccupationAvailability switch
            {
                EOccupationAvailability.CanOccupy => true,
                EOccupationAvailability.CannotOccupy => false,
                _ => null
            };
        }

        public bool HasDiagonalEdge()
        {
            return (Coords - NextNode.Coords).sqrMagnitude == 2;
        }
    }
}