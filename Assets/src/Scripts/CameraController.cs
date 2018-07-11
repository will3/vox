using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    private Vector3 target;
    private float speed = 20f;
    private float rotateSpeed = 80f;
    private Vector3 velocity;
    private float friction = 0.001f; 

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
        var f = Mathf.Pow(friction, Time.deltaTime);

        var forward = 0.0f;
        if (Input.GetKey(KeyCode.W)) forward += 1.0f;
        if (Input.GetKey(KeyCode.S)) forward -= 1.0f;

        var right = 0.0f;
        if (Input.GetKey(KeyCode.A)) right -= 1.0f;
        if (Input.GetKey(KeyCode.D)) right += 1.0f;

        var rotate = 0.0f;
        if (Input.GetKey(KeyCode.Q)) rotate -= 1.0f;
        if (Input.GetKey(KeyCode.E)) rotate += 1.0f;
        rotation.y += rotate * Time.deltaTime * rotateSpeed;

        var forwardVector = (target - transform.position).normalized;
        forwardVector = Vector3.ProjectOnPlane(forwardVector, Vector3.up);
        var rightVector = Vector3.Cross(Vector3.up, forwardVector);
        velocity += forwardVector * forward * speed * Time.deltaTime;
        velocity += rightVector * right * speed * Time.deltaTime;
        target += velocity;
        velocity *= f;

        var position = target - Quaternion.Euler(rotation) * Vector3.forward * distance;
        transform.position = position;
        transform.LookAt(target, Vector3.up);
	}
}
