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

    int spawned = 1;

    HighlightHoveredSurface highlight;
    List<Actor> actors = new List<Actor>();

    public Commander commander;

    // Use this for initialization
    void Start () {
        terrian = new Terrian();
        highlight = gameObject.AddComponent<HighlightHoveredSurface>();
        var source = gameObject.AddComponent<VisionSource>();
        source.radius = 100.0f;
        commander = gameObject.GetComponent<Commander>();
	}
	
    bool Spawn() {
        RaycastHit hit;
        var radius = 10.0f;
        var position = new Vector3(Random.Range(-1.0f, 1.0f) * radius, 100, Random.Range(-1.0f, 1.0f) * radius);
        var ray = new Ray(position, Vector3.down);

        if (Physics.Raycast(ray, out hit))
        {
            NavMeshHit navMeshHit;
            if (NavMesh.SamplePosition(hit.point, out navMeshHit, 10.0f, 1 << NavMeshAreas.Walkable)) {
                var go = new GameObject("guy");
                var actor = go.AddComponent<Actor>();
                actor.radius = 2.0f;
                actors.Add(actor);
                go.transform.position = navMeshHit.position;
                return true;
            }
        }

        return false;
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
        }

        if (spawned > 0)
        {
            if (Spawn()) {
                spawned -= 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                foreach (var actor in actors) {
                    actor.SetFormationDestination(hit.point);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.S)) {
            terrian.ShadowMap.Rebuild();
        }
    }

	void OnDestroy()
	{
        if (visionMap != null) visionMap.Dispose();
        if (terrian != null) terrian.Dispose();
	}

	void OnDrawGizmosSelected()
	{
        TaskMap.Instance.OnDrawGizmosSelected();	
	}
}
