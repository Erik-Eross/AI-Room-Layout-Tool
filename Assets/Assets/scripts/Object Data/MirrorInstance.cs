using UnityEngine;

public class MirrorInstance : MonoBehaviour
{
    [Header("References")]
    public Camera mirrorCamera;
    public Renderer mirrorRenderer;

    [Header("Render Texture Settings")]
    private int textureWidth = 1024;
    private int textureHeight = 1024;
    private int depthBuffer = 16;

    private RenderTexture renderTexture;
    private Material materialInstance;

    private void Awake()
    {
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
