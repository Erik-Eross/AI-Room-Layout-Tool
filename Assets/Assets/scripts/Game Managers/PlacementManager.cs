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
    //public float cellSize;
    public LayerMask gridLayer;
    public float rotationStep = 45f;

    // [Header("UI")]
    // public GameObject chairObject;
    // public GameObject tableObject;

    // [Header("AI Script Reference")]
    // public LocalAI localAIScript;

    void Update()
    {
        if (gridManager.generatedFirstGrid)
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

            if (chosenPrefab != null)
            {
                //enable grid highlight for all cells
                foreach (GameObject obj in gridManager.grid)
                {
                    obj.transform.GetChild(0).gameObject.SetActive(true);
                }

                //makes a raycast from the camera and checks to see if you are hovering over a grid cell
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100f, gridLayer))
                {
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
                    Vector2Int gridHit = gridManager.WorldToGrid(hitPos);
                    Vector2Int previewSize = FurnitureInfo.GetRotatedSize(furnitureDetails.w, furnitureDetails.h, previewObject.transform.eulerAngles.y);

                    Vector3 center = gridManager.GetCenterWorld(hit.collider.transform.position, previewSize.x, previewSize.y, gridManager.cellSize);
                    previewObject.transform.position = center;

                    //checks if the object can be placed
                    if (gridManager.CanPlace(gridHit.x, gridHit.y, previewSize.x, previewSize.y))
                    {
                        //show green preview of the object
                        if (previewRenderers != null)
                        {
                            foreach (Renderer renderer in previewRenderers)
                            {
                                Material[] mats = renderer.materials;

                                foreach (Material mat in mats)
                                {
                                    if (mat != null)
                                    {
                                        mat.color = Color.green;
                                    }
                                }
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
                                Material[] mats = renderer.materials;

                                foreach (Material mat in mats)
                                {
                                    if (mat != null)
                                    {
                                        mat.color = Color.red;
                                    }
                                }
                            }
                        }
                    }

                    //allows for rotation of the object before its placed
                    rotateButtonsUI.SetActive(true);
                    if (Input.GetKeyDown(KeyCode.Q))
                    {
                        SnapRotatePreview(previewObject, -rotationStep);
                    }
                    if (Input.GetKeyDown(KeyCode.E))
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

                //disable grid highlight for all cells
                foreach (GameObject obj in gridManager.grid)
                {
                    obj.transform.GetChild(0).gameObject.SetActive(false);
                }
            }
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

    private float GetRelativeYawDegrees(Transform actual, Transform prefab)
    {
        Vector3 actualForward = Vector3.ProjectOnPlane(actual.forward, Vector3.up).normalized;
        Vector3 prefabForward = Vector3.ProjectOnPlane(prefab.forward, Vector3.up).normalized;

        if (actualForward.sqrMagnitude < 0.001f || prefabForward.sqrMagnitude < 0.001f)
            return 0f;

        float angle = Vector3.SignedAngle(prefabForward, actualForward, Vector3.up);
        return Mathf.Repeat(angle, 360f);
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
            float placementYaw = previewObject != null ? previewObject.transform.eulerAngles.y : chosenPrefab.transform.eulerAngles.y;
            Vector2Int placementSize = FurnitureInfo.GetRotatedSize(chosenFurnitureDetails.w, chosenFurnitureDetails.h, placementYaw);

            //if the grid is free, then we will instantiate the furniture onto the grid cell
            if (hitGrid.CanPlace(gridHit.x, gridHit.y, placementSize.x, placementSize.y))
            {
                Vector3 bottomLeft = hit.collider.transform.position;
                var fp = chosenPrefab.GetComponent<FurnitureInfo>();

                //find the center of the cell and generate the furniture
                Vector3 center = hitGrid.GetCenterWorld(bottomLeft, placementSize.x, placementSize.y, gridManager.cellSize);
                GameObject placedObject = Instantiate(chosenPrefab, center, previewObject.transform.rotation);

                var furnitureDetails = placedObject.GetComponent<FurnitureInfo>();

                //normalize and snap rotation to the configured step displayed as integer degrees
                float rawY = GetRelativeYawDegrees(placedObject.transform, chosenPrefab.transform);
                float snapped = Mathf.Round(rawY / rotationStep) * rotationStep;
                snapped = Mathf.Repeat(snapped, 360f);
                int rotInt = Mathf.RoundToInt(snapped);
                if (rotInt != 0) { furnitureDetails.rotation = ", Rotation: " + rotInt.ToString() + " °"; }
                else { furnitureDetails.rotation = ""; }
                furnitureDetails.furniturePrefab = chosenPrefab;
                furnitureDetails.xPos = gridHit.x;
                furnitureDetails.yPos = gridHit.y;
                furnitureDetails.objectPlaced = true;

                //fill the grid ascii and debug it to the console
                hitGrid.Fill(gridHit.x, gridHit.y, placementSize.x, placementSize.y, furnitureDetails.furnitureId, furnitureDetails.rotation);
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
