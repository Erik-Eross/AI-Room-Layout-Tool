using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class LayoutSaveSystem : MonoBehaviour
{
    [Header("Script Reference")]
    public GridManager gridManager;
    public OrbitCamera orbitCameraScript;

    [Header("Prefab Reference")]
    public GameObject chairPrefab;
    public GameObject tablePrefab;

    public void SaveLayout(int saveSlot)
    {
        //find the room layout and all furnitures before saving
        LayoutData layout = new LayoutData();

        layout.roomWidth = gridManager.generateWidth;
        layout.roomHeight = gridManager.generateHeight;
        layout.timeStamp = DateTime.Now.ToString("dd MMM HH:mm");

        GameObject[] allFurniture = GameObject.FindGameObjectsWithTag("Furniture");

        //find the details of each furniture and check it exists
        foreach (GameObject obj in allFurniture)
        {
            FurnitureInfo info = obj.GetComponent<FurnitureInfo>();

            if (info == null || !info.objectPlaced)
                continue;

            FurnitureData data = new FurnitureData();

            //create a new furniture data to store as a JSON
            data.type = info.furnitureId;
            data.gridX = info.xPos;
            data.gridY = info.yPos;
            data.width = info.w;
            data.height = info.h;
            data.rotation = info.rotation;

            //add the data to the LayoutData
            layout.furniture.Add(data);
        }

        //save the json when all furnitures have been logged
        string json = JsonUtility.ToJson(layout, true);
        string path = Application.persistentDataPath + "/layout" + saveSlot + ".json";

        //write to files and debug to check
        File.WriteAllText(path, json);
        Debug.Log("Layout saved to: " + path);
    }

    public void LoadLayout(int saveSlot)
    {
        //get the path of the JSON file
        string path = Application.persistentDataPath + "/layout" + saveSlot + ".json";

        if (!File.Exists(path))
        {
            Debug.LogWarning("No save file found at: " + path);
            return;
        }

        //read the JSON and find the layout data
        string json = File.ReadAllText(path);
        LayoutData layout = JsonUtility.FromJson<LayoutData>(json);

        gridManager.width = layout.roomWidth;
        gridManager.height = layout.roomHeight;
        gridManager.generateWidth = layout.roomWidth;
        gridManager.generateHeight = layout.roomHeight;

        //regenerate the grid and reset the camera position
        gridManager.GenerateGrid();
        orbitCameraScript.ResetCamOrbit();

        GameObject[] existingFurniture = GameObject.FindGameObjectsWithTag("Furniture");
        foreach (GameObject obj in existingFurniture)
        {
            Destroy(obj);
        }

        //mark which prefab to spawn for each furniture data
        foreach (FurnitureData data in layout.furniture)
        {
            GameObject prefabToSpawn = null;

            switch (data.type)
            {
                case "Chair":
                    prefabToSpawn = chairPrefab;
                    break;
                case "Table":
                    prefabToSpawn = tablePrefab;
                    break;
            }

            //if the prefab is not found an error will debug
            if (prefabToSpawn == null)
            {
                Debug.LogWarning("No prefab found for furniture type: " + data.type);
                continue;
            }

            //it will then call this function to spawn furniture
            SpawnFurnitureFromData(prefabToSpawn, data);
        }

        //quick debug to double check
        Debug.Log("Layout loaded from: " + path);
    }

    private void SpawnFurnitureFromData(GameObject prefab, FurnitureData data)
    {
        //most of the code is reused from placement manager to generate furniture
        GameObject[,] grid = gridManager.grid;
        GameObject hitGrid = grid[data.gridX, data.gridY];

        Vector3 bottomLeft = hitGrid.transform.position;
        FurnitureInfo fp = prefab.GetComponent<FurnitureInfo>();

        Vector3 center = gridManager.GetCenterWorld(bottomLeft, fp.w, fp.h, gridManager.cellSize);

        Quaternion rotation = Quaternion.identity;

        //find object rotation
        if (!string.IsNullOrEmpty(data.rotation))
        {
            string cleaned = data.rotation.Replace(", Rotation:", "").Replace("°", "").Trim();

            if (int.TryParse(cleaned, out int rotY))
            {
                rotation = Quaternion.Euler(0, rotY, 0);
            }
        }

        //we then instantiate the objects and add their data
        GameObject placedObject = Instantiate(prefab, center + Vector3.up * 0.5f, rotation);

        FurnitureInfo furnitureDetails = placedObject.GetComponent<FurnitureInfo>();
        furnitureDetails.furniturePrefab = prefab;
        furnitureDetails.xPos = data.gridX;
        furnitureDetails.yPos = data.gridY;
        furnitureDetails.rotation = data.rotation;
        furnitureDetails.objectPlaced = true;

        furnitureDetails.AssignInstanceName();
        placedObject.name = furnitureDetails.symbol;

        //fill in the grid manager with it
        gridManager.Fill(data.gridX, data.gridY, furnitureDetails.w, furnitureDetails.h, furnitureDetails.symbol, furnitureDetails.rotation);
    }
}

[Serializable]
//room layout and furniture data
public class LayoutData
{
    public int roomWidth;
    public int roomHeight;
    public List<FurnitureData> furniture = new List<FurnitureData>();
    public string timeStamp;
}

[Serializable]
//key furniture data to store
public class FurnitureData
{
    public string type;
    public int gridX;
    public int gridY;
    public int width;
    public int height;
    public string rotation;
}