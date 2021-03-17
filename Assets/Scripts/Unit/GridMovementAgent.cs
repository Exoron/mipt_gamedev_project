using Field;
using UnityEngine;

namespace Unit
{
    public class GridMovementAgent: MonoBehaviour
    {
        [SerializeField]
        private float m_Speed;

        private const float TOLERANCE = 0.1f;

        private Node m_TargetNode;
    
        void Start()
        {

        }

        void Update()
        {
            if (m_TargetNode == null)
            {
                return;
            }

            Vector3 target = m_TargetNode.Position;
            
            if ((target - transform.position).magnitude < TOLERANCE)
            {
                m_TargetNode = m_TargetNode.NextNode;
                return;
            }
        
            Vector3 dir = (target - transform.position).normalized;
            Vector3 delta = dir * (m_Speed * Time.deltaTime);
            transform.Translate(delta);
        }

        public void SetTargetNode(Node node)
        {
            m_TargetNode = node;
        }
    }
}