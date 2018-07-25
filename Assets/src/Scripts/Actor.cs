using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.AI;

namespace FarmVox
{
    public class Actor : MonoBehaviour
    {
        SpriteSheet spriteSheet;

        Card card;
        public float radius = 1.0f;
        public Vector3 scale = new Vector3(1.0f, 1.0f, 1.0f);
        NavMeshAgent agent;
        Vector3? destination;
        Vector3 lastPosition;

        void Start()
        {
            spriteSheet = new ArcherSpriteSheet();

            card = gameObject.AddComponent<Card>();

            var cardScale = scale;
            cardScale.x *= spriteSheet.Scale.x;
            cardScale.y *= spriteSheet.Scale.y;
            cardScale.z *= spriteSheet.Scale.z;
            card.scale = cardScale;

            spriteSheet.Walk();
            card.SetTexture(spriteSheet.CurrentTexture);

            agent = gameObject.AddComponent<NavMeshAgent>();
            agent.radius = 0.2f;
            agent.height = 0.4f;
            agent.speed = 24;
            agent.angularSpeed = 120;
            agent.acceleration = 1000;
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;

            started = true;
        }

        float stopDistance = 1.0f;
        Vector3 smoothVelocity = new Vector3();
        float smoothRatio = 0.8f;
        bool started = false;

        // Update is called once per frame
        void Update()
        {
            if (!started) {
                return;
            }

            var velocity = transform.position - lastPosition;
            velocity.y = 0;

            var walking = agent.desiredVelocity.magnitude > 0;
            if (walking) {
                //var amount = velocity.magnitude * 4.0f;
                //if (amount < 0.3f) {
                //    amount = 0.3f;
                //}
                var amount = 1.0f;
                spriteSheet.Walk(amount);    
            } else {
                
            }

            card.SetTexture(spriteSheet.CurrentTexture);

            smoothVelocity += velocity;
            smoothVelocity *= smoothRatio;

            if (destination != null)
            {
                var distance = (destination.Value - transform.position).magnitude;

                if (distance < 10) {
                    if (smoothVelocity.magnitude < 0.2f && stopDistance < distance)
                    {
                        stopDistance *= 1.6f;
                    }
                } else if (distance < 20) {
                    if (smoothVelocity.magnitude < 0.1f && stopDistance < distance)
                    {
                        stopDistance *= 1.1f;
                    }
                }
            }

            agent.stoppingDistance = stopDistance;

            lastPosition = transform.position;
        }

        public void SetDestination(Vector3 destination) {
            agent.SetDestination(destination);
            this.destination = destination;
            stopDistance = 1.0f;
            agent.isStopped = false;
        }
    }
}