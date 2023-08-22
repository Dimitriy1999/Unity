using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
public class SavePlayerItems : MonoBehaviour
{
    [Header("Saveable Items")]
    [ArrayRename(typeof(ItemSaveType))]
    [SerializeField] private LoadableItems[] prefabItems = new LoadableItems[Enum.GetNames(typeof(ItemSaveType)).Length - 1];
    [Space(5)]
    [Header("Line Visual Start Point")]
    [SerializeField] private Transform startLinePoint;
    [Space(10)]
    [SerializeField] private float lineDuration;

    private List<SaveData> savedItems;
    private LineRenderer lineRenderer;
    private BoolTimer isVisibleTimer;
    private BoolTimer cyclingThroughItemsTimer;
    private BoolTimer itemRecentlyEntered;
    private BoolTimer itemsRecentlyLoaded;

    private List<Transform> itemsToDelete;

    private int currentIndex = 0;

    private const int MAX_NUMBER_LINES = 2;
    public static int maxNumberOfSavableItems = 25;

    private volatile int version;
    private object saveLock = new();


    private void Awake()
    {
        itemsToDelete = new(maxNumberOfSavableItems);
        savedItems = new List<SaveData>(maxNumberOfSavableItems);
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.positionCount = MAX_NUMBER_LINES;
        lineRenderer.SetPosition(0, startLinePoint.position);
        isVisibleTimer = BoolTimer.Create();
        cyclingThroughItemsTimer = BoolTimer.Create();
        itemRecentlyEntered = BoolTimer.Create();
        itemsRecentlyLoaded = BoolTimer.Create();
    }

    private void Update()
    {
        if (!lineRenderer.enabled) return;

        if (!isVisibleTimer && !cyclingThroughItemsTimer)
        {
            lineRenderer.enabled = false;
            return;
        }

        if (!cyclingThroughItemsTimer.Value || currentIndex >= savedItems.Count || isVisibleTimer.Value) return;

        lineRenderer.SetPosition(1, savedItems[currentIndex].transform.position);
        isVisibleTimer.Set(lineDuration);
        currentIndex++;
    }

    private void OnTriggerEnter(Collider other)
    {
        var itemType = other.GetComponent<ItemType>();
        if (itemType == null || savedItems.Count >= maxNumberOfSavableItems || itemsRecentlyLoaded) return;

        savedItems.Add(new SaveData(itemType.EnumType(), other.transform));
        lineRenderer.enabled = true;
        isVisibleTimer.Set(lineDuration);
        lineRenderer.SetPosition(1, other.transform.position);
        itemsToDelete.Remove(other.transform);

        if (itemRecentlyEntered)
        {
            cyclingThroughItemsTimer.Set(savedItems.Count * lineDuration);
        }
        else
        {
            cyclingThroughItemsTimer.Set(lineDuration);
            currentIndex = savedItems.Count - 1;
        }

        itemRecentlyEntered.Set(0.05f);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<ItemType>(out var itemType)) return;

        savedItems.Remove(new SaveData(itemType.EnumType(), other.transform));
        itemsToDelete.Add(other.transform);
    }

    private void SavePlayerData(object param)
    {
        try
        {
            SaveDTO saveDTO = (SaveDTO)param;
            if (saveDTO.savedItems.Count <= 0) return;

            lock (saveLock)
            {
                if (saveDTO.version < version) return;

                SaveLoad.Save(saveDTO);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }

    public void Save()
    {
        ThreadData threadData = new();
        version++;
        SaveDTO saveDTO = SaveLoad.CreateDTO(savedItems);
        threadData.saveDTO = saveDTO;
        threadData.version = version;

        Task task = new(SavePlayerData, saveDTO);
        task.Start();
        SetupLineRenderer();
    }

    public void LoadPlayerData()
    {
        SaveLoad.Load(savedItems, prefabItems, itemsToDelete);
        itemsRecentlyLoaded.Set(0.1f);
        SetupLineRenderer();
    }

    private void SetupLineRenderer()
    {
        if (savedItems.Count <= 0) return;

        lineRenderer.enabled = true;
        cyclingThroughItemsTimer.Set(savedItems.Count * lineDuration);
        isVisibleTimer.Set(lineDuration);
        lineRenderer.SetPosition(1, savedItems[0].transform.position);
        currentIndex = 1;
    }
}
