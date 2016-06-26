using UnityEngine;
using System.Collections;

namespace Warlock.Player
{
    public class ForceTest : MonoBehaviour
    {

        void OnCollisionEnter(Collision col)
        {
            var m_RigidBody = col.gameObject.GetComponent<CharacterControls>();
            if (m_RigidBody != null)
            {
                m_RigidBody.AddSpellForce((col.transform.forward * -1) * 100);
                Debug.Log("Force applied to " + col.gameObject.name);
            }
        }
    } 
}
