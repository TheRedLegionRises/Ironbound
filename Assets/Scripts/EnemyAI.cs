
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EnemyType
{
    Melee,
    Ranged
}

public class EnemyAI : MonoBehaviour
{
    public EnemyType enemyType;

    public GridManager gridManager;
    public PlayerMovement playerMovement;
    public HealthSystem playerHealth;
    public EquipmentSystem equipmentSystem;
    public PlayerCombat playerCombat;

    //Base Stats
    public int maxHealth = 30;
    public int currentHealth = 30;
    public int attackDamage = 0;

    //Ranged 
    public int attackRange = 4;
    public int minimumRange = 2;

    //Grid
    public Tile currentTile;

    //Action Points
    public int maxAP = 5;
    public int currentAP = 5;

    public int attackAPCost = 2;

    [Header("AI Settings")]
    public float actionDelay = 0.3f;

    public bool isAlive = true;

    // Final health after equipment bonuses
    public int MaxHealth
    {
        get
        {
            int bonus = equipmentSystem != null
                ? equipmentSystem.GetMaxHealthBonus()
                : 0;

            return maxHealth + bonus;
        }
    }

    // Final damage after equipment bonuses
    public int GetTotalDamage
    {
        get
        {
            int bonus = equipmentSystem != null
                ? equipmentSystem.GetTotalDamageBonus()
                : 0;

            return attackDamage + bonus;
        }
    }

    void Start()
    {
        GameObject GridManager = GameObject.Find("GridManager");
        gridManager = GridManager.GetComponent<GridManager>();

        GameObject Player = GameObject.FindWithTag("Player"); 
        playerMovement = Player.GetComponent<PlayerMovement>();
        playerHealth = Player.GetComponent<HealthSystem>();
        playerCombat = Player.GetComponent<PlayerCombat>();

        equipmentSystem = GetComponent<EquipmentSystem>();

        currentHealth = MaxHealth;

    }

    
    public IEnumerator TakeTurn()
    {
        if (!isAlive || playerHealth == null ||
            playerHealth.IsDefeated)
            yield break;

        // Restore AP at the beginning of the enemy turn
        currentAP = maxAP;

        Debug.Log(name + " begins turn with " + currentAP + " AP.");

        while (currentAP > 0 && isAlive && !playerHealth.IsDefeated)
        {
            int previousAP = currentAP;

            if (enemyType == EnemyType.Melee)
            {
                TakeMeleeTurn();
            }
            else if (enemyType == EnemyType.Ranged)
            {
                TakeRangedTurn();
            }

            // Stop if the enemy couldn't perform an action
            if (currentAP == previousAP)
                break;

            Debug.Log(name + " has " + currentAP + " AP remaining.");

            yield return new WaitForSeconds(actionDelay);
        }

        Debug.Log(name + " ended turn.");
    }

    
    void TakeMeleeTurn()
    {
        Tile playerTile = playerMovement.currentTile;

        // Attack if adjacent
        if (IsAdjacent(currentTile, playerTile))
        {
            if (currentAP >= attackAPCost)
            {
                AttackPlayer();
                currentAP -= attackAPCost;
            }

            return;
        }

        // Otherwise, move toward player
        List<Tile> path = FindPathToPlayer(playerTile);

        if (path != null && path.Count > 0)
        {
            MoveToTile(path[0]);
        }
    }
    
    void TakeRangedTurn()
    {
        Tile playerTile = playerMovement.currentTile;

        if (playerTile == null || currentTile == null)
            return;

        int distance = GetDistance(currentTile, playerTile);

        // Retreat if player is too close
        if (distance < minimumRange)
        {
            RetreatFromPlayer(playerTile);
        }
        // Attack if player is within range
        else if (distance <= attackRange)
        {
            if (currentAP >= attackAPCost)
            {
                AttackPlayer();
                currentAP -= attackAPCost;
            }
        }
        // Approach if player is too far away
        else
        {
            List<Tile> path =
                FindPathToRange(playerTile, attackRange);

            if (path != null && path.Count > 0)
            {
                MoveToTile(path[0]);
            }
        }
    }

    void MoveToTile(Tile targetTile)
    {
        if (targetTile == null || currentTile == null)
            return;

        if (targetTile.isOccupied || !targetTile.isWalkable)
            return;

        int movementCost = targetTile.movementCost;

        // Check whether enemy has enough AP
        if (currentAP < movementCost)
            return;

        // Spend AP
        currentAP -= movementCost;

        // Update tile occupancy
        currentTile.isOccupied = false;

        currentTile = targetTile;
        currentTile.isOccupied = true;

        // Update enemy position
        transform.position =
            currentTile.transform.position + Vector3.up * 0.5f;

        Debug.Log(name + " moved for " + movementCost + " AP.");
    }

