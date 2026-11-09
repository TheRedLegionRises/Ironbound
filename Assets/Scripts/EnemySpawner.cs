
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager;
    public TurnManager turnManager;
    public GameObject enemyPrefab;

    [Header("Spawn Position")]
    public int spawnX = 3;
    public int spawnY = 3;

    void Start()
    {
        SpawnEnemy();
    }

    public void SpawnEnemy()
{
    if (gridManager == null || enemyPrefab == null || turnManager == null)
    {
        Debug.LogError("EnemySpawner references are missing!");
        return;
    }

    Tile spawnTile = gridManager.GetTile(spawnX, spawnY);

    if (spawnTile == null || !spawnTile.isWalkable || spawnTile.isOccupied)
    {
        Debug.LogWarning("Spawn tile is unavailable!");
        return;
    }

    Vector3 spawnPosition =
        spawnTile.transform.position + Vector3.up * 0.5f;

    GameObject enemy = Instantiate(
        enemyPrefab,
        spawnPosition,
        Quaternion.identity
    );

    EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();

    if (enemyAI == null)
    {
        Debug.LogError("Enemy prefab is missing EnemyAI!");
        Destroy(enemy);
        return;
    }

    enemyAI.gridManager = gridManager;
    enemyAI.currentTile = spawnTile;

    spawnTile.isOccupied = true;

    // Register enemy with TurnManager
    turnManager.RegisterEnemy(enemyAI);

    Debug.Log("Enemy spawned and registered with TurnManager!");
}
}
