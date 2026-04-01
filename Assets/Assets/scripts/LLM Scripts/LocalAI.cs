using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Collections;
using TMPro;
using Mono.Cecil;
using System.Collections.Generic;
using Unity.Mathematics;

public class LocalAI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI outputText;
    public TMP_InputField inputField;

    //the structure of the request that will be sent to the local AI server
    public class CompletionRequest
    {
        public string prompt;
        public int n_predict = 140;
        public float temperature = 0.2f;
        public float repeat_penalty = 1.25f;
        public string[] stop = new[] { "\n- -", "\n\n", "Answer:", "ASCII:" };
    }

    //structure of the response returned from the AI server
    public class CompletionResponse
    {
        public string content;
        public string response;
    }

    //this will be called when the user presses the button for assistance
    public void AskAI()
    {
        if(inputField.text != "")
        {
            GridManager gridManager = FindAnyObjectByType<GridManager>();

            //checks for user input, gets the room layout, and start a request
            if(this.enabled)
            {
                StopAllCoroutines();
                StartCoroutine(SendRequest(gridManager.ToAscii(), 
                    gridManager.width.ToString() + "x" + 
                    gridManager.height.ToString(), inputField.text));
                outputText.text = "Asking the AI for suggestions...";
            }
        }
        else {
            //if no text was provided
            outputText.text = "Please enter a question for the AI.";
        }

        //clear the chat after each click
        inputField.text = "";
    }

    IEnumerator SendRequest(string ascii, string roomSize, string userInput)
    {
        //structured prompt is given to the AI and sends it to the local server
        string prompt =
            "You are a room layout assistant, and a user is asking a question please be helpful to them it is very important.\n" +
            "IMPORTANT: keep your response short and simple and to 100 characters MAX.\n" +
            "I will provide an ascii map showing where the current furnitures are placed.\n" +
            "Room size: " + roomSize + "\n" +
            "ASCII map:\n" + ascii + "\n" +
            "User Input: " + userInput + "\n\n" +
            "NOTE: the first value of the grid is the top left of the room, all objects base rotation (0 degrees) are facing the bottom of the room by default.\n" + 
            "Dont give grid coordinates, instead say words like 'top left' or 'middle', to show positioning.\n" +
            "IMPORTANT: dont refer to furniture as 'Chair1' or 'Chair5' just say Chair, do this for all furniture.\n" + 
            "Please make the advice actually useful dont make any up you cannot see in the ascii map.\n" +
            "Dont just say where the furniture is or give context, the user knows what the room looks like, keep the answer JUST your suggestion.\n" +
            "Give only your answer no context keep it plain english treat it like you are talking to another person just plain english.\n" +
            "REMEMBER: If the user's question isnt relevant to this task, please mention how it is important to stay relevant.\n" + 
            "Answer:\n\n";
        CompletionRequest requestBody = new CompletionRequest
        {
            prompt = prompt
        };

        //it then converts the request into JSON and sends it as a POST request to the local AI endpoint
        string json = JsonUtility.ToJson(requestBody);

        UnityWebRequest req = new UnityWebRequest("http://localhost:8080/completion", "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        //we the read the AI response, extract the actual text and remove any unwanted output
        if (req.result == UnityWebRequest.Result.Success)
        {
            var raw = req.downloadHandler.text;
            var data = JsonUtility.FromJson<CompletionResponse>(raw);

            string aiText = !string.IsNullOrEmpty(data.content) ? data.content : data.response;

            aiText = aiText.Trim();

            Debug.Log("Raw AI response:\n" + aiText);

            //remove any leading junk before first letter
            int firstLetter = aiText.IndexOfAny("ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray());
            if (firstLetter > 0)
                aiText = aiText.Substring(firstLetter);

            outputText.text = "AI Evaluation:\n" + string.Join("\n", aiText);
        }
        else
        {
            //shows an error if the AI request fails
            Debug.LogError(req.error);
            Debug.Log(req.downloadHandler.text);
            outputText.text = "Error: " + req.error;
        }
    }
}