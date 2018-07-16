﻿using System.Collections;
using System.Collections.Generic;
using FarmVox;
using UnityEngine;
using UnityEngine.Assertions;

public class GameController : MonoBehaviour {
    public bool drawRoutes = false;
    private Terrian terrian;
    private CameraController cameraController;

    public CameraController CameraController
    {
        get
        {
            if (cameraController == null) {
                cameraController = Camera.main.GetComponent<CameraController>();
            }
            return cameraController;
        }
    }

    private bool spawned = false;
    private GameObject highlight;

    public GameObject Highlight
    {
        get
        {
            return highlight;
        }
    }

    // Use this for initialization
    void Start () {
        Assert.IsNotNull(CameraController);

        terrian = new Terrian();
        terrian.Transform = transform;

        highlight = new GameObject("Highlight");
        highlight.AddComponent<HighlightHoveredSurface>();
        highlight.GetComponent<HighlightHoveredSurface>().terrian = terrian;
        highlight.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        terrian.Target = CameraController.Target;
        terrian.Update();
        if (!spawned) {
            terrian.SpawnDwarfs();
            spawned = true;
        }
	}

	private void OnDrawGizmos()
	{
        if (drawRoutes && terrian != null) {
            foreach (var kv in terrian.Map)
            {
                var terrianChunk = kv.Value;
                terrianChunk.DrawRoutesGizmos();
            }    
        }
	}
}
