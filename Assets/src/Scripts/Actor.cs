using UnityEngine;
using System.Collections;
using System.Linq;

namespace FarmVox
{
    public class Actor : MonoBehaviour
    {
        readonly RoutingAgent routingAgent = new RoutingAgent();
        SpriteSheet spriteSheet;

        public RoutingAgent RoutingAgent
        {
            get
            {
                return routingAgent;
            }
        }

        Card card;
        public float radius = 0.5f;
        public Vector3 scale = new Vector3(1.0f, 1.0f, 1.0f);

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
        }

        public void SetPosition(Vector3 position) {
            routingAgent.position = position;
        }

        public Vector3 position {
            get {
                return routingAgent.position;
            }
        }

        // Update is called once per frame
        void Update()
        {
            card.transform.position = routingAgent.position + new Vector3(1, -1, 1);
            routingAgent.Update();

            spriteSheet.Walk();
            card.SetTexture(spriteSheet.CurrentTexture);
        }

        public void Navigate(Vector3 to) {
            routingAgent.Navigate(to);
        }

        public void Drag(Vector3 to) {
            routingAgent.Drag(to);
        }
    }
}