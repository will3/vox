using FarmVox.Terrain;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class Actor : MonoBehaviour
    {
        private Terrian _terrian;
        private Card _card;
        private Texture2D _texture;
        public Vector3Int Coord;
        public Vector3 Scale;
        
        private void Start()
        {
            _terrian = Terrian.Instance;
            _card = gameObject.AddComponent<Card>();
            _texture = Textures.Load("blue_0");
            _card.Texture = _texture;
        }

        private void Update()
        {
            gameObject.transform.position = Coord + new Vector3(0.5f, 0, 0.5f);
            _card.Scale = Scale;
        }
    }
}