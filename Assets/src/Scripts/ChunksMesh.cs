using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class ChunksMesh : MonoBehaviour
{
    private Chunks chunks = new Chunks(32);

    public Chunks Chunks
    {
        get
        {
            return chunks;
        }
    }

    private Material material;

	// Use this for initialization
	void Start()
	{
        material = new Material(Shader.Find("Standard"));
	}

	// Update is called once per frame
	void Update()
	{
        Mesher.MeshChunks(chunks, transform, material);
	}
}
