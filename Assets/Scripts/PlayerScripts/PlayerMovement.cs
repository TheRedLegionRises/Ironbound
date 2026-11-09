
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    public GridManager gridManager;
    public TurnManager turnManager;
    public PlayerCombat playerCombat;
    public MissionObjectiveManager objectiveManager;

    [Header("Starting Position")]
    public int startX = 0;
    public int startY = 0;

    [Header("Action Points")]
    public int maxAP = 5;
    public int currentAP = 5;

    public float moveSpeed = 5f;
    public float doubleClickTime = 0.3f;
    public float verticalOffset = 0.6f;

    public Tile currentTile;

    private Tile lastClickedTile;
    private float lastClickTime;
    private Tile hoveredTile;

    private bool isMoving = false;

    void Start()
    {
        currentAP = maxAP;

        currentTile = gridManager.GetTile(startX, startY);

        if (currentTile != null)
        {
            currentTile.isOccupied = true;

            transform.position = currentTile.transform.position + Vector3.up * verticalOffset;
        }

        UpdateMovementRange();
    }

    void Update()
    {
        if (turnManager == null || !turnManager.IsPlayerTurn)
            return;

        UpdateHoveredTile();

        if (isMoving)
            return;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            HandleMouseClick();
    }

    void UpdateHoveredTile()
    {
        if (Camera.main == null || Mouse.current == null)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        Tile newHoveredTile = null;

        if (Physics.Raycast(ray, out RaycastHit hit))
            newHoveredTile = hit.collider.GetComponent<Tile>();

        if (newHoveredTile == hoveredTile)
            return;

        if (hoveredTile != null)
            hoveredTile.SetHoverHighlight(false);

        hoveredTile = newHoveredTile;

        if (hoveredTile != null)
            hoveredTile.SetHoverHighlight(true);
    }

    void HandleMouseClick()
    {
        if (Camera.main == null)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        // If attacking, send the click to PlayerCombat instead.
        if (playerCombat != null && playerCombat.IsTargeting)
        {
            playerCombat.TryHandleTargetClick(ray);
            return;
        }

        if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

        Tile clickedTile = hit.collider.GetComponent<Tile>();

        if (clickedTile == null)
            return;

        float timeSinceLastClick = Time.time - lastClickTime;

        if (clickedTile == lastClickedTile && timeSinceLastClick <= doubleClickTime)
        {
            MoveToTile(clickedTile);

            lastClickedTile = null;
            lastClickTime = 0f;
        }
        else
        {
            lastClickedTile = clickedTile;
            lastClickTime = Time.time;
        }
    }

    void MoveToTile(Tile targetTile)
    {
        if (targetTile == currentTile)
            return;

        if (!targetTile.isWalkable || targetTile.isOccupied)
            return;

        List<Tile> path = FindPath(currentTile, targetTile);

        if (path == null || path.Count == 0)
            return;

        int totalCost = 0;

        foreach (Tile tile in path)
            totalCost += tile.movementCost;

        if (totalCost > currentAP)
        {
            Debug.Log("Not enough AP!");
            return;
        }

        currentAP -= totalCost;

        StartCoroutine(FollowPath(path));
    }

    List<Tile> FindPath(Tile start, Tile target)
    {
        List<Tile> openSet = new List<Tile>();

        Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();
        Dictionary<Tile, int> gScore = new Dictionary<Tile, int>();
        Dictionary<Tile, int> fScore = new Dictionary<Tile, int>();

        openSet.Add(start);

        gScore[start] = 0;
        fScore[start] = GetHeuristic(start, target);

        while (openSet.Count > 0)
        {
            Tile current = openSet[0];

            foreach (Tile tile in openSet)
            {
                if (fScore[tile] < fScore[current])
                    current = tile;
            }

            if (current == target)
                return ReconstructPath(cameFrom, current);

            openSet.Remove(current);

            foreach (Tile neighbor in gridManager.GetNeighbors(current))
            {
                if (!neighbor.isWalkable)
                    continue;

                if (neighbor.isOccupied && neighbor != target)
                    continue;

                int tentativeGScore = gScore[current] + neighbor.movementCost;

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;

                    gScore[neighbor] = tentativeGScore;

                    fScore[neighbor] = tentativeGScore + GetHeuristic(neighbor, target);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null;
    }

    int GetHeuristic(Tile a, Tile b)
    {
        return gridManager.GetHexDistance(a, b);
    }

    List<Tile> ReconstructPath(Dictionary<Tile, Tile> cameFrom, Tile current)
    {
        List<Tile> path = new List<Tile>();

        while (cameFrom.ContainsKey(current))
        {
            path.Add(current);
            current = cameFrom[current];
        }

        path.Reverse();

        return path;
    }

    IEnumerator FollowPath(List<Tile> path)
    {
        isMoving = true;

        foreach (Tile tile in path)
        {
            currentTile.isOccupied = false;
            tile.isOccupied = true;

            Vector3 targetPosition = tile.transform.position + Vector3.up * verticalOffset;

            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPosition;
            currentTile = tile;

            if (objectiveManager != null)
                objectiveManager.CheckObjective(currentTile);
        }

        isMoving = false;

        UpdateMovementRange();

        Debug.Log("Remaining AP: " + currentAP);
    }

    void UpdateMovementRange()
    {
        // Clear previous highlights.
        for (int x = 0; x < gridManager.width; x++)
        {
            for (int y = 0; y < gridManager.height; y++)
            {
                Tile tile = gridManager.GetTile(x, y);

                if (tile != null)
                    tile.SetMovementHighlight(false);
            }
        }

        List<Tile> openSet = new List<Tile>();

        Dictionary<Tile, int> costs = new Dictionary<Tile, int>();

        openSet.Add(currentTile);
        costs[currentTile] = 0;

        while (openSet.Count > 0)
        {
            Tile current = openSet[0];
            openSet.RemoveAt(0);

            foreach (Tile neighbor in gridManager.GetNeighbors(current))
            {
                if (!neighbor.isWalkable)
                    continue;

                if (neighbor.isOccupied && neighbor != currentTile)
                    continue;

                int newCost = costs[current] + neighbor.movementCost;

                if (newCost > currentAP)
                    continue;

                if (!costs.ContainsKey(neighbor) || newCost < costs[neighbor])
                {
                    costs[neighbor] = newCost;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        foreach (var entry in costs)
        {
            if (entry.Key != currentTile)
                entry.Key.SetMovementHighlight(true);
        }
    }

    public void ResetAP()
    {
        currentAP = maxAP;
        UpdateMovementRange();
    }
}
