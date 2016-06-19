using UnityEngine;
using System.Collections;

namespace Warlock.Player
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody))]
    public class AnimationsController : MonoBehaviour
    {
        private Rigidbody m_Rigidbody;
        private Animator m_Animator;

        void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Animator = GetComponent<Animator>();
        }

        void Update()
        {
            if(m_Rigidbody.velocity != Vector3.zero)
            {
                m_Animator.SetBool("IsMoving", true);
            }
            else
            {
                m_Animator.SetBool("IsMoving", false);
            }
        }
    } 
}
