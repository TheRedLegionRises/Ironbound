using UnityEngine;

public abstract class WeaponAbility : ScriptableObject
{
    public string abilityName;

    public string description;

    // Player attacking an enemy
    public virtual void OnBeforePlayerAttack(EnemyAI target, ref int damage, ref float armorPenetration)
    {
    }

    // Enemy attacking the player
    public virtual void OnBeforeEnemyAttack(PlayerCombat combat, ref int damage, ref float armorPenetration)
    {
    }

    public virtual void OnAfterAttack(PlayerCombat combat, EnemyAI target)
    {
    }

    //For player ability such as counterattack
    public virtual void OnIncomingAttack(PlayerCombat combat, EnemyAI attacker, ref int damage)
    {
    }
}