using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Warlock.Models;
using System;

namespace Warlock.UI
{
    [RequireComponent(typeof(Animator))]
    public class AnimationStateMachine : MonoBehaviour
    {
        public List<AnimatorParamConfigModel> AnimatorParameters;

        private Animator m_Animator;
        private string m_SpeedParam;

        void Start()
        {
            m_Animator = GetComponent<Animator>();
            m_SpeedParam = AnimatorParameters
                .FirstOrDefault(x => x.Type == PlayerAnimatorParameter.Speed).Name;
        }

        void Update()
        {
            var speed = Input.GetAxis("Vertical");
            m_Animator.SetFloat(m_SpeedParam, speed);
        }

        [Serializable]
        public enum PlayerAnimatorParameter
        {
            Speed = 0,
            Falling = 1
        }
    } 
}