    void RetreatFromPlayer(Tile playerTile)
    {
        Tile bestTile = null;
        int bestDistance = GetDistance(currentTile, playerTile);

        foreach (Tile neighbor in gridManager.GetNeighbors(currentTile))
        {
            if (neighbor == null)
                continue;

            if (!neighbor.isWalkable || neighbor.isOccupied)
                continue;

            int distance = GetDistance(neighbor, playerTile);

            if (distance > bestDistance)
            {
                bestDistance = distance;
                bestTile = neighbor;
            }
        }

        if (bestTile != null)
        {
            MoveToTile(bestTile);
            Debug.Log(name + " retreated.");
        }
    }

    List<Tile> FindPathToPlayer(Tile playerTile)
    {
        return FindPathToRange(playerTile, 1);
    }

    List<Tile> FindPathToRange(Tile playerTile, int desiredRange)
    {
        if (currentTile == null || playerTile == null)
            return null;

        Queue<Tile> queue = new Queue<Tile>();

        Dictionary<Tile, Tile> cameFrom =
            new Dictionary<Tile, Tile>();

        queue.Enqueue(currentTile);
        cameFrom[currentTile] = null;

        Tile destination = null;

        while (queue.Count > 0)
        {
            Tile current = queue.Dequeue();

            if (current != currentTile &&
                GetDistance(current, playerTile) <= desiredRange)
            {
                destination = current;
                break;
            }

            foreach (Tile neighbor in gridManager.GetNeighbors(current))
            {
                if (neighbor == null)
                    continue;

                if (cameFrom.ContainsKey(neighbor))
                    continue;

                if (!neighbor.isWalkable || neighbor.isOccupied)
                    continue;

                if (neighbor.movementCost > currentAP)
                    continue;

                cameFrom[neighbor] = current;
                queue.Enqueue(neighbor);
            }
        }

        if (destination == null)
            return null;

        List<Tile> path = new List<Tile>();
        Tile step = destination;

        while (step != currentTile)
        {
            path.Add(step);
            step = cameFrom[step];
        }

        path.Reverse();

        return path;
    }

    void ApplyWeaponAbilities(ref int damage, ref float armorPenetration)
    {
        if (equipmentSystem == null)
            return;

        EquipmentItem mainWeapon = equipmentSystem.GetEquippedItem(EquipmentSlot.MainHand);
        EquipmentItem offHand = equipmentSystem.GetEquippedItem(EquipmentSlot.OffHand);

        if (mainWeapon != null)
        {
            foreach (WeaponAbility ability in mainWeapon.abilities)
            {
                if (ability != null)
                    ability.OnBeforeEnemyAttack(playerCombat, ref damage, ref armorPenetration);
            }
        }

        if (offHand != null)
        {
            foreach (WeaponAbility ability in offHand.abilities)
            {
                if (ability != null)
                    ability.OnBeforeEnemyAttack(playerCombat, ref damage, ref armorPenetration);
            }
        }
    }

    void AttackPlayer()
    {
        int damage = GetTotalDamage;
        float armorPenetration = 0f;

        ApplyWeaponAbilities(ref damage, ref armorPenetration);
        Debug.Log(name + " attacks for " + damage + " damage!");

        playerHealth.TakeDamage(damage, armorPenetration);
    }

    public void TakeDamage(int damage, float armorPenetration = 0f)
    {
        int totalArmor = equipmentSystem != null ? equipmentSystem.GetTotalArmor() : 0;

        int effectiveArmor = Mathf.RoundToInt(totalArmor * (1f - armorPenetration));

        int finalDamage = Mathf.Max(0, damage - effectiveArmor);

        currentHealth -= finalDamage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isAlive = false;

        if (currentTile != null)
        {
            currentTile.isOccupied = false;
        }

        Debug.Log(name + " was defeated!");

        Destroy(gameObject);
    }

    bool IsAdjacent(Tile a, Tile b)
    {
        return GetDistance(a, b) == 1;
    }

    int GetDistance(Tile a, Tile b)
    {
        if (a == null || b == null)
            return int.MaxValue;

        return Mathf.Abs(a.x - b.x) +
               Mathf.Abs(a.y - b.y);
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
}
