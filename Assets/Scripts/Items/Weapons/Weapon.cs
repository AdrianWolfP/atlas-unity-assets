using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Items/Weapon")]
public class Weapon : BaseItem
{
    [Header("Weapon Properties")]
    public WeaponType weaponType; // Type of weapon
    public float attackPower; // Attack power
    public float attackSpeed;  // Attack speed
    public float durability; // Durability of the weapon
    public float range; // Range of the weapon
    public float criticalHitChance; // Chance of a critical hit
}