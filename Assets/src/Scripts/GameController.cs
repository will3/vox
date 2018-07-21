﻿using System.Collections;
using System.Collections.Generic;
using FarmVox;
using UnityEngine;
using UnityEngine.Assertions;

public class GameController : MonoBehaviour {
    public bool drawRoutes = false;
    private Terrian terrian;

    public Terrian Terrian
    {
        get
        {
            return terrian;
        }
    }

    private bool spawned = false;
    private GameObject highlight;

    public WorkerQueue queue = new WorkerQueue();

    public GameObject Highlight
    {
        get
        {
            return highlight;
        }
    }

    // Use this for initialization
    void Start () {
        terrian = new Terrian();
        terrian.Transform = transform;

        highlight = new GameObject("Highlight");
        highlight.AddComponent<HighlightHoveredSurface>();
	}
	
	// Update is called once per frame
	void Update () {
        queue.Update();
        var cameraController = Finder.FindCameraController();
        if (cameraController != null) {
            terrian.Target = cameraController.Target;
            terrian.Update();
            if (!spawned)
            {
                terrian.SpawnDwarfs();
                spawned = true;
            }    
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
