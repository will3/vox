using UnityEngine;
using System.Collections;
using System.Linq;

namespace FarmVox
{
    public class Actor : MonoBehaviour
    {
        public string spriteSheetName = "archer";

        private RoutingAgent routingAgent = new RoutingAgent();
        private Card card;
        public float radius = 0.5f;
        public Vector3 scale = new Vector3(1.0f, 1.0f, 1.0f);

        void Start()
        {
            card = gameObject.AddComponent<Card>();
            card.spriteSheetName = spriteSheetName;
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
            card.transform.position = routingAgent.position + new Vector3(1, 1, 1);
        }

        public void Navigate(Vector3 to) {
            routingAgent.Navigate(to);
        }

        public void Drag(Vector3 to) {
            routingAgent.Drag(to);
        }
    }
}