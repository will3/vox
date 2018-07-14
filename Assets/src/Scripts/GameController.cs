using System.Collections;
using System.Collections.Generic;
using FarmVox;
using UnityEngine;
using UnityEngine.Assertions;

public class GameController : MonoBehaviour {
    private Terrian terrian;
    private CameraController cameraController;
    private bool spawned = false;
    private HighlightHoveredSurface highlight;

	// Use this for initialization
	void Start () {
        cameraController = Camera.main.GetComponent<CameraController>();
        Assert.IsNotNull(cameraController);

        terrian = new Terrian();
        terrian.Transform = transform;

        highlight = gameObject.AddComponent<HighlightHoveredSurface>();
        highlight.terrian = terrian;
	}
	
	// Update is called once per frame
	void Update () {
        terrian.Target = cameraController.Target;
        terrian.Update();
        if (!spawned) {
            terrian.SpawnDwarfs();
            spawned = true;
        }
	}

	private void OnDrawGizmos()
	{
        if (terrian != null) {
            foreach (var kv in terrian.Map)
            {
                var terrianChunk = kv.Value;
                terrianChunk.DrawRoutesGizmos();
            }    
        }
	}
}
