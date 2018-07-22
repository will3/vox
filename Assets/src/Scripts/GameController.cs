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
    private List<GameObject> robots = new List<GameObject>();
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
                var coord = result.GetCoord();
                var robot = new GameObject("robot");
                var actor = robot.AddComponent<Actor>();
                actor.SetNode(coord);
                robots.Add(robot);
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            if (result != null) {
                foreach(var robot in robots) {
                    robot.GetComponent<Actor>().Goto(result.GetCoord());
                }
            }
        }

        if (hideTerrian) {
            terrianObject.SetActive(false);
        } else {
            terrianObject.SetActive(true);
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
