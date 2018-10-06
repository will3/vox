using System.Collections;
using System.Collections.Generic;
using FarmVox;
using UnityEngine;
using UnityEngine.AI;

public class GameController : MonoBehaviour
{
    public bool drawRoutes = false;

    public Terrian Terrian { get; private set; }

    HighlightHoveredSurface highlight;

    // Use this for initialization
    void Start () {
        Terrian = new Terrian();
        highlight = gameObject.AddComponent<HighlightHoveredSurface>();

        Terrian.InitColumns();

        StartCoroutine(Terrian.UpdateTerrianLoop());
        StartCoroutine(Terrian.UpdateMeshesLoop());
        StartCoroutine(Terrian.UpdateWaterfallsLoop());
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
        if (Terrian != null) Terrian.Dispose();
	}

	void OnDrawGizmosSelected()
	{
        //TaskMap.Instance.OnDrawGizmosSelected();	
	}
}
