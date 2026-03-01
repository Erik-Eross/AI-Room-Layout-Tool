using Unity.VisualScripting;
using UnityEngine;

public class FurnitureInfo : MonoBehaviour
{
    public string furnitureId;
    public char symbol;
    public int w;
    public int h;
    public int xPos;
    public int yPos;
    public float rotation;
    public bool canInteract;
    public bool objectPlaced;
    private float timePlaced;
    private bool loopStopper;
    public GameObject furniturePrefab;

    void Update()
    {
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
