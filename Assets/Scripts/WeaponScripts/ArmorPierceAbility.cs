using UnityEngine;

[CreateAssetMenu(
    fileName = "New Armor Pierce Ability",
    menuName = "IRONBOUND/Weapon Abilities/Armor Pierce"
)]
public class ArmorPierceAbility : WeaponAbility
{
    [Header("Armor Pierce Settings")]
    [Range(0f, 1f)]
    public float armorPenetrationPercent = 0.25f;
    
    //Attacking enemy
    public override void OnBeforePlayerAttack(EnemyAI target, ref int damage, ref float armorPenetration)
    {
        armorPenetration = armorPenetrationPercent;
    }

    //Attacking player
    public override void OnBeforeEnemyAttack(PlayerCombat combat, ref int damage, ref float armorPenetration)
    {
        armorPenetration = armorPenetrationPercent;
    }
}