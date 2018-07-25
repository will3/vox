using UnityEngine;
using System.Collections;
using System.Linq;

namespace FarmVox
{
    public class Actor : MonoBehaviour
    {
        RoutingAgent routingAgent;
        SpriteSheet spriteSheet;

        public RoutingAgent RoutingAgent
        {
            get
            {
                return routingAgent;
            }
        }

        Card card;
        public float radius = 1.0f;
        public Vector3 scale = new Vector3(1.0f, 1.0f, 1.0f);

        void Start()
        {
            spriteSheet = new ArcherSpriteSheet();

            routingAgent = GetComponent<RoutingAgent>();
            routingAgent.radius = radius;

            card = gameObject.AddComponent<Card>();

            var cardScale = scale;
            cardScale.x *= spriteSheet.Scale.x;
            cardScale.y *= spriteSheet.Scale.y;
            cardScale.z *= spriteSheet.Scale.z;
            card.scale = cardScale;

            spriteSheet.Walk();
            card.SetTexture(spriteSheet.CurrentTexture);
        }

        Vector3 getPosition() {
            var pos = routingAgent.position + new Vector3(0, 1, 0);
            pos.y += routingAgent.GetBumpY();
            return pos;
        }

        // Update is called once per frame
        void Update()
        {
            var pos = getPosition();
            card.transform.position = pos;

            if (routingAgent.Moved) {
                spriteSheet.Walk(1.0f);
            } 

            card.SetTexture(spriteSheet.CurrentTexture);
        }

        public void SetGoal(Vector3 to) {
            routingAgent.SetGoal(to);
        }

        public void Push(Vector3 to) {
            routingAgent.Push(to);
        }
    }
}