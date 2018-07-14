using UnityEngine;
using System.Collections;
using FarmVox;

public class Test : MonoBehaviour
{
    private Layer layer;
    private Material material;
	// Use this for initialization
	void Start()
	{
        layer = new Layer();
        material = new Material(Shader.Find("Unlit/voxelunlit"));
        layer.Chunks.Set(1, 1, 1, 1);
        layer.Chunks.SetColor(1, 1, 1, new Color(1.0f, 1.0f, 1.0f));

        layer.Chunks.Set(1, 2, 1, 1);
        layer.Chunks.SetColor(1, 2, 1, new Color(1.0f, 1.0f, 1.0f));

        layer.Chunks.Set(1, 1, 2, 1);
        layer.Chunks.SetColor(1, 1, 2, new Color(1.0f, 1.0f, 1.0f));
        layer.Draw(new Vector3Int(), transform, material);
	}

	// Update is called once per frame
	void Update()
	{
	}
}
