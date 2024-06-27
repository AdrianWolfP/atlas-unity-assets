using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

public class PotionDatabaseWindow : EditorWindow
{
    private Potion selectedItem;
    private string newPotionName = "";
    private bool isDuplicateName = false;
    private bool hasInvalidCharacter = false;

    private Regex nameValidationRegex = new Regex(@"^[a-zA-Z0-9 \-']*$");

    private Vector2 scrollPosition;
    
        private void OnInspectorUpdate()
        {
            Repaint();
        }
    [MenuItem("Window/Potion Database")]
    public static void ShowWindow()
    {
        GetWindow<PotionDatabaseWindow>("Potion Database");
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        DrawTopLeftOptions();
        DrawPotionList();
        DrawPropertiesSection();
        GUILayout.EndHorizontal();
    }

    private void DrawTopLeftOptions()
    {
        GUILayout.Label("Database Admin Functions:", EditorStyles.boldLabel);
        if (GUILayout.Button("Export to CSV")) ExportPotionsToCSV();
        if (GUILayout.Button("Import from CSV")) ImportPotionsFromCSV();
        if (GUILayout.Button("Delete Selected Potion")) DeleteSelectedPotion();
        if (GUILayout.Button("Duplicate Selected Potion")) DuplicateSelectedPotion();
        if (GUILayout.Button("Create New Potion"))
        {
            CreatePotionWindow window = (CreatePotionWindow)EditorWindow.GetWindow(typeof(CreatePotionWindow), false, "Create Potion");
        }
    }

    private void DrawPotionList()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        foreach (string guid in AssetDatabase.FindAssets("t:potion"))
        {
            Potion potion = AssetDatabase.LoadAssetAtPath<Potion>(AssetDatabase.GUIDToAssetPath(guid));
            if (potion!= null)
            {
                DrawPotionItem(potion);
            }
        }
        GUILayout.EndScrollView();
    }

    private void DrawPotionItem(Potion potion)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayoutOption widthOption = GUILayout.Width(50f);
        GUILayoutOption heightOption = GUILayout.Height(50f);
        GUILayout.Box(potion.icon? potion.icon.texture : Texture2D.grayTexture, widthOption, heightOption);
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField(potion.itemName, EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Base Value: " + potion.baseValue.ToString());
        EditorGUILayout.LabelField("Rarity: " + potion.rarity.ToString());
        EditorGUILayout.EndVertical();
        if (GUILayout.Button("Select", GUILayout.Width(60)))
        {
            selectedItem = potion;
            newPotionName = selectedItem.itemName;
            Repaint();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawPropertiesSection()
    {
        if (selectedItem!= null)
        {
            EditorGUILayout.LabelField("Potion Properties", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name: ");
            newPotionName = EditorGUILayout.TextField(newPotionName);
            EditorGUILayout.EndHorizontal();

            hasInvalidCharacter =!nameValidationRegex.IsMatch(newPotionName);
            isDuplicateName = CheckForDuplicateName(newPotionName, selectedItem);

            if (hasInvalidCharacter || isDuplicateName || string.IsNullOrWhiteSpace(newPotionName))
            {
                GUI.color = Color.red;
            }
            else
            {
                GUI.color = Color.white;
            }

            if (hasInvalidCharacter)
            {
                EditorGUILayout.HelpBox("Potion name contains invalid characters.", MessageType.Error);
            }
            else if (isDuplicateName)
            {
                EditorGUILayout.HelpBox("Potion name is a duplicate.", MessageType.Error);
            }
            else if (string.IsNullOrWhiteSpace(newPotionName))
            {
                EditorGUILayout.HelpBox("Potion name cannot be empty.", MessageType.Error);
            }

            GUI.color = Color.white;

            selectedItem.itemName = newPotionName;
            selectedItem.description = EditorGUILayout.TextField("Description: ", selectedItem.description);
            selectedItem.baseValue = EditorGUILayout.IntField("Base Value: ", selectedItem.baseValue);
            selectedItem.rarity = (Rarity)EditorGUILayout.EnumPopup("Rarity: ", selectedItem.rarity);

            if (GUILayout.Button("Save Changes"))
            {
                EditorUtility.SetDirty(selectedItem);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }

    protected void ExportPotionsToCSV()
    {
        // Export functionality implementation
    }

    protected void ImportPotionsFromCSV()
    {
        // Import functionality implementation
    }

    protected void DeleteSelectedPotion()
    {
        if (selectedItem!= null)
        {
            string itemName = selectedItem.itemName;
            bool delete = EditorUtility.DisplayDialog($"Delete {itemName}?", $"Are you sure you want to delete {itemName}?", "Yes", "No");
            if (delete)
            {
                string selectedItemPath = AssetDatabase.GetAssetPath(selectedItem);
                AssetDatabase.DeleteAsset(selectedItemPath);
                selectedItem = null;
                AssetDatabase.Refresh();
            }
        }
    }

    protected void DuplicateSelectedPotion()
    {
        if (selectedItem!= null)
        {
            string path = AssetDatabase.GetAssetPath(selectedItem);
            string newPath = AssetDatabase.GenerateUniqueAssetPath(path);
            AssetDatabase.CopyAsset(path, newPath);
            AssetDatabase.Refresh();
        }
    }

    private bool CheckForDuplicateName(string name, Potion currentPotion)
    {
        string[] guids = AssetDatabase.FindAssets("t:Potion");
        foreach (string guid in guids)
        {
            Potion potion = AssetDatabase.LoadAssetAtPath<Potion>(AssetDatabase.GUIDToAssetPath(guid));
            if (potion!= null && potion!= currentPotion && potion.itemName == name)
            {
                return true;
            }
        }
        return false;
    }
}