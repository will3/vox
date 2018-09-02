using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FarmVox;
using UnityEngine.AI;

public class TerrianNavMeshBuilder : MonoBehaviour {
    Chunks[] layersToBuildNavMesh;
    NavMeshSurface navMeshSurface;
    static float nextBuildTime;
    static bool navMeshNeedsUpdate = false;

	// Use this for initialization
	void Start () {
        navMeshSurface = GetComponent<NavMeshSurface>();
	}
	
	// Update is called once per frame
	void Update () {
        if (navMeshNeedsUpdate && Time.time > nextBuildTime) {
            navMeshSurface.BuildNavMesh();
            navMeshNeedsUpdate = false;
        }
	}

    public static void TriggerBuild() {
        // Debounce
        nextBuildTime = Time.time + 0.2f;
        navMeshNeedsUpdate = true;
    }
}
