using System.Collections;
using System.Collections.Generic;
using FarmVox;
using UnityEngine;
using UnityEngine.Assertions;

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

    bool spawned = false;

    HighlightHoveredSurface highlight;
    List<Actor> actors = new List<Actor>();

    // Use this for initialization
    void Start () {

        terrian = new Terrian();

        var go = new GameObject("Highlight");
        highlight = go.AddComponent<HighlightHoveredSurface>();
        gameObject.AddComponent<VisionSource>();
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
    }

	void OnDestroy()
	{
        visionMap.Dispose();
	}
}
