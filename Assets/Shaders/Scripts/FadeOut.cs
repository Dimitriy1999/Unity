using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    [SerializeField] private float fadeOutTime;
    [SerializeField] private Timer time;
    [SerializeField] private Timer fadeStart;
    [SerializeField] private float timeUntilWeFade;
    [SerializeField] private Material materialRef;

    private bool removeCollision;
    private Material materialInstance;
    private Renderer thisObjectRenderer;
    private void Awake()
    {
        Physics.IgnoreLayerCollision(10, 10, true);
        thisObjectRenderer = GetComponent<MeshRenderer>();
        materialInstance = new(materialRef);
        Transform player = World.Instance.Player.transform.GetChild(0);

        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                var renderer = child.GetComponent<Renderer>();
                Physics.IgnoreCollision(child.GetComponent<MeshCollider>(), player.GetComponent<Collider>());
                renderer.material = materialInstance;
            }
        }

        if (thisObjectRenderer != null)
        {
            thisObjectRenderer.material = materialInstance;
        }

        time = new();
        materialInstance.SetFloat("_Current_Time", 1);
        UpdateLayerToFading();
    }

    void Update()
    {
        if (time.Time < timeUntilWeFade || materialInstance.GetFloat("_Current_Time") <= 0) return;

        if (fadeStart == null)
        {
            fadeStart = new();
        }

        float currentTime = (fadeOutTime - fadeStart.Time) / fadeOutTime;
        float value = Mathf.Clamp(currentTime, 0, fadeOutTime);
        materialInstance.SetFloat("_Current_Time", value);
        Destroy(gameObject, fadeOutTime);
    }
    private void UpdateLayerToFading()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);

            child.gameObject.layer = Layer.ObjectFadeOutLayer;
        }
    }
}
