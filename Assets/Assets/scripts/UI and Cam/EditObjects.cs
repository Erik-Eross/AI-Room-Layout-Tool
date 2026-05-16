using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditObjects : MonoBehaviour
{
    [Header("Object References")]
    public bool hoveringObject;
    public GameObject selectedObject;
    public GameObject hoveredObject;

    [Header("Script References")]
    public PlacementManager placementManagerScript;
    public GridManager gridManagerScript;
    public ObjectSelectMenu objectSelectMenu;

    [Header("Other Settings")]
    public LayerMask furnitureLayer;
    public float emissionHighlightAmount;
    public float rotationStep = 45f;

    void Update()
    {
        //create a raycast from the camera to check if we are hovering over a furniture
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, furnitureLayer))
        {
            //grabs the details from the script of the furniture we are hovering over
            FurnitureInfo furnitureInfo = hit.transform.GetComponent<FurnitureInfo>();

            if (hit.transform.tag == "Furniture" && furnitureInfo != null && !IsPointerOverUI()
                && furnitureInfo.canInteract && placementManagerScript.chosenPrefab == null)
            {
                hoveringObject = true;

                //deselects all object select menus
                objectSelectMenu.menuType = "";

                //gives the hovered object a glow to show the user whats happening
                if (hoveredObject != null && hoveredObject != hit.transform.gameObject)
                {
                    //the object emission color is reset
                    foreach (Renderer renderer in hoveredObject.GetComponentsInChildren<Renderer>())
                    {
                        Material[] mats = renderer.materials;

                        foreach (Material mat in mats)
                        {
                            if (mat != null)
                            {
                                mat.EnableKeyword("_EMISSION");
                                mat.SetColor("_EmissionColor", Color.white * 0f);
                            }
                        }
                    }
                }

                hoveredObject = hit.transform.gameObject;
                Color emissionColor = Color.white * emissionHighlightAmount;

                foreach (Renderer renderer in hoveredObject.GetComponentsInChildren<Renderer>())
                {
                    Material[] mats = renderer.materials;

                    foreach (Material mat in mats)
                    {
                        if (mat != null)
                        {
                            mat.EnableKeyword("_EMISSION");
                            mat.SetColor("_EmissionColor", emissionColor);
                        }
                    }
                }

                //if you left click the object is selected
                if (Input.GetMouseButtonDown(0))
                {
                    if (selectedObject != null)
                    {
                        foreach (Renderer renderer in selectedObject.GetComponentsInChildren<Renderer>())
                        {
                            Material[] mats = renderer.materials;

                            foreach (Material mat in mats)
                            {
                                if (mat != null)
                                {
                                    mat.EnableKeyword("_EMISSION");
                                    mat.SetColor("_EmissionColor", Color.white * 0f);
                                }
                            }
                        }
                    }

                    foreach (Renderer renderer in hoveredObject.GetComponentsInChildren<Renderer>())
                    {
                        Material[] mats = renderer.materials;

                        foreach (Material mat in mats)
                        {
                            if (mat != null)
                            {
                                mat.EnableKeyword("_EMISSION");
                                mat.SetColor("_EmissionColor", Color.white * 0f);
                            }
                        }
                    }

                    //select the object and change the UI state
                    UIManager.currentState = "objectEdit";
                    selectedObject = hit.transform.gameObject;
                    Debug.Log("clicked: " + hit.transform.name);
                }
            }
        }
        else
        {
            //here we reset the object and its emissions
            if (hoveredObject)
            {
                hoveringObject = false;

                foreach (Renderer renderer in hoveredObject.GetComponentsInChildren<Renderer>())
                {
                    Material[] mats = renderer.materials;

                    foreach (Material mat in mats)
                    {
                        if (mat != null)
                        {
                            mat.EnableKeyword("_EMISSION");
                            mat.SetColor("_EmissionColor", Color.white * 0f);
                        }
                    }
                }

                hoveredObject = null;
            }
        }

        //if you right click and have a selected object it will deselect
        if (Input.GetMouseButtonDown(1) && selectedObject != null)
        {
            selectedObject.SetActive(true);
            Deselect();
        }

        if (selectedObject != null)
        {
            //make the object glow again with emission
            Color emissionColor = Color.white * emissionHighlightAmount;

            foreach (Renderer renderer in selectedObject.GetComponentsInChildren<Renderer>())
            {
                Material[] mats = renderer.materials;

                foreach (Material mat in mats)
                {
                    if (mat != null)
                    {
                        mat.EnableKeyword("_EMISSION");
                        mat.SetColor("_EmissionColor", emissionColor);
                    }
                }
            }
        }
    }

    //move UI button when an object is selected
    public void MoveObjectButton()
    {
        if (selectedObject != null)
        {
            //We reset the UI and the grid to account for the object being moved
            UIManager.currentState = "main";
            FurnitureInfo furnitureInfo = selectedObject.GetComponent<FurnitureInfo>();
            Vector2Int clearedSize = FurnitureInfo.GetRotatedSize(
                furnitureInfo.furniturePrefab.GetComponent<FurnitureInfo>().w,
                furnitureInfo.furniturePrefab.GetComponent<FurnitureInfo>().h,
                selectedObject.transform.eulerAngles.y);
            gridManagerScript.Fill(furnitureInfo.xPos, furnitureInfo.yPos,
                clearedSize.x, clearedSize.y, ".", "");
            placementManagerScript.chosenPrefab = selectedObject.GetComponent<FurnitureInfo>().furniturePrefab;
            placementManagerScript.chosenPrefab.transform.rotation = selectedObject.transform.rotation;
            hoveringObject = false;
            Destroy(selectedObject);
            selectedObject = null;
        }
    }

    public void RotateObjectButton()
    {
        //sets the UI button states
        UIManager.currentState = "rotate";
    }

    public void DeleteObjectButton()
    {
        if (selectedObject != null)
        {
            //if an object is selected, it will be deleted and the grid cell will be reset
            FurnitureInfo furnitureInfo = selectedObject.GetComponent<FurnitureInfo>();
            Vector2Int clearedSize = FurnitureInfo.GetRotatedSize(
                furnitureInfo.furniturePrefab.GetComponent<FurnitureInfo>().w,
                furnitureInfo.furniturePrefab.GetComponent<FurnitureInfo>().h,
                selectedObject.transform.eulerAngles.y);
            gridManagerScript.Fill(furnitureInfo.xPos, furnitureInfo.yPos,
                clearedSize.x, clearedSize.y, ".", "");
            hoveringObject = false;
            Destroy(selectedObject);
            Deselect();

            GameObject[] allFurniture = GameObject.FindGameObjectsWithTag("Furniture");

            //re-assign names of objects when one gets deleted
            foreach (GameObject furniture in allFurniture)
            {
                var details = furniture.GetComponent<FurnitureInfo>();
                //details.AssignInstanceName();
            }
        }
    }

    public void LeftRotateButton()
    {
        //rotate the object 45 degrees to the left
        if (selectedObject != null)
        {
            SnapRotatePreview(selectedObject, rotationStep);
            RepositionSelectedObjectOnGrid();
        }
    }

    public void RightRotateButton()
    {
        //rotate the object 45 degrees to the right
        if (selectedObject != null)
        {
            SnapRotatePreview(selectedObject, -rotationStep);
            RepositionSelectedObjectOnGrid();
        }
    }

    public void ConfirmRotateButton()
    {
        //if the button is clicked and there is an object selected
        if (selectedObject != null)
        {
            FurnitureInfo furnitureInfo = selectedObject.GetComponent<FurnitureInfo>();
            Vector2Int oldSize = FurnitureInfo.GetRotatedSize(
                furnitureInfo.furniturePrefab.GetComponent<FurnitureInfo>().w,
                furnitureInfo.furniturePrefab.GetComponent<FurnitureInfo>().h,
                FurnitureInfo.GetRotationDegrees(furnitureInfo.rotation));
            int oldRotationDegrees = FurnitureInfo.GetRotationDegrees(furnitureInfo.rotation);
            string previousRotation = furnitureInfo.rotation;

            //normalize and snap rotation to the configured step displayed as integer degrees
            float rawY = GetRelativeYawDegrees(selectedObject.transform, furnitureInfo.furniturePrefab.transform);
            float snapped = Mathf.Round(rawY / rotationStep) * rotationStep;
            snapped = Mathf.Repeat(snapped, 360f);
            int rotInt = Mathf.RoundToInt(snapped);
            if (rotInt != 0) { furnitureInfo.rotation = ", Rotation: " + rotInt.ToString() + " °"; }
            else { furnitureInfo.rotation = ""; }

            Vector2Int newSize = FurnitureInfo.GetRotatedSize(
                furnitureInfo.furniturePrefab.GetComponent<FurnitureInfo>().w,
                furnitureInfo.furniturePrefab.GetComponent<FurnitureInfo>().h,
                snapped);

            //clear the old footprint before testing the new one
            gridManagerScript.Fill(furnitureInfo.xPos, furnitureInfo.yPos,
                oldSize.x, oldSize.y, ".", "");

            if (!gridManagerScript.CanPlace(furnitureInfo.xPos, furnitureInfo.yPos, newSize.x, newSize.y))
            {
                //restore previous rotation and footprint on failure
                furnitureInfo.rotation = previousRotation;
                selectedObject.transform.eulerAngles = new Vector3(
                    selectedObject.transform.eulerAngles.x,
                    oldRotationDegrees,
                    selectedObject.transform.eulerAngles.z);
                RepositionSelectedObjectOnGrid();
                gridManagerScript.Fill(furnitureInfo.xPos, furnitureInfo.yPos,
                    oldSize.x, oldSize.y, furnitureInfo.furnitureId, furnitureInfo.rotation);
                Debug.LogWarning("Rotate failed: object cannot fit after rotation.");
                return;
            }

            RepositionSelectedObjectOnGrid();

            //then fill in the grid and deselect the object
            gridManagerScript.Fill(furnitureInfo.xPos, furnitureInfo.yPos,
                newSize.x, newSize.y, furnitureInfo.furnitureId, furnitureInfo.rotation);
            hoveringObject = false;
            Deselect();
        }
    }

    void Deselect()
    {
        //handles reseting the UI and deselecting the object
        UIManager.currentState = "main";
        foreach (Renderer renderer in selectedObject.GetComponentsInChildren<Renderer>())
        {
            Material[] mats = renderer.materials;

            foreach (Material mat in mats)
            {
                if (mat != null)
                {
                    mat.EnableKeyword("_EMISSION");
                    mat.SetColor("_EmissionColor", Color.white * 0f);
                }
            }
        }
        selectedObject = null;
        placementManagerScript.chosenPrefab = null;
        placementManagerScript.previewObject = null;

        //we then debug the ASCII map to confirm
        GridManager gridManager = FindFirstObjectByType<GridManager>();
        Debug.Log("\n" + gridManager.ToAscii());
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

    private void RepositionSelectedObjectOnGrid()
    {
        if (selectedObject == null) return;
        FurnitureInfo furnitureInfo = selectedObject.GetComponent<FurnitureInfo>();
        Vector2Int rotatedSize = FurnitureInfo.GetRotatedSize(
            furnitureInfo.furniturePrefab.GetComponent<FurnitureInfo>().w,
            furnitureInfo.furniturePrefab.GetComponent<FurnitureInfo>().h,
            selectedObject.transform.eulerAngles.y);
        Vector3 bottomLeft = gridManagerScript.grid[furnitureInfo.xPos, furnitureInfo.yPos].transform.position;
        selectedObject.transform.position = gridManagerScript.GetCenterWorld(bottomLeft, rotatedSize.x, rotatedSize.y, gridManagerScript.cellSize);
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

    bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
