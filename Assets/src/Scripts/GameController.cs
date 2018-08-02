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

    int spawned = 0;

    HighlightHoveredSurface highlight;

    public Commander commander;

    // Use this for initialization
    void Start () {
        terrian = new Terrian();
        highlight = gameObject.AddComponent<HighlightHoveredSurface>();
        var source = gameObject.AddComponent<VisionSource>();
        source.radius = 100.0f;
        commander = gameObject.GetComponent<Commander>();

        terrian.InitColumns();

        StartCoroutine(terrian.UpdateTerrianLoop());
        StartCoroutine(terrian.UpdateMeshesLoop());
        StartCoroutine(terrian.UpdateWaterfallsLoop());
	}
	
	// Update is called once per frame
	void Update () {
        WorkerQueues.meshQueue.Update();
        WorkerQueues.routingQueue.Update();

        if (spawned > 0)
        {
            if (Spawn()) {
                spawned -= 1;
            }
        }

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
        TaskMap.Instance.OnDrawGizmosSelected();	
	}

    bool Spawn()
    {
        RaycastHit hit;
        var radius = 10.0f;
        var position = new Vector3(Random.Range(-1.0f, 1.0f) * radius, 100, Random.Range(-1.0f, 1.0f) * radius);
        var ray = new Ray(position, Vector3.down);

        if (Physics.Raycast(ray, out hit))
        {
            NavMeshHit navMeshHit;
            if (NavMesh.SamplePosition(hit.point, out navMeshHit, 10.0f, 1 << NavMeshAreas.Walkable))
            {
                var go = new GameObject("guy");
                var actor = go.AddComponent<Actor>();
                actor.radius = 2.0f;
                go.transform.position = navMeshHit.position;
                return true;
            }
        }

        return false;
    }
}
