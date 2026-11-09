using UnityEngine;
using UnityEngine.InputSystem;

public class TownBuildingManager : MonoBehaviour
{
    private BuildingInteraction selectedBuilding;

    void Update()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
            HandleBuildingClick();
    }

    void HandleBuildingClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

        BuildingInteraction building = hit.collider.GetComponentInParent<BuildingInteraction>();

        if (building == null)
            return;

        if (selectedBuilding != null)
            selectedBuilding.CloseBuildingMenu();

        selectedBuilding = building;

        selectedBuilding.OpenBuildingMenu();
    }
}