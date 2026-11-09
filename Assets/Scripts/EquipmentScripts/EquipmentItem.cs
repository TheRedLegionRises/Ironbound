using UnityEngine;
using System.Collections.Generic;

public enum EquipmentSlot
{
    MainHand,
    OffHand,
    Head,
    Body,
    Accessory,
}

public enum WeaponAttackType
{
    Melee,
    Ranged
}

public enum ArmorType
{
    None,
    Light,
    Medium,
    Heavy
}

[CreateAssetMenu(
    fileName = "New Equipment",
    menuName = "IRONBOUND/Equipment"
)]
public class EquipmentItem : ScriptableObject
{
    [Header("Basic Information")]
    public string itemName;
    [TextArea]
    public string description;

    public EquipmentSlot slot;

    public List<AttackOption> attackOptions = new List<AttackOption>(); 
    public List<WeaponAbility> abilities = new List<WeaponAbility>();
    //TODO delete after refactor
    // public string attackName = "Attack";
    // public WeaponAttackType attackType = WeaponAttackType.Melee;
    // public int attackRange = 1;
    // public int attackAPCost = 2;

    [Header("Statistics")]
    public int damageBonus = 0;
    public int armorBonus = 0;
    public int maxHealthBonus = 0;

     public ArmorType bodyArmorType = ArmorType.None;
}
