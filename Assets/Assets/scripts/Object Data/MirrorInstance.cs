using UnityEngine;

public class MirrorInstance : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mirrorCamera;
    [SerializeField] private Renderer mirrorRenderer;

    [Header("Render Texture Settings")]
    [SerializeField] private int textureWidth = 1024;
    [SerializeField] private int textureHeight = 1024;
    [SerializeField] private int depthBuffer = 16;

    private RenderTexture renderTexture;
    private Material materialInstance;

    private void Awake()
    {
        //create a unique RenderTexture for this mirror
        renderTexture = new RenderTexture(textureWidth, textureHeight, depthBuffer);
        renderTexture.name = $"MirrorRT_{gameObject.GetInstanceID()}";
        renderTexture.Create();

        mirrorCamera.targetTexture = renderTexture;
        materialInstance = mirrorRenderer.material;
        materialInstance.mainTexture = renderTexture;

        materialInstance.SetTexture("_BaseMap", renderTexture);
    }

    private void OnDestroy()
    {
        if (mirrorCamera != null)
        {
            mirrorCamera.targetTexture = null;
        }

        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }

        if (materialInstance != null)
        {
            Destroy(materialInstance);
        }
    }
}
