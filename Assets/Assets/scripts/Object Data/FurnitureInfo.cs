using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class FurnitureInfo : MonoBehaviour
{
    [Header("Setup For The Furniture")]
    public string furnitureId;
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

    //return the furniture size after applying current yaw rotation.
    public static Vector2Int GetRotatedSize(int width, int height, float yawDegrees)
    {
        int normalized = Mathf.RoundToInt(Mathf.Repeat(yawDegrees, 360f));
        int remainder = normalized % 180;
        if (remainder < 0) remainder += 180;
        if (remainder == 90)
        {
            return new Vector2Int(height, width);
        }
        return new Vector2Int(width, height);
    }

    //parse the rotation string and return a normalized degree value.
    public static int GetRotationDegrees(string rotation)
    {
        if (string.IsNullOrWhiteSpace(rotation)) return 0;
        string cleaned = rotation.Replace(", Rotation:", "").Replace("°", "").Trim();
        if (int.TryParse(cleaned, out int result))
        {
            return Mathf.RoundToInt(Mathf.Repeat(result, 360f));
        }
        return 0;
    }

    //compute this furniture instance's rotated size from its transform.
    public Vector2Int GetRotatedSize()
    {
        return GetRotatedSize(w, h, transform.eulerAngles.y);
    }

    void Update()
    {
        //makes the object interactable afer a short delay once its placed
        if (objectPlaced && !loopStopper)
        {
            timePlaced += Time.deltaTime;
            if (timePlaced > 0.5f)
            {
                loopStopper = true;
                canInteract = true;
            }
        }
    }
}
