using System;
using Unity.VisualScripting;
using UnityEngine;

public class EditObjects : MonoBehaviour
{
    public bool hoveringObject;
    public GameObject selectedObject;
    public PlacementManager placementManagerScript;
    public GridManager gridManagerScript;
    public LayerMask furnitureLayer;
    public GameObject hoveredObject;
    public float emissionHighlightAmount;

    void Update()
    {
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, furnitureLayer))
        {
            FurnitureInfo furnitureInfo = hit.transform.GetComponent<FurnitureInfo>();
            
            if (hit.transform.tag == "Furniture" && furnitureInfo != null 
                && furnitureInfo.canInteract && placementManagerScript.chosenPrefab == null)
            {
                hoveringObject = true;

                if(hoveredObject != null && hoveredObject != hit.transform.gameObject)
                {
                    foreach (Renderer mat in hoveredObject.GetComponentsInChildren<Renderer>())
                    {
                        mat.material.EnableKeyword("_EMISSION");
                        mat.material.SetColor("_EmissionColor", Color.white * 0f);
                    }
                }

                hoveredObject = hit.transform.gameObject;
                Color emissionColor = Color.white * emissionHighlightAmount;

                foreach (Renderer mat in hit.transform.GetComponentsInChildren<Renderer>())
                {
                    mat.material.EnableKeyword("_EMISSION");
                    mat.material.SetColor("_EmissionColor", emissionColor);
                }

                if(Input.GetMouseButtonDown(0))
                {
                    UIManager.currentState = "objectEdit";
                    selectedObject = hit.transform.gameObject;
                    Debug.Log("clicked: " + hit.transform.name);
                }
            }
        }
        else 
        {
            if(hoveredObject) 
            {
                hoveringObject = false;

                foreach (Renderer mat in hoveredObject.GetComponentsInChildren<Renderer>())
                {
                    mat.material.EnableKeyword("_EMISSION");
                    mat.material.SetColor("_EmissionColor", Color.white * 0f);
                }
                
                hoveredObject = null;
            }
        }

        if(Input.GetMouseButtonDown(1) && selectedObject != null)
        {
            selectedObject.SetActive(true);
            Deselect();
        }

        if(selectedObject != null) 
        {
            Color emissionColor = Color.white * emissionHighlightAmount;

            foreach (Renderer mat in selectedObject.GetComponentsInChildren<Renderer>())
            {
                mat.material.EnableKeyword("_EMISSION");
                mat.material.SetColor("_EmissionColor", emissionColor);
            }
        }
    }

    public void MoveObjectButton() {
        if(selectedObject != null)
        {
            UIManager.currentState = "main";
            FurnitureInfo furnitureInfo = selectedObject.GetComponent<FurnitureInfo>();
            gridManagerScript.Fill(furnitureInfo.xPos, furnitureInfo.yPos, 
                furnitureInfo.w, furnitureInfo.h, '.', 0);
            placementManagerScript.chosenPrefab = selectedObject.GetComponent<FurnitureInfo>().furniturePrefab;
            placementManagerScript.chosenPrefab.transform.rotation = selectedObject.transform.rotation;
            hoveringObject = false;
            Destroy(selectedObject);
            selectedObject = null;
        }
    }

    public void RotateObjectButton() {
        UIManager.currentState = "rotate";
    }

    public void DeleteObjectButton() {
        if(selectedObject != null)
        {
            FurnitureInfo furnitureInfo = selectedObject.GetComponent<FurnitureInfo>();
            gridManagerScript.Fill(furnitureInfo.xPos, furnitureInfo.yPos, 
                furnitureInfo.w, furnitureInfo.h, '.', 0);
            hoveringObject = false;
            Destroy(selectedObject);
            Deselect();
        }
    }

    public void LeftRotateButton()
    {
        if(selectedObject != null)
        {
            selectedObject.transform.Rotate(0, -45, 0);
        }
    }

    public void RightRotateButton()
    {
        if(selectedObject != null)
        {
            selectedObject.transform.Rotate(0, 45, 0);
        }
    }

    public void ConfirmRotateButton()
    {
        if(selectedObject != null)
        {
            FurnitureInfo furnitureInfo = selectedObject.GetComponent<FurnitureInfo>();
            float yRot = selectedObject.transform.eulerAngles.y;
                if (yRot > 180f)
                    yRot -= 360f;
            furnitureInfo.rotation = yRot;
            gridManagerScript.Fill(furnitureInfo.xPos, furnitureInfo.yPos, 
                furnitureInfo.w, furnitureInfo.h, furnitureInfo.symbol, furnitureInfo.rotation);
            hoveringObject = false;
            Deselect();
        }
    }

    void Deselect() {
        UIManager.currentState = "main";
        foreach (Renderer mat in selectedObject.GetComponentsInChildren<Renderer>())
        {
            mat.material.EnableKeyword("_EMISSION");
            mat.material.SetColor("_EmissionColor", Color.white * 0f);
        }
        selectedObject = null;
        placementManagerScript.chosenPrefab = null;
        placementManagerScript.previewObject = null;

        GridManager gridManager = FindFirstObjectByType<GridManager>();
        Debug.Log("\n" + gridManager.ToAscii());
    }
}
