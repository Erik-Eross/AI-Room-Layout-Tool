using UnityEngine;
using System.Diagnostics;
using System.IO;

public class StartLocalServer : MonoBehaviour
{
    void Start()
    {
        //starts the local server from the executable inside of the game files
        string basePath = Application.dataPath;
        string projectRoot = Directory.GetParent(basePath).FullName;

        string exePath = Path.Combine(projectRoot, "LLM", "llama-server.exe");
        string modelPath = Path.Combine(projectRoot, "LLM", "models", "model.gguf");

        Process.Start(exePath, $"-m \"{modelPath}\" -c 2048");
    }
}
