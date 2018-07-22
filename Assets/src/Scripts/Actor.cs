using UnityEngine;
using System.Collections;
using System.Linq;

namespace FarmVox
{
    public class Actor : MonoBehaviour
    {
        public string spriteSheetName = "archer";

        private Card card;
        private Vector3 _position;
        public Vector3 position {
            get {
                return _position;
            }
        }
        public float radius = 0.5f;
        public Vector3 scale = new Vector3(1.0f, 1.0f, 1.0f);

        public void SetPosition(Vector3 position) {
            _position = position;    
        }

        void Start()
        {
            card = gameObject.AddComponent<Card>();
            card.spriteSheetName = spriteSheetName;
            card.scale = scale;
        }

        public void Place(Vector3Int above)
        {
            SetPosition(new Vector3(above.x + 0.5f, above.y + 1.5f, above.z + 0.5f));
        }

        // Update is called once per frame
        void Update()
        {
            card.transform.position = position;
        }

        public static Routes GetRoutes(Vector3 position) {
            var terrian = Finder.FindTerrian();
            if (terrian == null)
            {
                return null;
            }

            var origin = terrian.GetOrigin(position.x, position.y, position.z);
            var tc = terrian.GetTerrianChunk(origin);

            if (tc == null)
            {
                return null;
            }
            return tc.Routes;
        }

        Routes GetRoutes() {
            return GetRoutes(position);
        }

        public void Navigate(Vector3 to) {
            var routes = GetRoutes();

            var nextPosition = routes.Drag(position, to);

            if (nextPosition == null) {
                nextPosition = routes.AStar(position, to);
            }

            if (nextPosition != null) {
                SetPosition(nextPosition.Value);
            }
        }

        public void Drag(Vector3 to) {
            var routes = GetRoutes();

            var nextPosition = routes.Drag(position, to);

            if (nextPosition != null)
            {
                SetPosition(nextPosition.Value);
            }
        }
    }
}