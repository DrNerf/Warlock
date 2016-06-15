using UnityEngine;
using System.Collections;

namespace Warlock.Player
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class PointToMove : MonoBehaviour
    {
        public float MaxDistance = 15;

        private NavMeshAgent m_Agent;
        private Rigidbody m_RigidBody;

        // Use this for initialization
        void Start()
        {
            m_Agent = GetComponent<NavMeshAgent>();
            m_RigidBody = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray))
                {
                    if (!m_Agent.enabled)
                    {
                        m_RigidBody.isKinematic = true;
                        m_Agent.enabled = true;
                    }

                    var point = ray.GetPoint(MaxDistance);
                    m_Agent.SetDestination(point);
                }
            }

            if(m_Agent.enabled)
                m_RigidBody.velocity = m_Agent.desiredVelocity;
        }

        void OnCollisionEnter(Collision col)
        {
            m_RigidBody.isKinematic = false;
            m_Agent.enabled = false;
            m_RigidBody.AddForce((transform.forward * -1) * 150);
        }
    } 
}
