using UnityEngine;
using System.Collections;
using System.IO;
using FarmVox;

public class Card : MonoBehaviour
{
    private Material material;
    private MeshFilter meshFilter;
    private Vector3 up = Vector3.up;
    private CameraController cameraController;
    public string textureName = "tiny_top";

	// Use this for initialization
	void Start()
	{
        material = new Material(Shader.Find("Unlit/Transparent"));
        var texture = Resources.Load<Texture2D>("Textures/" + textureName);
        material.SetTexture("_MainTex", texture);

        var mesh = getQuad();

        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        gameObject.GetComponent<Renderer>().material = material;
        gameObject.GetComponent<MeshFilter>().mesh = mesh;

        cameraController = Camera.main.GetComponent<CameraController>();
	}

    private Mesh getQuad() {
        var mesh = new Mesh();

        var vertices = new Vector3[4];

        //vertices[0] = new Vector3(-0.5f, 0, 0);
        //vertices[1] = new Vector3(0.5f, 0, 0);
        //vertices[2] = new Vector3(0.5f, 1.0f, 0);
        //vertices[3] = new Vector3(-0.5f, 1.0f, 0);

        vertices[0] = new Vector3(-0.5f, -0.5f, 0);
        vertices[1] = new Vector3(0.5f, -0.5f, 0);
        vertices[2] = new Vector3(0.5f, 0.5f, 0);
        vertices[3] = new Vector3(-0.5f, 0.5f, 0);

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
	void Update()
	{
        // Linear billboard
        //var forward = cameraController.GetVector();
        //var right = Vector3.Cross(up, forward);
        //var face = Vector3.Cross(right, up);

        //transform.LookAt(transform.position + face, Vector3.up);


        // Billboard
        transform.LookAt(cameraController.transform.position, Vector3.up);
	}
}
