using UnityEngine;
using System.Collections;

namespace Warlock.Player
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class PointToMove : MonoBehaviour
    {
        public float MaxDistance = 15;
        public float ControllableVelocity;
        public float VelocityControlPower = 70;
        public float VelocityControlPowerDepleter = 25;

        private NavMeshAgent m_Agent;
        private Rigidbody m_RigidBody;
        private bool m_IsControllable;
        private float m_VelocityControlPower;

        // Use this for initialization
        void Start()
        {
            m_Agent = GetComponent<NavMeshAgent>();
            m_RigidBody = GetComponent<Rigidbody>();
            m_VelocityControlPower = VelocityControlPower;
        }

        // Update is called once per frame
        void Update()
        {
            m_IsControllable = IsControllable();

            if(m_Agent.enabled)
                m_RigidBody.velocity = m_Agent.desiredVelocity;

            if (Input.GetButtonDown("Fire1"))
            {
                if (!m_Agent.enabled)
                {
                    if (m_IsControllable)
                    {
                        EnableNavmeshAgent();
                    }
                    else
                    {
                        var raycast = GetClickedPoint();
                        var pointClicked = raycast.GetPoint(MaxDistance);
                        var direction = pointClicked - transform.position;
                        m_RigidBody.AddForce(direction * m_VelocityControlPower);
                        m_VelocityControlPower -= VelocityControlPowerDepleter;
                        if (m_VelocityControlPower < 0)
                            m_VelocityControlPower = 0;

                        return;
                    }
                }

                var ray = GetClickedPoint();
                var point = ray.GetPoint(MaxDistance);
                m_Agent.SetDestination(point);
            }
        }

        private void EnableNavmeshAgent()
        {
            m_RigidBody.isKinematic = true;
            m_Agent.enabled = true;
            m_VelocityControlPower = VelocityControlPower;
        }

        private Ray GetClickedPoint()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray);
            return ray;
        }

        bool IsControllable()
        {
            var velocity = m_RigidBody.velocity;
            return (velocity.x < ControllableVelocity && velocity.x > -ControllableVelocity) &&
                (velocity.y < ControllableVelocity && velocity.y > -ControllableVelocity) &&
                (velocity.z < ControllableVelocity && velocity.z > -ControllableVelocity);
        }

        void OnCollisionEnter(Collision col)
        {
            m_RigidBody.isKinematic = false;
            m_Agent.enabled = false;
            m_RigidBody.AddForce((transform.forward * -1) * 300);
        }
    } 
}
