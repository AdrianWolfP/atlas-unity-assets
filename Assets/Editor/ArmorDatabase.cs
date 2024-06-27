using System.IO;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

public class ArmorDatabase : ItemDatabase<Armor>
{
    private List<Armor> armors = new List<Armor>();
    private string newItemName = "";
    private bool isDuplicateName = false;
    private bool hasInvalidCharacter = false;

    private Regex nameValidationRegex = new Regex(@"^[a-zA-Z0-9 \-']*$");

    private string searchQuery = "";
    private ArmorType[] armorTypes;
    private int selectedArmorTypeIndex = 0;

    private List<Armor> filteredArmors = new List<Armor>();

    protected override void DrawItemList()
    {
        DrawSearchBar();
        DrawFilters();

        FilterArmors();

        foreach (Armor armor in filteredArmors)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Box(armor.icon ? armor.icon.texture : Texture2D.grayTexture, GUILayout.Width(50), GUILayout.Height(50));
            GUILayout.Label(armor.itemName, EditorStyles.boldLabel);
            if (GUILayout.Button("Select", GUILayout.Width(60)))
            {
                selectedItem = armor;
                newItemName = selectedItem.itemName;
                Repaint();
            }
            GUILayout.EndHorizontal();
        }
    }

    private void DrawSearchBar()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Search:", GUILayout.Width(50));
        searchQuery = GUILayout.TextField(searchQuery);
        GUILayout.EndHorizontal();
    }

    private void DrawFilters()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Type:", GUILayout.Width(50));
        selectedArmorTypeIndex = GUILayout.Popup(selectedArmorTypeIndex, GetArmorTypeNames());
        GUILayout.EndHorizontal();
    }

    private string[] GetArmorTypeNames()
    {
        if (armorTypes == null)
        {
            armorTypes = (ArmorType[])Enum.GetValues(typeof(ArmorType));
        }

        string[] names = new string[armorTypes.Length + 1];
        names[0] = "All";
        for (int i = 0; i < armorTypes.Length; i++)
        {
            names[i + 1] = armorTypes[i].ToString();
        }

        return names;
    }

    private void FilterArmors()
    {
        filteredArmors.Clear();

        foreach (Armor armor in armors)
        {
            if (armor.itemName.ToLower().Contains(searchQuery.ToLower()) && (selectedArmorTypeIndex == 0 || armor.armorType.HasFlag(armorTypes[selectedArmorTypeIndex - 1])))
            {
                filteredArmors.Add(armor);
            }
        }
    }

    protected override void DrawPropertiesSection()
    {
        if (selectedItem != null)
        {
            GUILayout.Label("Armor Properties", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Name: ");
            newItemName = GUILayout.TextField(newItemName);
            GUILayout.EndHorizontal();

            hasInvalidCharacter = !nameValidationRegex.IsMatch(newItemName);
            isDuplicateName = CheckForDuplicateName(newItemName, selectedItem);

            if (hasInvalidCharacter || isDuplicateName || string.IsNullOrWhiteSpace(newItemName))
            {
                GUI.color = Color.red;
            }
            else
            {
                GUI.color = Color.white;
            }

            if (hasInvalidCharacter)
            {
                EditorGUILayout.HelpBox("Item name contains invalid characters.", MessageType.Error);
            }
            else if (isDuplicateName)
            {
                EditorGUILayout.HelpBox("Item name is a duplicate.", MessageType.Error);
            }
            else if (string.IsNullOrWhiteSpace(newItemName))
            {
                EditorGUILayout.HelpBox("Item name cannot be empty.", MessageType.Error);
            }

            GUI.color = Color.white;

            if (GUILayout.Button("Apply Changes"))
            {
                if (string.IsNullOrWhiteSpace(newItemName) || hasInvalidCharacter || isDuplicateName)
                {
                    EditorUtility.DisplayDialog("Invalid Input", "Please fix the errors before applying changes.", "OK");
                }
                else
                {
                    EditorUtility.SetDirty(selectedItem);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }
        else
        {
            GUILayout.Label("Select a armor to see its properties.");
        }
    }

    private bool CheckForDuplicateName(string name, Armor currentArmor)
    {
        foreach (Armor armor in armors)
        {
            if (armor != currentArmor && armor.itemName == name)
            {
                return true;
            }
        }
        return false;
    }

    protected override void ExportItemsToCSV()
    {
        string csvContent = "Item Name,Armor Type,Icon\n";

        foreach (Armor armor in armors)
        {
            csvContent += $"{armor.itemName},{armor.armorType},{armor.icon?.texture.name}\n";
        }

        string filePath = EditorUtility.SaveFilePanel("Export Armor Database to CSV", "", "armor_database", "csv");
        if (!string.IsNullOrEmpty(filePath))
        {
            File.WriteAllText(filePath, csvContent);
            Debug.Log("Armor database exported to CSV successfully!");
        }
    }

    protected override void ImportItemsFromCSV()
    {
        string filePath = EditorUtility.OpenFilePanel("Import Armor Database from CSV", "", "csv");
        if (!string.IsNullOrEmpty(filePath))
        {
            string csvContent = File.ReadAllText(filePath);
            string[] rows = csvContent.Split('\n');

            foreach (string row in rows.Skip(1)) // Skip the header row
            {
                string[] columns = row.Split(',');
                if (columns.Length >= 3)
                {
                    Armor armor = new Armor();
                    armor.itemName = columns[0].Trim();
                    armor.armorType = (ArmorType)Enum.Parse(typeof(ArmorType), columns[1].Trim());
                    Texture2D texture = Resources.Load<Texture2D>($"Icons/{columns[2].Trim()}");
                    armor.icon = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                    armors.Add(armor);
                }
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Armor database imported from CSV successfully!");
        }
    }
}