using System.Collections;
using System.Collections.Generic;
using FarmVox;
using UnityEngine;
using UnityEngine.Assertions;

public class GameController : MonoBehaviour {
    private Terrian terrian;
    private CameraController cameraController;

	// Use this for initialization
	void Start () {
        cameraController = Camera.main.GetComponent<CameraController>();
        Assert.IsNotNull(cameraController);

        terrian = new Terrian();
        terrian.Transform = transform;

        spawnInitial();
	}

    void spawnInitial() {
        
    }
	
	// Update is called once per frame
	void Update () {
        terrian.Target = cameraController.Target;
        terrian.Update();
	}
}
