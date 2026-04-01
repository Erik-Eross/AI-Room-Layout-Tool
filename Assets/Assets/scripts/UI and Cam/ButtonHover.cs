using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler//, IPointerExitHandler
{
    public ObjectSelectMenu objectSelectMenu;
    public string objectType;
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Hover ON: " + gameObject.name);

        objectSelectMenu.menuType = objectType;
    }

    // public void OnPointerExit(PointerEventData eventData)
    // {
    //     Debug.Log("Hover OFF: " + gameObject.name);

    //     objectSelectMenu.menuType = "";
    // }
}
