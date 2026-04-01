using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class FurnitureInfo : MonoBehaviour
{
    [Header("Setup For The Furniture")]
    public string furnitureId;
    public string symbol;
    public int w;
    public int h;
    public int xPos;
    public int yPos;
    public string rotation;
    public bool canInteract;
    public bool objectPlaced;
    private float timePlaced;
    private bool loopStopper;

    [Header("Furniture Prefab")]
    public GameObject furniturePrefab;

    //this is called when the object has been placed
    public void AssignInstanceName()
    {
        GameObject[] allFurniture = GameObject.FindGameObjectsWithTag("Furniture");
        Dictionary<string, int> furnitureCount = new Dictionary<string, int> { {"Chairs", 0}, {"Tables", 0} };

        //counts all the furnitures and gives it the appropriate symbol
        foreach(GameObject furniture in allFurniture)
        {
            var details = furniture.GetComponent<FurnitureInfo>();
            if(details.objectPlaced)
            {
                if(details.symbol == "Chair" || details.symbol.StartsWith("Chair"))
                {
                    furnitureCount["Chairs"] ++;
                }
                else if(details.symbol == "Table" || details.symbol.StartsWith("Table"))
                {
                    furnitureCount["Tables"] ++;
                }
            }
        }

        //assigns the furniture's symbol
        switch(symbol)
        {
            case "Chair":
                symbol = "Chair" + furnitureCount["Chairs"].ToString();
                break;
            case "Table":
                symbol = "Table" + furnitureCount["Tables"].ToString();
                break;
        }
    }

    void Update()
    {
        //makes the object interactable afer a short delay once its placed
        if(objectPlaced && !loopStopper)
        {
            timePlaced += Time.deltaTime;
            if(timePlaced > 0.5f)
            {
                loopStopper = true;
                canInteract = true;
            }
        }
    }
}
