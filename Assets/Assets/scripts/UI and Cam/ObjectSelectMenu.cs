using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectSelectMenu : MonoBehaviour
{
    //menu type set in button hover scripts
    public string menuType;

    [Header("Select Menus")]
    public GameObject chairSelectMenu;
    public GameObject tableSelectMenu;
    public GameObject bedSelectMenu;
    public GameObject closetSelectMenu;
    public GameObject couchSelectMenu;
    public GameObject drawerSelectMenu;
    public GameObject lightSelectMenu;
    void Update()
    {
        //selects the menu to be able to pick the furniture
        switch(menuType)
        {
            case "Chair":
                EnableMenu(chairSelectMenu);
                break;
            case "Table":
                EnableMenu(tableSelectMenu);
                break;
            case "Bed":
                EnableMenu(bedSelectMenu);
                break;
            case "Closet":
                EnableMenu(closetSelectMenu);
                break;
            case "Couch":
                EnableMenu(couchSelectMenu);
                break;
            case "Drawer":
                EnableMenu(drawerSelectMenu);
                break;
            case "Light":
                EnableMenu(lightSelectMenu);
                break;
            default:
                EnableMenu(null);
                break;
        }

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            // If NOT clicking on UI
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                menuType = "";
            }
        }
    }

    void EnableMenu(GameObject obj)
    {
        chairSelectMenu.SetActive(false);
        tableSelectMenu.SetActive(false);
        bedSelectMenu.SetActive(false);
        closetSelectMenu.SetActive(false);
        couchSelectMenu.SetActive(false);
        drawerSelectMenu.SetActive(false);
        lightSelectMenu.SetActive(false);

        if(obj != null) { obj.SetActive(true); }
    }
}
