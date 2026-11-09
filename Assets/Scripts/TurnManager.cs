
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    public enum TurnState
    {
        PlayerTurn,
        EnemyTurn
    }

    [Header("References")]
    public PlayerMovement playerMovement;
    public HealthSystem playerHealth;
    public List<EnemyAI> enemies = new List<EnemyAI>();

    [Header("Turn Settings")]
    public float enemyTurnDelay = 0.3f;

    public TurnState CurrentTurn { get; private set; }

    public bool IsPlayerTurn =>
        CurrentTurn == TurnState.PlayerTurn;

    void Start()
    {
        StartCoroutine(InitializeTurn());
    }

    IEnumerator InitializeTurn()
    {
        yield return null;

        StartPlayerTurn();
    }
    public void EndPlayerTurn()
    {
        if (!IsPlayerTurn)
            return;

        StartCoroutine(EnemyTurnRoutine());
    }

    IEnumerator EnemyTurnRoutine()
    {
        CurrentTurn = TurnState.EnemyTurn;

        Debug.Log("Enemy Turn");

        foreach (EnemyAI enemy in enemies)
        {
            if (enemy == null || !enemy.isAlive)
                continue;

            if (playerHealth.IsDefeated)
                break;

            yield return StartCoroutine(enemy.TakeTurn());

            yield return new WaitForSeconds(enemyTurnDelay);
        }

        if (!playerHealth.IsDefeated)
        {
            StartPlayerTurn();
        }
    }

    void StartPlayerTurn()
    {
        CurrentTurn = TurnState.PlayerTurn;

        playerMovement.ResetAP();

        Debug.Log("Player Turn");
    }

    public void RegisterEnemy(EnemyAI enemy)
    {
        if (enemy != null && !enemies.Contains(enemy))
        {
            enemies.Add(enemy);

            Debug.Log("Registered enemy: " + enemy.name);
            Debug.Log("Total enemies: " + enemies.Count);
        }
    }
}
