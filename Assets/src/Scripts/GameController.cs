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
    public bool drawPath = false;
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
            if (result != null) {
                var pos = result.HitPos;
                var node = routesMap.GetNode(pos);
                if (routesMap.HasNode(node)) {
                    var go = new GameObject("guy");
                    var actor = go.AddComponent<Actor>();
                    actor.radius = 2.0f;
                    var routingAgent = go.AddComponent<RoutingAgent>();
                    routingAgent.SetRoutesMap(routesMap);
                    routingAgent.position = node;
                    actors.Add(actor);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            if (result != null) {
                var pos = result.HitPos;
                var node = routesMap.GetNode(pos);
                if (routesMap.HasNode(node)) {
                    foreach (var actor in actors) {
                        actor.GetComponent<Actor>().SetGoal(pos);
                    }
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

        if (drawPath) {
            foreach (var actor in actors) {
                actor.RoutingAgent.DrawGizmos();
            }
        }
	}
}
