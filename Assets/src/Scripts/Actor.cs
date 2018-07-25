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
        float smoothRatio = 0.5f;
        bool started = false;

        // Update is called once per frame
        void Update()
        {
            if (!started) {
                return;
            }

            var walking = agent.desiredVelocity.magnitude > 0;
            if (walking) {
                spriteSheet.Walk(1.0f);    
            }
            card.SetTexture(spriteSheet.CurrentTexture);

            var velocity = transform.position - lastPosition;
            smoothVelocity += velocity;
            smoothVelocity *= smoothRatio;
            agent.stoppingDistance = stopDistance;
            lastPosition = transform.position;

            if (destination != null)
            {
                var distance = (destination.Value - transform.position).magnitude;
                if (smoothVelocity.magnitude < 0.1f)
                {
                    stopDistance *= 1.2f;
                }
            }

            if (Mathf.Approximately(agent.desiredVelocity.magnitude, 0f)) {
                stopDistance = 1.0f;
                destination = null;
            }
        }

        public void SetDestination(Vector3 destination) {
            agent.SetDestination(destination);
            this.destination = destination;
            stopDistance = 1.0f;
        }
    }
}