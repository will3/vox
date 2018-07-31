using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 target = new Vector3(0, 32, 0);
    float speed = 20f;
    float rotateSpeed = 80f;
    Vector3 velocity;
    float friction = 0.001f;
    float zoomRate = 1.1f;
    float mouseRotateSpeed = 0.2f;

    public Vector3 GetVector() {
        return (Target - transform.position).normalized;
    }

    public Vector3 Target
    {
        get
        {
            return target;
        }
    }

    public float rotateX = 45;
    Vector3 rotation = new Vector3(45, 45, 0);
    float distance = 200;
    Vector3 lastDragPosition;

    bool dragging;

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
        rotation.x = rotateX;
        rotation.y += rotate * Time.deltaTime * rotateSpeed;

        if (Input.GetKeyDown(KeyCode.Equals)) {
            distance /= zoomRate;
        }

        if (Input.GetKeyDown(KeyCode.Minus)) {
            distance *= zoomRate;
        }

        if (!dragging && Input.GetKey(KeyCode.Mouse1)) {
            dragging = true;
            lastDragPosition = Input.mousePosition;
        }

        if (dragging && !Input.GetKey(KeyCode.Mouse1)) {
            dragging = false;
        }

        if (dragging)
        {
            var diff = Input.mousePosition - lastDragPosition;
            rotation.y -= diff.x * mouseRotateSpeed;
            lastDragPosition = Input.mousePosition;
        }
    
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
