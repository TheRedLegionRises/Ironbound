using UnityEngine;

[CreateAssetMenu(
    fileName = "New Shred Ability",
    menuName = "IRONBOUND/Weapon Abilities/Shred"
)]
public class ShredAbility : WeaponAbility
{
    [Header("Shred Settings")]
    [Range(0f, 1f)]
    public float damageBonusPercent = 0.15f;

    //Attacking enemy
    public override void OnBeforePlayerAttack(EnemyAI target, ref int damage, ref float armorPenetration)
    {
        ApplyShred(target, ref damage);
    }

    //Attacking player
    public override void OnBeforeEnemyAttack(PlayerCombat combat, ref int damage, ref float armorPenetration)
    {
        ApplyShred(combat, ref damage);
    }


    //One for attacking enemy
    private void ApplyShred(EnemyAI target, ref int damage)
    {
        if (target == null)
            return;

        if (target.GetArmorType() != ArmorType.Light && target.GetArmorType() != ArmorType.None)
            return;

        damage = Mathf.RoundToInt(damage * (1f + damageBonusPercent));
    }

    //Attacking Player
    private void ApplyShred(PlayerCombat target, ref int damage)
    {
        if (target == null)
            return;

        if (target.GetArmorType() != ArmorType.Light && target.GetArmorType() != ArmorType.None)
            return;

        damage = Mathf.RoundToInt(damage * (1f + damageBonusPercent));
    }
}