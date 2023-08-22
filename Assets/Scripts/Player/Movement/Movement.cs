using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private Transform orientation;
    [SerializeField] private LayerMask platformLayer;

    private MoveLift elevatorComponent;
    private Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        var horizontalInput = Input.GetAxisRaw("Horizontal");
        var verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 velocity = ((transform.forward * verticalInput) + (transform.right * horizontalInput)) * movementSpeed;
        HandlePlatform(velocity);
    }

    private void HandlePlatform(Vector3 velocity)
    {
        if (elevatorComponent == null || !elevatorComponent.OnPlatform() || !elevatorComponent.MovingDown())
        {
            rb.velocity = velocity;
        }
        else if (elevatorComponent.OnPlatform() && elevatorComponent.MovingDown())
        {
            var newVel = new Vector3(velocity.x, elevatorComponent.Velocity().y, velocity.z);
            rb.velocity = newVel;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (1 << collision.gameObject.layer != platformLayer.value) return;

        elevatorComponent = collision.transform.GetComponent<MoveLift>();
    }

    private void OnCollisionExit(Collision collision)
    {
        if (1 << collision.gameObject.layer != platformLayer.value || elevatorComponent == null || elevatorComponent.OnPlatform()) return;

        elevatorComponent = null;
    }

}
