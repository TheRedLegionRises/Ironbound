using UnityEngine;

public class BuildingInteraction : MonoBehaviour
{
    public string buildingName;
    public GameObject buildingMenu;

    public void OpenBuildingMenu()
    {
        if (buildingMenu != null)
            buildingMenu.SetActive(true);

        Debug.Log("Opened " + buildingName);
    }

    public void CloseBuildingMenu()
    {
        if (buildingMenu != null)
            buildingMenu.SetActive(false);
    }
}