using System.Collections;
using System.Collections.Generic;
using FarmVox;
using UnityEngine;
using UnityEngine.Assertions;

public class GameController : MonoBehaviour {
    public bool drawRoutes = false;
    Terrian terrian;

    public Terrian Terrian
    {
        get
        {
            return terrian;
        }
    }

    RoutesMap routesMap;

    public RoutesMap RoutesMap
    {
        get
        {
            return routesMap;
        }
    }

    private bool spawned = false;
    public bool hideTerrian = false;
    public bool hideTrees = false;

    private HighlightHoveredSurface highlight;
    private List<Actor> actors = new List<Actor>();
    private GameObject terrianObject;

    // Use this for initialization
    void Start () {
        terrianObject = new GameObject("terrian");
        terrianObject.transform.parent = transform.parent;

        terrian = new Terrian();
        terrian.Transform = terrianObject.transform;

        routesMap = new RoutesMap(terrian);

        var go = new GameObject("Highlight");
        highlight = go.AddComponent<HighlightHoveredSurface>();
	}
	
	// Update is called once per frame
	void Update () {
        if (terrian == null) {
            return;
        }

        WorkerQueues.meshQueue.Update();
        WorkerQueues.routingQueue.Update();

        var cameraController = Finder.FindCameraController();
        if (cameraController != null) {
            terrian.Target = cameraController.Target;
            terrian.Update();
            if (!spawned)
            {
                // TODO
                spawned = true;
            }    
        }

        var result = highlight.Trace();

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit)) {
                var go = new GameObject("guy");
                var actor = go.AddComponent<Actor>();
                actor.radius = 2.0f;
                actors.Add(actor);
                go.transform.position = hit.point;
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                foreach (var actor in actors) {
                    actor.SetDestination(hit.point);
                }
            }
        }

        if (hideTerrian) {
            terrianObject.SetActive(false);
        } else {
            terrianObject.SetActive(true);
        }

        terrian.TreeLayer.SetActive(!this.hideTrees);

        routesMap.Update();
	}

	void OnDrawGizmos()
	{
        if (drawRoutes) {
            routesMap.DrawGizmos();    
        }
	}
}
