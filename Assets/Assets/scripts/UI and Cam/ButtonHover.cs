using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler//, IPointerExitHandler
{
    public ObjectSelectMenu objectSelectMenu;
    public string objectType;
    public void OnPointerEnter(PointerEventData eventData)
    {
        objectSelectMenu.menuType = objectType;
    }
}
