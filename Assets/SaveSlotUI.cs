using UnityEngine;
using System.IO;
using TMPro;

public class SaveSlotUI : MonoBehaviour
{
    [Header("Script References")]
    public LayoutSaveSystem layoutSaveSystem;
    public SettingsManager settingsManager;

    [Header("Save Slots")]
    public GameObject mainSaveUI;
    public GameObject clickedSaveScreen;
    public GameObject loadButton;
    public int saveSlotNum;

    [Header("UI Interface")]
    public GameObject filledSlotUI;
    public GameObject emptySlotUI;
    public TextMeshProUGUI roomSize;
    public TextMeshProUGUI furnitureCount;
    public TextMeshProUGUI timeStamp;
    public void ClickedUISlot()
    {
        mainSaveUI.SetActive(false);
        clickedSaveScreen.SetActive(true);
    }

    public void SaveClicked()
    {
        layoutSaveSystem.SaveLayout(saveSlotNum);
        CancelButton();
    }

    public void LoadClicked()
    {
        layoutSaveSystem.LoadLayout(saveSlotNum);
        settingsManager.settingsToggle = false;
        CancelButton();
    }

    public void CancelButton()
    {
        clickedSaveScreen.SetActive(false);
        mainSaveUI.SetActive(true);
    }

    public void DeleteSaveButton()
    {
        string path = Application.persistentDataPath + "/layout" + saveSlotNum + ".json";

        //checks if the save path exists and generates the data
        if (!File.Exists(path))
        {
            return;
        }

        File.Delete(path);
        CancelButton();
    }

    void Update()
    {
        string path = Application.persistentDataPath + "/layout" + saveSlotNum + ".json";

        //checks if the save path exists and generates the data
        if (!File.Exists(path))
        {
            emptySlotUI.SetActive(true);
            filledSlotUI.SetActive(false);
            loadButton.SetActive(false);
            return;
        }
        else
        {
            emptySlotUI.SetActive(false);
            filledSlotUI.SetActive(true);
            loadButton.SetActive(true);
        }

        //read the JSON and find the layout data
        string json = File.ReadAllText(path);
        LayoutData layout = JsonUtility.FromJson<LayoutData>(json);

        //setting UI up
        roomSize.text = layout.roomWidth + "x" + layout.roomHeight + " Room";
        furnitureCount.text = "Furniture Count: " + layout.furniture.Count.ToString();
        timeStamp.text = layout.timeStamp;
    }
}
