using System;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Object References")]
    public GameObject mainUI;
    public GameObject collapseButton;
    public GameObject objectEditUI;
    public GameObject rotateUI;

    [Header("Current UI State")]
    public static String currentState;

    [Header("Toolbar")]
    public Animator toolbarAnim;

    void Start()
    {
        currentState = "main";
    }

    void Update()
    {
        //switches between the current UI states (changed in other scripts)
        switch (currentState)
        {
            case "main":
                SetObjectState(new GameObject[] { mainUI, collapseButton, objectEditUI, rotateUI }, new bool[] { true, true, false, false });
                break;
            case "objectEdit":
                SetObjectState(new GameObject[] { mainUI, collapseButton, objectEditUI, rotateUI }, new bool[] { false, false, true, false });
                break;
            case "rotate":
                SetObjectState(new GameObject[] { mainUI, collapseButton, objectEditUI, rotateUI }, new bool[] { false, false, false, true });
                break;
        }
    }

    void SetObjectState(GameObject[] uiObjects, bool[] state)
    {
        //turns gameobjects on and off
        for (int i = 0; i < uiObjects.Length; i++)
        {
            uiObjects[i].SetActive(state[i]);
        }
    }

    //in-game UI toolbar
    public void ToolbarStatus(bool open)
    {
        toolbarAnim.SetBool("isOpen", open);
    }
}