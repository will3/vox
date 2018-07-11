using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TerrianManager : MonoBehaviour {
    private Terrian terrian;
    public CameraController cameraController;

	// Use this for initialization
	void Start () {
        Assert.IsNotNull(cameraController);

        terrian = new Terrian();
    }
	
	// Update is called once per frame
	void Update () {
        terrian.GenerateForCamera(cameraController.Target);
        terrian.MeshChunks(transform);
	}
}
