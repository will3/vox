using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace FarmVox.Scripts
{
    public class SeekAndDestroy : MonoBehaviour
    {
        public Actor actor;
        public Animator animator;
        private static readonly int Attack = Animator.StringToHash("Attack");
        
        private Actor _target;

        private IEnumerator Start()
        {
            while (true)
            {
                if (!actor.agent.enabled)
                {
                    yield return new WaitForSeconds(0.2f);
                    continue;
                }

                if (_target == null)
                {
                    _target = GameObject.FindWithTag("Player")?.GetComponent<Actor>();
                }

                if (_target == null)
                {
                    yield return new WaitForSeconds(0.2f);
                    continue;
                }

                var agent = actor.agent;

                var stoppingDistance = 3.0f;
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("walk"))
                {
                    agent.isStopped = false;
                    agent.destination = _target.agent.nextPosition;
                    agent.stoppingDistance = stoppingDistance;    
                }
                else
                {
                    agent.isStopped = true;
                }

                
                var distanceFromTarget = (agent.nextPosition - _target.transform.position).magnitude;
                var shouldAttack = distanceFromTarget < 4.0f;
                animator.SetBool("Attack", shouldAttack);

                if (animator.GetCurrentAnimatorStateInfo(0).IsName("attack"))
                {
                    transform.LookAt(_target.transform, Vector3.up);
                }

                yield return null;
            }
        }
    }
}