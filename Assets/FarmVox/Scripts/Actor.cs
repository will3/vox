using System;
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
        public Camera targetCamera;

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
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }
        }

        private void Update()
        {
            var textures = _spriteSheet.Idle;
            var speed = _spriteSheet.IdleSpeed;
            
            ApplyFrame(textures, speed);

            var right = Input.GetAxisRaw("Horizontal");
            var forward = Input.GetAxisRaw("Vertical");

            if (agent.isOnNavMesh)
            {
                if (Mathf.Abs(right) < Mathf.Epsilon && Mathf.Abs(forward) < Mathf.Epsilon)
                {
                    agent.isStopped = true;
                    agent.ResetPath();
                }
                else
                {
                    agent.isStopped = false;
                    var inVector = (transform.position - targetCamera.transform.position).normalized;
                    var rightVector = Vector3.Cross(Vector3.up, inVector);
                    var forwardVector = Vector3.Cross(rightVector, Vector3.up);

                    var dir = rightVector * right + forwardVector * forward;

                    var dest = agent.nextPosition + dir * 10;
                    if (!agent.SetDestination(dest))
                    {
                        Debug.Log("Failed to set destination");
                    }
                }
            }
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