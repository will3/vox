using UnityEngine;
using System.Collections;
using FarmVox;

public class HighlightHoveredSurface : MonoBehaviour
{
    private Terrian _terrian;

    private Terrian terrian {
        get {
            if (_terrian == null)
            {
                _terrian = GameObject.FindWithTag("GameController").GetComponent<GameController>().Terrian;
            }
            return _terrian;    
        }
    }

    private GameObject go;

	// Use this for initialization
	void Start()
	{
        go = new GameObject("highlight");
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();
	}

	// Update is called once per frame
	void Update()
	{
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (terrian == null) { return; }

        var result = terrian.Trace(ray.origin, ray.direction, 400);
        if (result != null) {
            var mesh = result.GetQuad();
            var pos = result.GetCoord();
            go.GetComponent<MeshFilter>().mesh = mesh;
            go.transform.position = pos + result.HitNormal * 0.1f;
        }
	}

    public RaycastResult Trace() {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return terrian.Trace(ray.origin, ray.direction, 400);
    }
}
