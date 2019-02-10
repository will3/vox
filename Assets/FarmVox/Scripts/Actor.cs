using UnityEngine;

namespace FarmVox.Scripts
{
    public class Actor : MonoBehaviour
    {
        private Card _card;
        private Texture2D _texture;
        public Vector3Int Coord;
        public Vector3 Scale;
        public string SpriteSheetName = "blue";
        private ActorSpriteSheet _spriteSheet;

        private float _frameCount;
        private int _frame;
        
        private void Start()
        {
            _card = gameObject.AddComponent<Card>();
            _spriteSheet = ActorSpriteSheetLoader.Load(SpriteSheetName);
            _frameCount = Random.Range(0.0f, 1.0f);
        }

        private void Update()
        {
            gameObject.transform.position = Coord + new Vector3(0.5f, 0, 0.5f);
            _card.Scale = Scale;
            
            var textures = _spriteSheet.Idle;
            var speed = _spriteSheet.IdleSpeed;
            
            ApplyFrame(textures, speed);
        }

        private void ApplyFrame(string[] textures, float speed)
        {
            _frameCount += Time.deltaTime * speed;
            if (_frameCount > 1.0f)
            {
                // Next frame
                _frameCount -= 1.0f;
                
                _frame += 1;
                _frame %= textures.Length;
            }

            _texture = Textures.Load(textures[_frame]);
            _card.Texture = _texture;
        }
    }
}