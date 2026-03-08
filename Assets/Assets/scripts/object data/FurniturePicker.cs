using UnityEngine;

public class FurniturePicker : MonoBehaviour
{
    [Header("Script Reference")]
    private PlacementManager placementManager;
    void Start()
    {
        //finds the script in the gameobject
        placementManager = GetComponent<PlacementManager>();
    }

    //UI references of buttons to pick the furniture
    public void DeselectFurniture()
    {
        placementManager.chosenPrefab = null;
    }
    public void PickFurniture(GameObject prefab)
    {
        placementManager.chosenPrefab = prefab;
    }
}