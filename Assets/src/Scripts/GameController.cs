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
    public bool hideTerrian = false;
    public bool drawPath = false;

    private HighlightHoveredSurface highlight;
    private List<Actor> actors = new List<Actor>();
    private GameObject terrianObject;

    // Use this for initialization
    void Start () {
        terrianObject = new GameObject("terrian");
        terrianObject.transform.parent = transform.parent;

        terrian = new Terrian();
        terrian.Transform = terrianObject.transform;

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
        var routesMap = terrian.RoutesMap;
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            if (result != null) {
                var coord = result.GetCoord();
                coord.y += 2;

                var origin = routesMap.GetOrigin(coord);
                var routes = routesMap.GetRoutes(origin);

                if (routes != null) {
                    var node = routes.GetExistingNode(coord);
                    if (node != null) {
                        var go = new GameObject("robot");
                        var actor = go.AddComponent<Actor>();
                        actor.radius = 2.0f;
                        actor.SetPosition(node.Value);
                        actors.Add(actor);
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            if (result != null) {
                foreach(var robot in actors) {
                    var coord = result.GetCoord() + new Vector3(0.5f, 1.5f, 0.5f);
                    robot.GetComponent<Actor>().Navigate(coord);
                }
            }
        }

        if (hideTerrian) {
            terrianObject.SetActive(false);
        } else {
            terrianObject.SetActive(true);
        }
	}

	void OnDrawGizmos()
	{
        if (drawRoutes) {
            terrian.RoutesMap.DrawGizmos();    
        }

        if (drawPath) {
            foreach (var actor in actors) {
                actor.RoutingAgent.DrawGizmos();
            }
        }
	}
}
