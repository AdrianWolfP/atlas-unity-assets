using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class BaseItemCreation<T> : EditorWindow where T : BaseItem
{
    // ...

    private Dictionary<Type, string> folderPaths = new Dictionary<Type, string>()
    {
        { typeof(Weapon), "Weapons/" },
        { typeof(Armor), "Armor/" },
        { typeof(Potion), "Potions/" },
        // Add more item types as needed
    };

    protected void CreateItem(T newItem)
    {
        // Assign entered values to the item fields
        newItem.itemName = itemName;
        newItem.icon = icon;
        newItem.description = description;
        newItem.baseValue = baseValue;
        newItem.requiredLevel = requiredLevel;
        newItem.rarity = rarity;
        newItem.equipSlot = equipSlot;

        // Determine the folder path based on the type of item
        string folderPath = "Assets/Items/";
        if (folderPaths.TryGetValue(typeof(T), out string subFolderPath))
        {
            folderPath += subFolderPath;
        }
        else
        {
            Debug.LogError($"Unknown item type: {typeof(T)}");
            return;
        }

        // Create the directory if it doesn't exist
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            System.IO.Directory.CreateDirectory(Application.dataPath + folderPath.Substring("Assets".Length));
            AssetDatabase.Refresh();
        }

        EditorUtility.SetDirty(newItem);

        // Define the full path for the asset
        string fullPath = folderPath + itemName + ".asset";
        fullPath = AssetDatabase.GenerateUniqueAssetPath(fullPath);

        AssetDatabase.CreateAsset(newItem, fullPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}