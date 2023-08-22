using UnityEngine;

public class ExplosionForce : MonoBehaviour
{
    [SerializeField] private LayerMask brokenLayer;
    [SerializeField] private float explosionForce;
    [SerializeField] private float hitForce;

    private Vector3 velocity;
    Transform brokenObject;
    private void Start()
    {
        if (brokenObject == null) return;

        for (int i = 0; i < transform.childCount; i++)
        {
            var item = transform.GetChild(i);
            var hasComponent = item.gameObject.TryGetComponent(out Rigidbody rb);

            if (!hasComponent) continue;

            rb.velocity = velocity;
            rb.AddForce(Random.insideUnitSphere * explosionForce, ForceMode.Impulse);
        }
        brokenObject.rotation = Quaternion.identity;
    }
    public void SetImpactSpeed(Vector3 velocity, Transform brokenObject)
    {
        this.velocity = velocity;
        this.brokenObject = brokenObject;
    }

}
