using UnityEngine;

public class FurniturePicker : MonoBehaviour
{
    [Header("Script Reference")]
    public PlacementManager placementManager;
    public ObjectSelectMenu objectSelectMenu;

    //UI references of buttons to pick the furniture
    public void DeselectFurniture()
    {
        placementManager.chosenPrefab = null;
        objectSelectMenu.menuType = "";
    }
    public void PickFurniture(GameObject prefab)
    {
        placementManager.chosenPrefab = prefab;
        objectSelectMenu.menuType = "";
    }
}