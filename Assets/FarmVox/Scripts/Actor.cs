using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace FarmVox.Scripts
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class Actor : MonoBehaviour
    {
        public MeshRenderer meshRenderer;
        public MeshFilter meshFilter;
        public TextAsset spriteSheet;
        public NavMeshAgent agent;

        public float speed = 15;
        public float waterSpeedMultiplier = 0.2f;

        private ActorSpriteSheet _spriteSheet;
        private Texture2D _texture;
        private float _frameCount;
        private int _frame;
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");

        private void Start()
        {
            _spriteSheet = ActorSpriteSheetLoader.Load(spriteSheet);
            _frameCount = Random.Range(0.0f, 1.0f);
            meshFilter.mesh = BuildQuad();

            agent.updatePosition = false;
        }

        private void Update()
        {
            var textures = _spriteSheet.Idle;
            ApplyFrame(textures, _spriteSheet.IdleSpeed);

            transform.position = agent.nextPosition;

            if (agent.enabled)
            {
                if (NavMesh.SamplePosition(agent.nextPosition, out var hit, 1f, NavMesh.AllAreas))
                {
                    var inWater = (hit.mask & 1 << 4) > 0;
                    var speedMultiplier = inWater ? waterSpeedMultiplier : 1;
                    agent.speed = speed * speedMultiplier;
                }
            }
        }

        private void ApplyFrame(IReadOnlyList<string> textures, float speed)
        {
            _frameCount += Time.deltaTime * speed;
            if (_frameCount > 1.0f)
            {
                // Next frame
                _frameCount -= 1.0f;
                
                _frame += 1;
                _frame %= textures.Count;
            }

            _texture = Textures.Load(textures[_frame]);
            
            meshRenderer.material.SetTexture(MainTex, _texture);
        }
        
        private static Mesh BuildQuad()
        {
            var mesh = new Mesh();

            var vertices = new Vector3[4];

            vertices[0] = new Vector3(-0.5f, 0, 0);
            vertices[1] = new Vector3(0.5f, 0, 0);
            vertices[2] = new Vector3(0.5f, 1.0f, 0);
            vertices[3] = new Vector3(-0.5f, 1.0f, 0);

            mesh.vertices = vertices;

            var tri = new int[12];

            tri[0] = 0;
            tri[1] = 1;
            tri[2] = 2;
            tri[3] = 2;
            tri[4] = 3;
            tri[5] = 0;

            tri[6] = 2;
            tri[7] = 1;
            tri[8] = 0;
            tri[9] = 0;
            tri[10] = 3;
            tri[11] = 2;

            mesh.triangles = tri;

            var uv = new Vector2[4];

            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(1, 0);
            uv[2] = new Vector2(1, 1);
            uv[3] = new Vector2(0, 1);

            mesh.uv = uv;

            return mesh;
        }
    }
}