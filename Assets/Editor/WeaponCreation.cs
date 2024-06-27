using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WeaponCreation : BaseItemCreation<Weapon>
{
    // Serialized Fields
    [SerializeField] private WeaponType weaponType; // Type of weapon
    [SerializeField] private float attackPower; // Attack power of weapon
    [SerializeField] private float attackSpeed; // Attack speed of weapon
    [SerializeField] private float durability; // Durability of the weapon
    [SerializeField] private float range; // Range of the weapon
    [SerializeField] private float criticalHitChance; // Chance of a critical hit

        // OnGUI Method
    private void OnGUI()
    {
        DrawCommonFields();

        DrawWeaponPropertySection();

        if (GUILayout.Button("Create Weapon"))
        {
            CreateWeapon();
        }
    }

    // Draw Weapon Property Section
    private void DrawWeaponPropertySection()
    {
        EditorGUILayout.LabelField("Weapon Properties", EditorStyles.boldLabel);

        weaponType = (WeaponType)EditorGUILayout.EnumPopup("Weapon Type", weaponType);
        attackPower = EditorGUILayout.FloatField("Attack Power", attackPower);
        attackSpeed = EditorGUILayout.FloatField("Attack Speed", attackSpeed);
        durability = EditorGUILayout.FloatField("Durability", durability);
        range = EditorGUILayout.FloatField("Range", range);
        criticalHitChance = EditorGUILayout.FloatField("Critical Hit Chance", criticalHitChance);
    }

    // Create Weapon
    private void CreateWeapon()
    {
        Weapon newWeapon = CreateInstance<Weapon>();

        // Assign weapon-specific values
        newWeapon.weaponType = weaponType;
        newWeapon.attackPower = attackPower;
        newWeapon.attackSpeed = attackSpeed;
        newWeapon.durability = durability;
        newWeapon.range = range;
        newWeapon.criticalHitChance = criticalHitChance;

        CreateItem(newWeapon);
    }
}
