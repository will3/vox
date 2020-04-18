using UnityEngine;
using UnityEngine.AI;

namespace FarmVox.Scripts
{
    public class PlayerMovement : MonoBehaviour
    {
        public NavMeshAgent agent;
        public Camera targetCamera;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            targetCamera = Camera.main;
        }

        private void Start()
        {
            agent.updatePosition = false;
        }

        private void Update()
        {
            UpdateMovement();

            transform.position = agent.nextPosition;
        }

        private void UpdateMovement()
        {
            var right = Input.GetAxisRaw("Horizontal");
            var forward = Input.GetAxisRaw("Vertical");

            if (!agent.isOnNavMesh)
            {
                return;
            }

            if (Mathf.Abs(right) < Mathf.Epsilon && Mathf.Abs(forward) < Mathf.Epsilon)
            {
                agent.isStopped = true;
                agent.ResetPath();
                return;
            }

            agent.isStopped = false;
            var inVector = (transform.position - targetCamera.transform.position).normalized;
            var rightVector = Vector3.Cross(Vector3.up, inVector);
            var forwardVector = Vector3.Cross(rightVector, Vector3.up);

            var dir = rightVector * right + forwardVector * forward;

            agent.Move(Time.deltaTime * agent.speed * dir);
        }
    }
}