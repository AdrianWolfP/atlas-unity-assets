using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class ItemDatabase<T> : EditorWindow where T : class
{
    // Constants
    private const float MIN_WIDTH = 600f;
    private const float MIN_HEIGHT = 400f;
    private const float PROPERTIES_SECTION_WIDTH = 400f;

    // Fields
    protected Vector2 scrollPosition;
    protected T selectedItem;

    // Constructor
    public ItemDatabase()
    {
        minSize = new Vector2(MIN_WIDTH, MIN_HEIGHT);
    }

    // Abstract Methods
    protected abstract void DrawItemList();
    protected abstract void DrawPropertiesSection();
    protected abstract void ExportItemsToCSV();
    protected abstract void ImportItemsFromCSV();

    // Virtual Methods
    protected virtual void DeleteSelectedItem()
    {
        // Delete item
    }

    protected virtual void DuplicateSelectedItem()
    {
        // Duplicate item
    }

    protected virtual void DrawTopLeftOptions()
    {
        GUILayout.Label("Database Admin Functions:", EditorStyles.boldLabel);
        if (GUILayout.Button("Export to CSV")) ExportItemsToCSV();
        if (GUILayout.Button("Import from CSV")) ImportItemsFromCSV();
        if (GUILayout.Button("Delete Selected Item")) DeleteSelectedItem();
        if (GUILayout.Button("Duplicate Selected Item")) DuplicateSelectedItem();
        
        // ... additional top-left options, like 'Create New Item' or 'Search' ...
    }

    // OnGUI Method
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        
        DrawItemList();
        
        GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(PROPERTIES_SECTION_WIDTH));
        DrawPropertiesSection();
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }
}