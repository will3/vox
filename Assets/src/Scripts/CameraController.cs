using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    private Vector3 target;

    public Vector3 Target
    {
        get
        {
            return target;
        }
    }

    private Vector3 rotation = new Vector3(45, 45, 0);
    private float distance = 200;

	// Use this for initialization
	void Start()
	{
        
	}

	// Update is called once per frame
	void Update()
	{
        var position = target - Quaternion.Euler(rotation) * Vector3.forward * distance;
        transform.position = position;
        transform.LookAt(target, Vector3.up);	
	}
}
