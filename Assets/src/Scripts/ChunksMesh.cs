using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class ChunksMesh : MonoBehaviour
{
    private Chunks chunks = new Chunks(32);
    public Material material;

    public Chunks Chunks
    {
        get
        {
            return chunks;
        }
    }

	// Use this for initialization
	void Start()
	{
        //material = material ?? new Material(Shader.Find("Unlit/Color"));
        material = material ?? new Material(Shader.Find("Standard"));
	}

	// Update is called once per frame
	void Update()
	{
        Mesher.MeshChunks(chunks, transform, material);
	}
}
