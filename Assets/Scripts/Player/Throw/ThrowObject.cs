using UnityEngine;
using UnityEngine.InputSystem;

public class ThrowObject : MonoBehaviour
{
    [Header("Keybinds")]
    [SerializeField] private InputActionReference throwKey;
    [Header("Coin")]
    [SerializeField] private GameObject objectToThrow;
    [Header("Arc Settings")]
    [SerializeField] private float timeStep;
    [SerializeField] private int throwStrength;
    [SerializeField] private float gravity;
    [Header("Other")]
    [SerializeField] private new Camera camera;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private int distance = 20;
    [SerializeField] private Material disableMaterial;
    [SerializeField] private Material[] previousMaterials;

    LineRenderer lineRenderer;
    private Timer timeUntillYouCanThrow;
    private bool holdingKey;
    private BoolTimer foundSpot;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        timeUntillYouCanThrow = new(1.0f);
        previousMaterials = new Material[objectToThrow.transform.childCount];

        for (int i = 0; i < objectToThrow.transform.childCount; i++)
        {
            var part = objectToThrow.transform.GetChild(i);
            previousMaterials[i] = part.GetComponent<MeshRenderer>().material;
        }
    }
    private void Update()
    {

        if (!holdingKey) return;

        UpdateCameraPosition();

    }

    private void UpdateCameraPosition()
    {
        RaycastHit ray;
        bool collided = Physics.Raycast(camera.transform.position, camera.transform.forward, out ray, distance);
        var collidedObjects = Physics.OverlapSphere(ray.point, objectToThrow.transform.localScale.y / 2);


        if (collidedObjects.Length > 1)
        {
            foundSpot.Reset();
            objectToThrow.transform.position = ray.point;
            ChangeMaterial(disableMaterial);
        }
        else if (!collided)
        {
            SetLinePositions(camera.transform.position + camera.transform.forward * Vector3.Distance(camera.transform.position, objectToThrow.transform.position));
            objectToThrow.transform.position = camera.transform.position + camera.transform.forward * distance;
            foundSpot.Reset();
            ChangeMaterial(disableMaterial);
        }
        else
        {
            Vector3 newPos = ray.point + ray.normal / 100;
            objectToThrow.transform.position = newPos;
            SetLinePositions(newPos);
            foundSpot.Set(1.0f);
            ChangeMaterial(previousMaterials);
        }

        if (ray.transform == null) return;

        CameraRotation(Quaternion.FromToRotation(ray.transform.up, ray.normal));
    }

    private void SetLinePositions(Vector3 position)
    {
        lineRenderer.SetPosition(0, camera.transform.position);
        lineRenderer.SetPosition(1, position);
    }

    private void CameraRotation(Quaternion rotation)
    {
        objectToThrow.transform.rotation = rotation;
    }

    public void Test(InputAction.CallbackContext obj)
    {
        if (obj.performed)
        {
            holdingKey = true;
            objectToThrow.SetActive(true);
        }
        else if (obj.canceled)
        {
            holdingKey = false;
            CheckForEnemiesInRadius();
        }
    }

    private void CheckForEnemiesInRadius()
    {
        if (!foundSpot)
        {
            objectToThrow.SetActive(false);
            return;
        }
        EnableGlowForEnemiesInRadius();
    }

    private void EnableGlowForEnemiesInRadius()
    {
        RaycastHit[] enemiesInRadius = Physics.SphereCastAll(objectToThrow.transform.position, 5, Vector3.down);
        Vector3 position = objectToThrow.transform.position;

        foreach (var item in enemiesInRadius)
        {
            if (!item.transform.TryGetComponent<Outline>(out var outlineComponent)) continue;

            float distance = Vector3.Distance(item.transform.position, position);
            Vector3 direction = (item.transform.position - position).normalized;
            bool collided = Physics.Raycast(position, direction, out RaycastHit raycast, distance);

            if (collided && raycast.transform.gameObject.layer != enemyLayer.value >> 5) continue;

            if (!item.transform.TryGetComponent<RevealEnemey>(out var revealEnemyComponent))
            {
                revealEnemyComponent = item.transform.gameObject.AddComponent<RevealEnemey>();
                revealEnemyComponent.Set(3, 0, 3);
            }
            else
            {
                revealEnemyComponent.ResetTimer();
            }
            outlineComponent.enabled = true;
        }
    }

    private void ChangeMaterial(Material[] material)
    {
        for (int i = 0; i < objectToThrow.transform.childCount; i++)
        {
            var part = objectToThrow.transform.GetChild(i);
            part.GetComponent<MeshRenderer>().material = material[i];
        }
    }

    private void ChangeMaterial(Material material)
    {
        for (int i = 0; i < objectToThrow.transform.childCount; i++)
        {
            var part = objectToThrow.transform.GetChild(i);
            part.GetComponent<MeshRenderer>().material = material;
        }
    }

}
