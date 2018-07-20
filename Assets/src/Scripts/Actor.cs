using UnityEngine;
using System.Collections;
using System.Linq;

namespace FarmVox
{
    public class Actor : MonoBehaviour
    {
        public Terrian terrian;
        public string spriteSheetName = "monster";

        private Card card;
        private Vector3Int currentNode;
        float nextWander;
        float wanderWait = 0.1f;

        void Start()
        {
            card = gameObject.AddComponent<Card>();
            card.spriteSheetName = spriteSheetName;
            var scale = 2.0f;
            card.transform.localScale = new Vector3(scale, scale, scale);
            UpdatePosition();
            nextWander = Time.time + wanderWait;
        }

        public void SetNode(Vector3Int node)
        {
            currentNode = node;
            UpdatePosition();
        }

        private void UpdatePosition() {
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