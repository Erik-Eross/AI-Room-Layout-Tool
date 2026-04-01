using UnityEngine;

public class FurniturePicker : MonoBehaviour
{
    [Header("Script Reference")]
    private PlacementManager placementManager;
    private ObjectSelectMenu objectSelectMenu;
    void Start()
    {
        //finds the script in the gameobject
        placementManager = GetComponent<PlacementManager>();
        objectSelectMenu = FindFirstObjectByType<ObjectSelectMenu>();
    }

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