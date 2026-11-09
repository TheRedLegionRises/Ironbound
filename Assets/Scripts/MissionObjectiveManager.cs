using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionObjectiveManager : MonoBehaviour
{
    [Header("Objective")]
    public Tile objectiveTile;

    [Header("Mission Complete UI")]
    public GameObject missionCompletePanel;

    [Header("Scene Settings")]
    public string townSceneName = "Town";

    public int objectiveCoordinateX = 4;
    public int objectiveCoordinateY = 4;

    private bool missionCompleted = false;

    void Start()
    {
        if (missionCompletePanel != null)
            missionCompletePanel.SetActive(false);
    }

    public void InitializeObjective(GridManager gridManager)
    {
        objectiveTile = gridManager.GetTile(objectiveCoordinateX, objectiveCoordinateY);

        if (objectiveTile == null)
        {
            Debug.LogError("Could not find objective tile!");
            return;
        }

        objectiveTile.isObjective = true;

        Debug.Log("Objective assigned to tile: " + objectiveCoordinateX + ", " + objectiveCoordinateY);
    }

    public void CheckObjective(Tile playerTile)
    {
        if (missionCompleted || playerTile == null)
            return;

        if (playerTile == objectiveTile)
            CompleteMission();
    }

    private void CompleteMission()
    {
        missionCompleted = true;

        Debug.Log("Mission Complete!");

        if (missionCompletePanel != null)
            missionCompletePanel.SetActive(true);
    }

    public void ReturnToTown()
    {
        SceneManager.LoadScene(townSceneName);
    }
}