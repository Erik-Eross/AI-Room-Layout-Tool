using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static String currentState;
    public GameObject mainUI;
    public GameObject objectEditUI;
    public GameObject rotateUI;

    void Update()
    {
        switch(currentState)
        {
            case "main":
                SetObjectState(new GameObject[] {mainUI, objectEditUI, rotateUI}, new bool[] {true, false, false});
                break;
            case "objectEdit":
                SetObjectState(new GameObject[] {mainUI, objectEditUI, rotateUI}, new bool[] {false, true, false});
                break;
            case "rotate":
                SetObjectState(new GameObject[] {mainUI, objectEditUI, rotateUI}, new bool[] {false, false, true});
                break;
        }
    }

    void SetObjectState(GameObject[] uiObjects, bool[] state)
    {
        for(int i = 0; i < uiObjects.Length; i++)
        {
            uiObjects[i].SetActive(state[i]);
        }
    }
}