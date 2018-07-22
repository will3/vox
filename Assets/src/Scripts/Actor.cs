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

        private Routes GetRoutes() {
            var terrian = Finder.FindTerrian();
            var routesMap = terrian.RoutesMap;
            var origin = routesMap.GetOrigin(position);
            return routesMap.GetRoutes(origin);
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