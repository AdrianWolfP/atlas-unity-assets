using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/BaseItem")]
public abstract class BaseItem : ScriptableObject
{
    [Header("General Properties")]
    public string itemName;
    public Sprite icon;
    public string description;
    public float baseValue;
    public Rarity rarity;
    public int requiredLevel;
    public EquipSlot equipSlot;
}