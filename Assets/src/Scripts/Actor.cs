using UnityEngine;
using System.Collections;
using System.Linq;

namespace FarmVox
{
    public class Actor : MonoBehaviour
    {
        RoutingAgent routingAgent = new RoutingAgent();

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
            card = gameObject.AddComponent<Card>();
            card.spriteSheet = new ArcherSpriteSheet();
            card.scale = scale;
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
        }

        public void Navigate(Vector3 to) {
            routingAgent.Navigate(to);
        }

        public void Drag(Vector3 to) {
            routingAgent.Drag(to);
        }
    }
}