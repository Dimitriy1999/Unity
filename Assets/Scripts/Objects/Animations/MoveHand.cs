using UnityEngine;

public class MoveHand : MonoBehaviour
{
    [SerializeField] private float resetDelay;
    [SerializeField] private float lerpAmount;
    private BoolTimer handGoingDown;
    private Vector3 newPosition;
    private Vector3 originalPos;
    private Timer moveDelay;

    private void Awake()
    {
        handGoingDown = new BoolTimer();
        newPosition = new Vector3(transform.position.x, transform.position.y - 0.25f, transform.position.z);
        originalPos = transform.position;
        handGoingDown.Set(1.0f);
        moveDelay = new Timer();
    }

    private void Update()
    {
        if (handGoingDown)
        {
            transform.position = Vector3.Lerp(transform.position, newPosition, lerpAmount * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, originalPos, lerpAmount * Time.deltaTime);
        }

        if (moveDelay.Time >= resetDelay)
        {
            moveDelay.Reset();
            handGoingDown.Set(1.0f);
        }
    }
}