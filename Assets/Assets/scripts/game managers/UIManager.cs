using System;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Object References")]
    public GameObject mainUI;
    public GameObject objectEditUI;
    public GameObject rotateUI;

    [Header("Current UI State")]
    public static String currentState;

    void Update()
    {
        //switches between the current UI states (changed in other scripts)
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
        //turns gameobjects on and off
        for(int i = 0; i < uiObjects.Length; i++)
        {
            uiObjects[i].SetActive(state[i]);
        }
    }
}