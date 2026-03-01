using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public Camera cam;
    public float cellSize;
    public LayerMask gridLayer;
    public GridManager gridManager;
    public GameObject chosenPrefab;
    public GameObject previewObject;
    private GameObject lastChosenPrefab;
    private Renderer[] previewRenderers;
    public EditObjects editObjectsScript;
    public GameObject rotateButtonsUI;
    
    [Header("UI")]
    public GameObject chairObject;
    public GameObject tableObject;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) 
            && chosenPrefab != null && !editObjectsScript.hoveringObject)
        {
            TryPlace();
        }

        if (Input.GetMouseButtonDown(1) && chosenPrefab != null)
        {
            chosenPrefab = null;
            if (previewObject != null) Destroy(previewObject);
            previewObject = null;
        }

        if(chosenPrefab != null)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, gridLayer))
            {
                //finds the grid hit and checks for the gridmanager script
                GridManager hitGrid = hit.transform.GetComponentInParent<GridManager>();
                if (hitGrid == null) hitGrid = gridManager;

                if (chosenPrefab != lastChosenPrefab || previewObject == null)
                {
                    if (previewObject != null) Destroy(previewObject);
                    previewObject = Instantiate(chosenPrefab);

                    previewRenderers = previewObject.GetComponentsInChildren<Renderer>();
                    lastChosenPrefab = chosenPrefab;
                }

                Vector3 hitPos = hit.point;
                var furnitureDetails = chosenPrefab.GetComponent<FurnitureInfo>();
                Vector2Int gridHit = hitGrid.WorldToGrid(hitPos);

                //snaps the preview object to the cell
                //Vector3 cellCenter = hitGrid.GridToWorld(gridHit.x, gridHit.y) + Vector3.up * 0.5f;
                
                Vector3 center = hitGrid.GetCenterWorld(hit.collider.transform.position, furnitureDetails.w, furnitureDetails.h, gridManager.cellSize);
                previewObject.transform.position = center + Vector3.up * 0.5f;

                if (hitGrid.CanPlace(gridHit.x, gridHit.y, furnitureDetails.w, furnitureDetails.h))
                {
                    //show green preview
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
                    //show red preview
                    if (previewRenderers != null)
                    {
                        foreach (Renderer renderer in previewRenderers)
                        {
                            if (renderer.material != null) renderer.material.color = Color.red;
                        }
                    }
                }

                rotateButtonsUI.SetActive(true);
                if(Input.GetKeyDown(KeyCode.Q))
                {
                    previewObject.transform.Rotate(0, -45, 0);
                }
                if(Input.GetKeyDown(KeyCode.E))
                {
                    previewObject.transform.Rotate(0, 45, 0);
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

    void TryPlace()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, gridLayer))
        {
            GridManager hitGrid = hit.transform.GetComponentInParent<GridManager>();
            if (hitGrid == null) hitGrid = gridManager; // fallback

            Vector3 hitPos = hit.point;
            Vector2Int gridHit = hitGrid.WorldToGrid(hitPos);
            var chosenFurnitureDetails = chosenPrefab.GetComponent<FurnitureInfo>();

            if (hitGrid.CanPlace(gridHit.x, gridHit.y, chosenFurnitureDetails.w, chosenFurnitureDetails.h))
            {
                //Vector3 placePos = hitGrid.GridToWorld(gridHit.x, gridHit.y) + Vector3.up * 0.5f;
                //GameObject placedObject = Instantiate(chosenPrefab, placePos, Quaternion.identity);
                Vector3 bottomLeft = hit.collider.transform.position;
                var fp = chosenPrefab.GetComponent<FurnitureInfo>();

                Vector3 center = hitGrid.GetCenterWorld(bottomLeft, fp.w, fp.h, gridManager.cellSize);
                GameObject placedObject = Instantiate(chosenPrefab, center + Vector3.up * 0.5f, previewObject.transform.rotation);
                
                var furnitureDetails = placedObject.GetComponent<FurnitureInfo>();

                // store the placed rotation before filling the grid so the ascii map reflects it
                float yRot = placedObject.transform.eulerAngles.y;
                if (yRot > 180f)
                    yRot -= 360f;
                furnitureDetails.rotation = yRot;
                furnitureDetails.furniturePrefab = chosenPrefab;
                furnitureDetails.xPos = gridHit.x;
                furnitureDetails.yPos = gridHit.y;
                furnitureDetails.objectPlaced = true;
                
                hitGrid.Fill(gridHit.x, gridHit.y, furnitureDetails.w, furnitureDetails.h, furnitureDetails.symbol, yRot);
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
