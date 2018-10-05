using System.Collections;
using System.Collections.Generic;
using FarmVox;
using UnityEngine;
using UnityEngine.AI;

public class GameController : MonoBehaviour
{
    public bool drawRoutes = false;
    Terrian terrian;
    readonly VisionMap visionMap = new VisionMap(512, new Vector2Int(-256, -256));

    public VisionMap VisionMap { get { return visionMap; } }

    public Terrian Terrian
    {
        get
        {
            return terrian;
        }
    }

    HighlightHoveredSurface highlight;

    // Use this for initialization
    void Start () {
        terrian = new Terrian();
        highlight = gameObject.AddComponent<HighlightHoveredSurface>();
        var source = gameObject.AddComponent<VisionSource>();
        source.radius = 100.0f;

        terrian.InitColumns();

        StartCoroutine(terrian.UpdateTerrianLoop());
        StartCoroutine(terrian.UpdateMeshesLoop());
        StartCoroutine(terrian.UpdateWaterfallsLoop());
	}
	
	// Update is called once per frame
	void Update () {
        WorkerQueues.meshQueue.Update();
        WorkerQueues.routingQueue.Update();

        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            //RaycastHit hit;
            //var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //if (Physics.Raycast(ray, out hit))
            //{
            //    foreach (var actor in actors) {
            //        actor.SetFormationDestination(hit.point);
            //    }
            //}
        }
    }

	void OnDestroy()
	{
        if (visionMap != null) visionMap.Dispose();
        if (terrian != null) terrian.Dispose();
	}

	void OnDrawGizmosSelected()
	{
        //TaskMap.Instance.OnDrawGizmosSelected();	
	}
}
