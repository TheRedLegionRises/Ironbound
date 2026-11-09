
using UnityEngine;
using System;
using System.Collections.Generic;


public class HealthSystem : MonoBehaviour
{
    [Header("Health")]
    public int baseMaxHealth = 100;
    public int currentHealth;

    [Header("Equipment")]
    public EquipmentSystem equipmentSystem;

    public bool IsDefeated => currentHealth <= 0;

    public int MaxHealth
    {
        get
        {
            int equipmentBonus = equipmentSystem != null
                ? equipmentSystem.GetMaxHealthBonus()
                : 0;
        
            return Mathf.Max(
                1,
                baseMaxHealth + equipmentBonus
            );
        }
    }

    public event Action OnDefeated;
    public event Action OnHealthChanged;

    void Awake()
    {
        currentHealth = MaxHealth;
    }

    public int CalculatePlayerDamage(int baseDamage, EquipmentSystem equipment)
    {
        return baseDamage + equipment.GetTotalDamageBonus();
    }

    public void TakeDamage(int damage, float armorPenetration = 0f)
    {
        if (IsDefeated)
            return;

        int totalArmor = equipmentSystem != null ? equipmentSystem.GetTotalArmor() : 0;

        int effectiveArmor = Mathf.RoundToInt(totalArmor * (1f - armorPenetration));

        int finalDamage = Mathf.Max(0, damage - effectiveArmor);

        currentHealth = Mathf.Max(
            0,
            currentHealth - finalDamage
        );

        Debug.Log("Player took " + finalDamage + " damage.");

        OnHealthChanged?.Invoke();

        if (IsDefeated)
        {
            Debug.Log("Player defeated!");
            OnDefeated?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        if (IsDefeated)
            return;

        currentHealth = Mathf.Min(
            currentHealth + amount,
            MaxHealth
        );

        OnHealthChanged?.Invoke();
    }

    public int GetMovementCost(int baseCost)
    {
        return baseCost;
    }

    public int GetAttackDamage(int baseDamage)
    {
        return baseDamage;
    }

    public int GetMaximumAP(int baseAP)
    {
        return baseAP;
    }
}
