using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TerrianManager : MonoBehaviour {
    private ChunksMesh chunksMesh;
    private Terrian terrian;
    public CameraController cameraController;

	// Use this for initialization
	void Start () {
        Assert.IsNotNull(cameraController);

        chunksMesh = gameObject.AddComponent<ChunksMesh>();
        terrian = new Terrian(chunksMesh.Chunks);
        terrian.GenerateForCamera(cameraController.Target);
    }
	
	// Update is called once per frame
	void Update () {
        
	}
}
