using UnityEngine;
using System.Collections;
using System.Linq;

namespace FarmVox
{
    public class Actor : MonoBehaviour
    {
        private Card card;
        public Terrian terrian;
        private Vector3Int currentNode;
        float nextWander;
        float wanderWait = 0.1f;
        // Use this for initialization
        void Start()
        {
            card = gameObject.AddComponent<Card>();
            var scale = 2.0f;
            card.transform.localScale = new Vector3(scale, scale, scale);
            updatePosition();
            nextWander = Time.time + wanderWait;
        }

        public void SetNode(Vector3Int node)
        {
            currentNode = node;
            updatePosition();
        }

        private void updatePosition() {
            if (card == null) {
                return;
            }
            card.transform.position = new Vector3(currentNode.x + 0.5f, currentNode.y + 2.0f, currentNode.z + 0.5f);    
        }

        // Update is called once per frame
        void Update()
        {
            //wander();
        }

        private void wander() {
            //if (Time.time < nextWander) {
            //    return;
            //}
            //var nextNodes = terrian.GetNextNodes(currentNode);
            //var index = Mathf.FloorToInt(Random.Range(0.0f, 1.0f) * nextNodes.Count);
            //var nextNode = nextNodes.ElementAt(index);

            //SetNode(nextNode);

            //nextWander = Time.time + wanderWait;
        }
    }
}