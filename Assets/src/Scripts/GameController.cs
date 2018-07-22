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
        if (Input.GetKey(KeyCode.Mouse0)) {
            if (result != null) {
                var coord = result.GetCoord();

                var routes = Actor.GetRoutes(coord);

                if (routes != null && routes.HasCoord(coord)) {
                    var go = new GameObject("robot");
                    var actor = go.AddComponent<Actor>();
                    actor.radius = 2.0f;
                    actor.scale = new Vector3(0.7f, 1.0f, 0.7f) * 14.0f;
                    actor.Place(coord);
                    actors.Add(actor);
                }
            }
        }

        if (Input.GetKey(KeyCode.Mouse1)) {
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

        UpdatePhysics();
        UpdatePhysics();
	}

    private void UpdatePhysics() {
        for (var i = 0; i < actors.Count; i++)
        {
            for (var j = i; j < actors.Count; j++) {
                if (i == j) {
                    continue;
                }

                var a = actors[i];
                var b = actors[j];
                var factor = 1.0f;

                var diff = (b.position - a.position);
                if (diff.magnitude == 0.0f) {
                    diff = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)).normalized * 0.01f;
                }

                var dis = diff.magnitude;
                if (dis < a.radius + b.radius) {
                    var dir = diff.normalized;
                    var force = dir * (a.radius + b.radius - dis) * factor;
                    b.Drag(b.position + force);
                    a.Drag(a.position - force);
                }
            }
        }
    }

	private void OnDrawGizmos()
	{
        if (drawRoutes && terrian != null) {
            foreach (var kv in terrian.Map)
            {
                var terrianChunk = kv.Value;
                terrianChunk.Routes.DrawGizmos();
            }    
        }
	}
}
