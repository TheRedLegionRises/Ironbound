using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement playerMovement;
    public EquipmentSystem equipmentSystem;

    public bool IsTargeting { get; private set; }
    private bool isCounterattackActive;
    private AttackOption selectedAttack;

    private EquipmentItem CurrentWeapon
    {
        get
        {
            return equipmentSystem.GetEquippedItem(
                EquipmentSlot.MainHand
            );
        }
    }

    // public void SelectAttack()
    // {
    //     if (CurrentWeapon == null)
    //     {
    //         Debug.Log("No weapon equipped!");
    //         return;
    //     }

    //     if (playerMovement.currentAP < CurrentWeapon.attackOptions[0].attackAPCost)
    //     {
    //         Debug.Log("Not enough AP!");
    //         return;
    //     }

    //     IsTargeting = true;

    //     Debug.Log("Select an enemy to use " +
    //               CurrentWeapon.attackOptions[0].attackName);
    // }

    public void SelectAttack(AttackOption attack)
    {
        if (attack == null)
            return;

        if (playerMovement.currentAP < attack.attackAPCost)
        {
            Debug.Log("Not enough AP!");
            return;
        }

        selectedAttack = attack;
        IsTargeting = true;
    }

    public void CancelTargeting()
    {
        IsTargeting = false;
    }

    public void TryHandleTargetClick(Ray ray)
    {
        if (!IsTargeting)
            return;

        if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

        // Find the tile that was clicked
        Tile clickedTile = hit.collider.GetComponent<Tile>();

        if (clickedTile == null)
            return;

        // Find an enemy occupying this tile
        EnemyAI enemy = FindEnemyOnTile(clickedTile);

        if (enemy == null)
        {
            Debug.Log("There is no enemy on this tile!");
            return;
        }

        EquipmentItem weapon = CurrentWeapon;

        if (weapon == null)
        {
            CancelTargeting();
            return;
        }

        // Check distance using grid coordinates
        int distance =
            Mathf.Abs(playerMovement.currentTile.x - clickedTile.x) +
            Mathf.Abs(playerMovement.currentTile.y - clickedTile.y);

        if (distance > weapon.attackOptions[0].attackRange)
        {
            Debug.Log("Enemy is out of range!");
            return;
        }

        // Check AP
        if (playerMovement.currentAP < weapon.attackOptions[0].attackAPCost)
        {
            Debug.Log("Not enough AP!");
            CancelTargeting();
            return;
        }

        PerformAttack(enemy, weapon);
    }

    private EnemyAI FindEnemyOnTile(Tile tile)
    {
        EnemyAI[] enemies = FindObjectsByType<EnemyAI>(
            FindObjectsSortMode.None
        );

        foreach (EnemyAI enemy in enemies)
        {
            if (enemy != null &&
                enemy.isAlive &&
                enemy.currentTile == tile)
            {
                return enemy;
            }
        }

        return null;
    }

    private void ApplyWeaponAbilitiesBeforeAttack(EnemyAI target, ref int damage, ref float armorPenetration)
    {
        EquipmentItem mainWeapon = equipmentSystem.GetEquippedItem(EquipmentSlot.MainHand);
        EquipmentItem offHand = equipmentSystem.GetEquippedItem(EquipmentSlot.OffHand);

        if (mainWeapon != null)
        {
            foreach (WeaponAbility ability in mainWeapon.abilities)
                ability.OnBeforePlayerAttack(target, ref damage, ref armorPenetration);
        }

        if (offHand != null)
        {
            foreach (WeaponAbility ability in offHand.abilities)
                ability.OnBeforePlayerAttack(target, ref damage, ref armorPenetration);
        }
    }

    // private void ApplyWeaponAbilitiesAfterAttack(EnemyAI target)
    // {
    //     EquipmentItem mainWeapon = equipmentSystem.GetEquippedItem(EquipmentSlot.MainHand);
    //     EquipmentItem offHand = equipmentSystem.GetEquippedItem(EquipmentSlot.OffHand);

    //     if (mainWeapon != null)
    //     {
    //         foreach (WeaponAbility ability in mainWeapon.abilities)
    //             ability.OnAfterAttack(this, target);
    //     }

    //     if (offHand != null)
    //     {
    //         foreach (WeaponAbility ability in offHand.abilities)
    //             ability.OnAfterAttack(this, target);
    //     }
    // }

    private void PerformAttack(EnemyAI enemy, EquipmentItem weapon)
    {
        int damage = equipmentSystem.GetTotalDamageBonus();

        float armorPenetration = 0f;

        ApplyWeaponAbilitiesBeforeAttack(enemy, ref damage, ref armorPenetration);

        enemy.TakeDamage(damage, armorPenetration);

        // ApplyWeaponAbilitiesAfterAttack(enemy);

        playerMovement.currentAP -= weapon.attackOptions[0].attackAPCost;
        playerMovement.UpdateMovementRange();

        Debug.Log("Used " + weapon.attackOptions[0].attackName + " for " + damage + " damage!");

        CancelTargeting();
    }

    public ArmorType GetArmorType()
        {   
            if (equipmentSystem == null)
                return ArmorType.None;

            EquipmentItem bodyArmor =
                equipmentSystem.GetEquippedItem(EquipmentSlot.Body);

            if (bodyArmor == null)
                return ArmorType.None;

            return bodyArmor.bodyArmorType;
        }

    //For counterattack and other player abilities
    public void HandleIncomingAttack(EnemyAI attacker, ref int damage)
    {
        EquipmentItem mainWeapon = equipmentSystem.GetEquippedItem(EquipmentSlot.MainHand);
        EquipmentItem offHand = equipmentSystem.GetEquippedItem(EquipmentSlot.OffHand);

        if (mainWeapon != null)
        {
            foreach (WeaponAbility ability in mainWeapon.abilities)
                ability.OnIncomingAttack(this, attacker, ref damage);
        }

        if (offHand != null)
        {
            foreach (WeaponAbility ability in offHand.abilities)
                ability.OnIncomingAttack(this, attacker, ref damage);
        }
    }
}