using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
public class WeaponDatabase : ItemDatabase<Weapon>
{
    private List<Weapon> weapons = new List<Weapon>(); // Define the list of weapons
    private Weapon selectedWeapon; // Define the selected weapon
    protected override void DrawPropertiesSection()
    {
        if (selectedWeapon != null)
        {
            GUILayout.BeginVertical("Box");

            // Weapon properties
            GUILayout.Label("Weapon Properties", EditorStyles.boldLabel);

            DrawPropertyField("Name", nameof(selectedWeapon.name));
            DrawPropertyField("Damage", nameof(selectedWeapon.damage));
            DrawPropertyField("Range", nameof(selectedWeapon.range));
            DrawPropertyField("Fire Rate", nameof(selectedWeapon.fireRate));
            DrawPropertyField("Ammo Capacity", nameof(selectedWeapon.ammoCapacity));

            GUILayout.EndVertical();
        }
        else
        {
            GUILayout.Label("No weapon selected", EditorStyles.boldLabel);
        }
    }

    private void DrawPropertyField(string label, string propertyName)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label, GUILayout.Width(100));

        object currentValue = typeof(Weapon).GetProperty(propertyName).GetValue(selectedWeapon);

        if (currentValue is string)
        {
            string newStringValue = GUILayout.TextField((string)currentValue);
            if (newStringValue != (string)currentValue)
            {
                if (IsValidName(newStringValue))
                {
                    typeof(Weapon).GetProperty(propertyName).SetValue(selectedWeapon, newStringValue);
                }
                else
                {
                    EditorGUILayout.HelpBox("Invalid name. Please use only alphanumeric characters, spaces, dashes, and single quotes.", MessageType.Error);
                }
            }
        }
        else if (currentValue is int)
        {
            int newIntValue = EditorGUILayout.IntField((int)currentValue);
            if (newIntValue != (int)currentValue)
            {
                if (newIntValue > 0)
                {
                    typeof(Weapon).GetProperty(propertyName).SetValue(selectedWeapon, newIntValue);
                }
                else
                {
                    EditorGUILayout.HelpBox("Value must be a positive integer.", MessageType.Error);
                }
            }
        }
        else if (currentValue is float)
        {
            float newFloatValue = EditorGUILayout.FloatField((float)currentValue);
            if (newFloatValue != (float)currentValue)
            {
                if (newFloatValue > 0)
                {
                    typeof(Weapon).GetProperty(propertyName).SetValue(selectedWeapon, newFloatValue);
                }
                else
                {
                    EditorGUILayout.HelpBox("Value must be a positive float.", MessageType.Error);
                }
            }
        }

        GUILayout.EndHorizontal();
    }

    bool IsValidName(string name)
    {
        // Check for invalid characters
        foreach (char c in name)
        {
            if (!char.IsLetterOrDigit(c) && c != ' ' && c != '-' && c != '\'')
            {
                return false;
            }
        }

        // Check for duplicates
        foreach (Weapon weapon in weapons)
        {
            if (weapon.name == name && weapon != selectedWeapon)
            {
                return false;
            }
        }

        return true;
    }

    protected override void ImportItemsFromCSV()
    {
        string filePath = EditorUtility.OpenFilePanel("Import Weapon Database from CSV", "", "csv");
        if (!string.IsNullOrEmpty(filePath))
        {
            string csvContent = File.ReadAllText(filePath);
            string[] rows = csvContent.Split('\n');

            foreach (string row in rows.Skip(1)) // Skip the header row
            {
                string[] columns = row.Split(',');
                if (columns.Length >= 5)
                {
                    Weapon weapon = new Weapon();
                    weapon.name = columns[0].Trim();
                    weapon.damage = int.Parse(columns[1].Trim());
                    weapon.range = float.Parse(columns[2].Trim());
                    weapon.fireRate = float.Parse(columns[3].Trim());
                    weapon.ammoCapacity = int.Parse(columns[4].Trim());

                    weapons.Add(weapon);
                }
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Weapon database imported from CSV successfully!");
        }
    }

    protected override void DrawItemList()
    {
        // Implementation of DrawItemList method
        // This method should draw the list of weapons
        foreach (Weapon weapon in weapons)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(weapon.name, EditorStyles.boldLabel);
            // Add other properties or buttons as needed
            GUILayout.EndHorizontal();
        }
    }

    public class Weapon
    {
        public string name { get; set; }
        public int damage { get; set; }
        public float range { get; set; }
        public float fireRate { get; set; }
        public int ammoCapacity { get; set; } // Add this property
    }

    protected override void ExportItemsToCSV()
    {
        string filePath = EditorUtility.SaveFilePanel("Export Weapon Database to CSV", "", "weapons", "csv");
        if (!string.IsNullOrEmpty(filePath))
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Name,Damage,Range,Fire Rate,Ammo Capacity"); // Header row

                foreach (Weapon weapon in weapons)
                {
                    writer.WriteLine($"{weapon.name},{weapon.damage},{weapon.range},{weapon.fireRate},{weapon.ammoCapacity}");
                }
            }

            Debug.Log("Weapon database exported to CSV successfully!");
        }
    }
}