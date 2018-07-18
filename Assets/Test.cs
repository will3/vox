using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FarmVox;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var noise = new Perlin3DGPU(new Noise(), 32, new Vector3());
        noise.Dispatch();
        var results = noise.Read();
        noise.Dispose();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
