using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.AI;
using System.Collections.Generic;

namespace FarmVox
{
    public class Actor : MonoBehaviour
    {
        SpriteSheet spriteSheet;
        Card card;

        public float radius = 1.0f;
        public Vector3 scale = new Vector3(1.0f, 1.0f, 1.0f);
        public float digSpeed = 0.4f;

        NavMeshAgent agent;
        Vector3? formationDestination;
        Vector3 lastPosition;
        VisionSource visionSource;

        public Vector3 Destination
        {
            get
            {
                return agent.destination;
            }
        }

        //float formationStopDistance = 1.0f;
        Vector3 smoothVelocity = new Vector3();
        float smoothRatio = 0.8f;
        bool started = false;
        Task currentTask = null;

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
            agent.speed = 20;
            agent.angularSpeed = 120;
            agent.acceleration = 1000;
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;

            visionSource = gameObject.AddComponent<VisionSource>();

            started = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (!started) {
                return;
            }

            UpdateFormationMovement();
            UpdateWalkAnimation();
            UpdateTask();
        }


        void UpdateTask() {
            if (itemWeight >= capacity && 
                (currentTask == null || currentTask.type != TaskType.StoreInventory)) {
                var task = new StoreInventoryTask();
                currentTask = task;
            }

            if (currentTask == null) {
                currentTask = TaskMap.Instance.FindTask(Vectors.FloorToInt(transform.position));
                if (currentTask != null) {
                    TaskMap.Instance.RemoveTask(currentTask);
                }
            }

            if (currentTask != null) {
                currentTask.Perform(this);

                if (currentTask.done)
                {
                    currentTask = null;
                }
            }
        }

        void UpdateWalkAnimation() {
            var velocity = transform.position - lastPosition;
            velocity.y = 0;

            var walking = agent.desiredVelocity.magnitude > 0;
            if (walking)
            {
                //var amount = velocity.magnitude * 4.0f;
                //if (amount < 0.3f) {
                //    amount = 0.3f;
                //}
                var amount = 1.0f;
                spriteSheet.Walk(amount);
            }

            card.SetTexture(spriteSheet.CurrentTexture);

            smoothVelocity += velocity;
            smoothVelocity *= smoothRatio;
            lastPosition = transform.position;
        }

        void UpdateFormationMovement() {
            // TODO
            //if (Destination != null)
            //{
            //    var distance = (Destination - transform.position).magnitude;

            //    if (distance < 10)
            //    {
            //        if (smoothVelocity.magnitude < 0.2f && formationStopDistance < distance)
            //        {
            //            formationStopDistance *= 1.6f;
            //        }
            //    }
            //    else if (distance < 20)
            //    {
            //        if (smoothVelocity.magnitude < 0.1f && formationStopDistance < distance)
            //        {
            //            formationStopDistance *= 1.1f;
            //        }
            //    }
            //    agent.stoppingDistance = formationStopDistance;
            //}
        }

        public void SetDestination(Vector3 destination, float stopDistance) {
            agent.SetDestination(destination);
            agent.isStopped = false;
            agent.stoppingDistance = stopDistance;
        }

        // TODO
        public void SetFormationDestination(Vector3 destination) {
            agent.SetDestination(destination);
            agent.isStopped = false;
            //formationStopDistance = 1.0f;
        }

        public bool ArrivedDestination() {
            float dist = agent.remainingDistance;
            //!Mathf.Approximately(dist, Mathf.Infinity) && 
            if (agent.pathStatus == NavMeshPathStatus.PathComplete && dist <= agent.stoppingDistance)
            {
                return true;
            }
            return false;
        }

        List<Item> items = new List<Item>();

        public List<Item> Items
        {
            get
            {
                return items;
            }
        }

        float capacity = 4.0f;
        float itemWeight = 0.0f;

        public void AddItem(Item item) {
            items.Add(item);
            itemWeight += item.weight;
        }

        public void RemoveItem(Item item) {
            items.Remove(item);
            itemWeight -= item.weight;
        }
    }
}