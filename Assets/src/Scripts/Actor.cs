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
        }

        // Update is called once per frame
        void Update()
        {
            spriteSheet.Walk(1.0f);
            card.SetTexture(spriteSheet.CurrentTexture);
        }

        public void SetDestination(Vector3 to) {
            agent.SetDestination(to);
        }

        public void Push(Vector3 to) {
            //routingAgent.Push(to);
        }
    }
}