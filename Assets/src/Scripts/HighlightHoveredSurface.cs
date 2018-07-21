using UnityEngine;
using System.Collections;
using FarmVox;

public class HighlightHoveredSurface : MonoBehaviour
{
    private Terrian terrian;

    private Terrian GetTerrian() {
        if (terrian == null) {
            terrian = GameObject.FindWithTag("GameController").GetComponent<GameController>().Terrian;
        }
        return terrian;
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
        terrian = GetTerrian();
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);



        var result = terrian.Trace(ray.origin, ray.direction, 400);
        if (result != null) {
            var mesh = result.GetQuad();
            var pos = result.GetCoord();
            go.GetComponent<MeshFilter>().mesh = mesh;
            go.transform.position = pos + result.HitNormal * 0.1f;
        }
	}
}
