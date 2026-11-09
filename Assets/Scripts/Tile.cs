using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x;
    public int y;

    public bool isWalkable = true;
    public bool isOccupied = false;
    public bool isObjective = false;

    public int movementCost = 1;

    [Header("Highlight Colors")]
    public Color normalColor = Color.white;
    public Color movementColor = Color.green;
    public Color hoverColor = Color.yellow;
    public Color objectiveColor = Color.purple;

    private Renderer tileRenderer;

    private bool isInMovementRange = false;
    private bool isHovered = false;

    void Awake()
    {
        tileRenderer = GetComponent<Renderer>();

        UpdateColor();
    }

    public void Initialize(int gridX, int gridY)
    {
        x = gridX;
        y = gridY;

        gameObject.name = $"Tile ({x}, {y})";
    }

    public void SetMovementHighlight(bool highlighted)
    {
        isInMovementRange = highlighted;
        UpdateColor();
    }

    public void SetHoverHighlight(bool highlighted)
    {
        isHovered = highlighted;
        UpdateColor();
    }

    void UpdateColor()
    {
        if (tileRenderer == null)
            return;

        if (isObjective)
        {
            tileRenderer.material.color = objectiveColor;
        } else if (isHovered)
        {
            tileRenderer.material.color = hoverColor;
        }
        else if (isInMovementRange)
        {
            tileRenderer.material.color = movementColor;
        }
        else
        {
            tileRenderer.material.color = normalColor;
        }
    }
}
