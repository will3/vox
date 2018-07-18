using System.Collections;
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

    public GameObject block;

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

        Spawn();
	}

    private GameObject Spawn() {
        var go = Instantiate(block);
        var color = Colors.special;
        var mesh = go.GetComponent<MeshFilter>().mesh;
        var colors = new List<Color>();
        for (var i = 0; i < mesh.vertices.Length; i++) {
            colors.Add(color);
        }
        mesh.SetColors(colors);
        return go;
    }
	
	// Update is called once per frame
	void Update () {
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
