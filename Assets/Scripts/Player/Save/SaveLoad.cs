using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System;
using Unity.Mathematics;

public static class SaveLoad
{
    private const int Version = 1;

    private static readonly string saveFilePath = Path.Combine(Application.persistentDataPath, "Save");
    private static readonly string fullFilePath = Path.Combine(saveFilePath, "save.json");
    private const string saveScriptFilePath = @"F:\My project (7)\Assets\Scripts\Player\Save\SaveDTO.cs";

    private static string lastModifedDate;
    public static string[] itemTypeEnumArr;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
        lastModifedDate = $"{File.GetLastWriteTimeUtc(saveScriptFilePath):yyyy-MM-dd} {File.GetLastAccessTime(saveScriptFilePath):hh:mm:ss}";
        itemTypeEnumArr = Enum.GetNames(typeof(ItemSaveType));
    }

    public static void Load(List<SaveData> itemsToSave, LoadableItems[] prefabItems, List<Transform> itemsToDelete)
    {
        if (File.Exists(fullFilePath))
        {
            try
            {
                LoadData(itemsToSave, prefabItems, itemsToDelete);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error loading save file: " + ex.Message);
            }
        }
        else
        {
            Debug.Log("Save file not found. Creating new save....");
            CheckDirectory();
            LoadData(itemsToSave, prefabItems, itemsToDelete);
        }
    }

    private static void LoadData(List<SaveData> list, LoadableItems[] prefabItems, List<Transform> itemsToDelete)
    {
        for (int i = 0; i < list.Count; i++)
        {
            var item = list[i].transform;
            UnityEngine.Object.Destroy(item.gameObject);
        }

        list.Clear();
        string json = File.ReadAllText(fullFilePath);
        var saveDTO = JsonConvert.DeserializeObject<SaveDTO>(json);

        for (int i = 0; i < saveDTO.savedItems.Count; i++)
        {
            var item = saveDTO.savedItems[i];

            var itemTypeIndex = (int)(item.itemType - 1);

            if (itemTypeIndex >= prefabItems.Length)
            {
                Debug.LogError($"Error: \"Item Type\" index ({itemTypeIndex}) is out of range. Max index: {itemTypeEnumArr.Length - 1}. \"Prefab Items\" length: {prefabItems.Length}");
            }

            var itemToInstantiate = prefabItems[itemTypeIndex].transform;

            var copiedItem = UnityEngine.Object.Instantiate(itemToInstantiate,
                item.position, item.rotation);
            list.Add(new SaveData(item.itemType, copiedItem));

            var savedItemsId = item.itemId;

            if (i >= itemsToDelete.Count) continue;

            Transform itemToDeleteTransform = itemsToDelete[i];

            if (savedItemsId != itemToDeleteTransform.GetInstanceID() || itemsToDelete == null) continue;

            UnityEngine.Object.Destroy(itemToDeleteTransform.gameObject);
        }
        itemsToDelete.Clear();
    }

    public static SaveDTO CreateDTO(List<SaveData> list)
    {
        SaveDTO saveDTO = new(Version, SavePlayerItems.maxNumberOfSavableItems, lastModifedDate);

        saveDTO.savedItems.Clear();
        if (list.Count < SavePlayerItems.maxNumberOfSavableItems)
        {
            for (int i = 0; i < list.Count; i++)
            {
                SaveData item = list[i];
                saveDTO.savedItems.Add(new DTO(item.itemType, item.transform.position,
                    item.transform.rotation, item.transform.GetInstanceID()));
            }
        }
        return saveDTO;
    }

    public static void Save(SaveDTO saveDTO)
    {
        try
        {
            CheckDirectory();
            string json = JsonConvert.SerializeObject(saveDTO, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
            });

            File.WriteAllText(fullFilePath, json);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error saving: " + ex.Message);
        }
    }

    private static void CheckDirectory()
    {
        Directory.CreateDirectory(saveFilePath);
    }
}
