using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace FarmVox.Scripts
{
    public class Card : MonoBehaviour
    {
        private Material _material;
        private MeshFilter _meshFilter;
        private readonly Vector3 _up = Vector3.up;
        private CameraController _cameraController;
        public Vector3 Scale = new Vector3(1.0f, 1.0f, 1.0f);

        public Texture2D Texture;

        private Texture2D _lastTexture;
        
        // Use this for initialization
        private void Start()
        {
            _material = _material ? _material : Materials.GetSpriteMaterial();

            var mesh = GetQuad();

            gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<MeshRenderer>();
            gameObject.GetComponent<Renderer>().material = _material;
            gameObject.GetComponent<MeshFilter>().mesh = mesh;

            Debug.Assert(Camera.main != null, "Camera.main != null");
            _cameraController = Camera.main.GetComponent<CameraController>();
        }

        private static Mesh GetQuad()
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
        
        // Update is called once per frame
        private void Update()
        {
            // Update material
            _material.SetTexture("_MainTex", Texture);
            
            // Linear billboard
            var forward = _cameraController.GetVector();
            var right = Vector3.Cross(_up, forward);
            var face = Vector3.Cross(right, _up);

            transform.LookAt(transform.position + face, Vector3.up);

            var localScale = Scale;

            transform.localScale = localScale;
        }
    }
}