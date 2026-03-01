using UnityEngine;

public class FurniturePicker : MonoBehaviour
{
    private PlacementManager placementManager;
    void Start()
    {
        placementManager = GetComponent<PlacementManager>();
    }
    public void DeselectFurniture()
    {
        placementManager.chosenPrefab = null;
    }
    public void PickFurniture(GameObject prefab)
    {
        placementManager.chosenPrefab = prefab;
    }
}