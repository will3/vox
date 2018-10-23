using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 Target = new Vector3(0, 32, 0);
    public float Speed = 20f;
    public float RotateSpeed = 80f;
    public Vector3 Velocity;
    public float Friction = 0.001f;
    public float ZoomRate = 1.1f;
    public float MouseRotateSpeed = 0.2f;

    public float RotateX = 45;
    public Vector3 Rotation = new Vector3(45, 45, 0);
    public float Distance = 300;
    
    private Vector3 _lastDragPosition;
    private bool _dragging;
    
    public Vector3 GetVector() {
        return (Target - transform.position).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        var f = Mathf.Pow(Friction, Time.deltaTime);

        var forward = 0.0f;
        if (Input.GetKey(KeyCode.W)) forward += 1.0f;
        if (Input.GetKey(KeyCode.S)) forward -= 1.0f;

        var right = 0.0f;
        if (Input.GetKey(KeyCode.A)) right -= 1.0f;
        if (Input.GetKey(KeyCode.D)) right += 1.0f;

        var rotate = 0.0f;
        if (Input.GetKey(KeyCode.Q)) rotate -= 1.0f;
        if (Input.GetKey(KeyCode.E)) rotate += 1.0f;
        Rotation.x = RotateX;
        Rotation.y += rotate * Time.deltaTime * RotateSpeed;

        if (Input.GetKeyDown(KeyCode.Equals)) {
            Distance /= ZoomRate;
        }

        if (Input.GetKeyDown(KeyCode.Minus)) {
            Distance *= ZoomRate;
        }

        if (!_dragging && Input.GetKey(KeyCode.Mouse1)) {
            _dragging = true;
            _lastDragPosition = Input.mousePosition;
        }

        if (_dragging && !Input.GetKey(KeyCode.Mouse1)) {
            _dragging = false;
        }

        if (_dragging)
        {
            var diff = Input.mousePosition - _lastDragPosition;
            Rotation.y -= diff.x * MouseRotateSpeed;
            _lastDragPosition = Input.mousePosition;
        }
    
        var forwardVector = (Target - transform.position).normalized;
        forwardVector = Vector3.ProjectOnPlane(forwardVector, Vector3.up);
        var rightVector = Vector3.Cross(Vector3.up, forwardVector);
        Velocity += forwardVector * forward * Speed * Time.deltaTime;
        Velocity += rightVector * right * Speed * Time.deltaTime;
        Target += Velocity;
        Velocity *= f;

        var position = Target - Quaternion.Euler(Rotation) * Vector3.forward * Distance;
        transform.position = position;
        transform.LookAt(Target, Vector3.up);
	}
}
