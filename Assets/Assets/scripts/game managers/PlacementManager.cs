using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms;

public class PlacementManager : MonoBehaviour
{
    [Header("Object References")]
    public Camera cam;
    public GridManager gridManager;
    public GameObject chosenPrefab;
    public GameObject previewObject;
    private GameObject lastChosenPrefab;
    private Renderer[] previewRenderers;
    public EditObjects editObjectsScript;
    public GameObject rotateButtonsUI;

    [Header("Set Values")]
    public float cellSize;
    public LayerMask gridLayer;
    public float rotationStep = 45f;
    
    [Header("UI")]
    public GameObject chairObject;
    public GameObject tableObject;

    [Header("AI Script Reference")]
    public LocalAI localAIScript;

    void Update()
    {
        //try place an object if one has been chosen
        if (Input.GetMouseButtonDown(0) 
            && chosenPrefab != null && !editObjectsScript.hoveringObject)
        {
            TryPlace();
        }

        //cancel the action if right clicked
        if (Input.GetMouseButtonDown(1) && chosenPrefab != null)
        {
            chosenPrefab = null;
            if (previewObject != null) Destroy(previewObject);
            previewObject = null;
        }

        if(chosenPrefab != null)
        {
            //makes a raycast from the camera and checks to see if you are hovering over a grid cell
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, gridLayer))
            {
                //finds the grid hit and checks for the gridmanager script
                GridManager hitGrid = hit.transform.GetComponentInParent<GridManager>();
                if (hitGrid == null) hitGrid = gridManager;

                if (chosenPrefab != lastChosenPrefab || previewObject == null)
                {
                    //creates a preview of the chosen objects to show where it will place
                    if (previewObject != null) Destroy(previewObject);
                    previewObject = Instantiate(chosenPrefab);

                    previewRenderers = previewObject.GetComponentsInChildren<Renderer>();
                    lastChosenPrefab = chosenPrefab;
                }

                Vector3 hitPos = hit.point;
                var furnitureDetails = chosenPrefab.GetComponent<FurnitureInfo>();
                Vector2Int gridHit = hitGrid.WorldToGrid(hitPos);
                
                Vector3 center = hitGrid.GetCenterWorld(hit.collider.transform.position, furnitureDetails.w, furnitureDetails.h, gridManager.cellSize);
                previewObject.transform.position = center + Vector3.up * 0.5f;

                //checks if the object can be placed
                if (hitGrid.CanPlace(gridHit.x, gridHit.y, furnitureDetails.w, furnitureDetails.h))
                {
                    //show green preview of the object
                    if (previewRenderers != null)
                    {
                        foreach (Renderer renderer in previewRenderers)
                        {
                            if (renderer.material != null) renderer.material.color = Color.green;
                        }
                    }
                }
                else
                {
                    //show red preview of the object
                    if (previewRenderers != null)
                    {
                        foreach (Renderer renderer in previewRenderers)
                        {
                            if (renderer.material != null) renderer.material.color = Color.red;
                        }
                    }
                }

                //allows for rotation of the object before its placed
                rotateButtonsUI.SetActive(true);
                if(Input.GetKeyDown(KeyCode.Q))
                {
                    SnapRotatePreview(previewObject, -rotationStep);
                }
                if(Input.GetKeyDown(KeyCode.E))
                {
                    SnapRotatePreview(previewObject, rotationStep);
                }
            }
            else
            {
                if (previewObject != null) Destroy(previewObject);
                previewObject = null;
                previewRenderers = null;
            }
        }
        else
        {
            rotateButtonsUI.SetActive(false);
        }
    }

    private void SnapRotatePreview(GameObject obj, float deltaAngle)
    {
        //function to help snap the rotation of the object to keep the values consistant
        if (obj == null) return;
        float rawY = obj.transform.eulerAngles.y + deltaAngle;
        float snapped = Mathf.Round(rawY / rotationStep) * rotationStep;
        snapped = Mathf.Repeat(snapped, 360f);
        Vector3 e = obj.transform.eulerAngles;
        obj.transform.eulerAngles = new Vector3(e.x, snapped, e.z);
    }

    void TryPlace()
    {
        //creates a raycast to try to place the object on the grid cell
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, gridLayer))
        {
            GridManager hitGrid = hit.transform.GetComponentInParent<GridManager>();
            if (hitGrid == null) hitGrid = gridManager;

            //checks the position and grid that was hit
            Vector3 hitPos = hit.point;
            Vector2Int gridHit = hitGrid.WorldToGrid(hitPos);
            var chosenFurnitureDetails = chosenPrefab.GetComponent<FurnitureInfo>();

            //if the grid is free, then we will instantiate the furniture onto the grid cell
            if (hitGrid.CanPlace(gridHit.x, gridHit.y, chosenFurnitureDetails.w, chosenFurnitureDetails.h))
            {
                Vector3 bottomLeft = hit.collider.transform.position;
                var fp = chosenPrefab.GetComponent<FurnitureInfo>();

                //find the center of the cell and generate the furniture
                Vector3 center = hitGrid.GetCenterWorld(bottomLeft, fp.w, fp.h, gridManager.cellSize);
                GameObject placedObject = Instantiate(chosenPrefab, center + Vector3.up * 0.5f, previewObject.transform.rotation);
                
                var furnitureDetails = placedObject.GetComponent<FurnitureInfo>();

                //normalize and snap rotation to the configured step displayed as integer degrees
                float rawY = placedObject.transform.eulerAngles.y;
                float snapped = Mathf.Round(rawY / rotationStep) * rotationStep;
                snapped = Mathf.Repeat(snapped, 360f);
                int rotInt = Mathf.RoundToInt(snapped);
                if (rotInt != 0) { furnitureDetails.rotation = ", Rotation: " + rotInt.ToString() + " °"; }
                else { furnitureDetails.rotation = ""; }
                furnitureDetails.furniturePrefab = chosenPrefab;
                furnitureDetails.xPos = gridHit.x;
                furnitureDetails.yPos = gridHit.y;
                furnitureDetails.objectPlaced = true;
                //give a unique symbol to the placed object
                furnitureDetails.AssignInstanceName();
                placedObject.name = furnitureDetails.symbol;
                
                //fill the grid ascii and debug it to the console
                hitGrid.Fill(gridHit.x, gridHit.y, furnitureDetails.w, furnitureDetails.h, furnitureDetails.symbol, furnitureDetails.rotation);
                Debug.Log("\n" + hitGrid.ToAscii());

                if (previewObject != null) Destroy(previewObject);
                chosenPrefab = null;
                previewObject = null;
                previewRenderers = null;
            }
            else
            {
                Debug.Log("Cannot place here");
            }
        }
    }
}
