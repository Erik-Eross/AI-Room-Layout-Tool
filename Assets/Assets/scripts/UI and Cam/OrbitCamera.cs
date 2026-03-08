using UnityEngine;
using Unity.Cinemachine;

public class OrbitCamera : MonoBehaviour
{
    [Header("Camera Reference")]
    public CinemachineCamera cam;
    
    [Header("Zoom")]
    public float minFov = 20f;
    private float maxFov;

    [Header("Sensitivity")]
    public float sensitivity;
    public float zoomSensitivity;

    [Header("Cam Pitch")]
    public float minPitch;
    public float maxPitch;
    float yaw;
    float pitch;
    public GridManager grid;
    public Transform followTarget;
    public float padding;
    CinemachineOrbitalFollow orbital;

    void Awake()
    {
        orbital = cam.GetComponent<CinemachineOrbitalFollow>();

        //setup yaw and pitch 
        yaw = orbital.HorizontalAxis.Value;

        pitch = orbital.VerticalAxis.Value;

        followTarget.position = grid.transform.position;

        //set radius based on grid size
        float w = grid.width * grid.cellSize;
        float h = grid.height * grid.cellSize;
        float biggest = Mathf.Max(w, h);

        orbital.Radius = biggest + padding;

        //set max fov on start
        maxFov = cam.Lens.FieldOfView;
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        
        //only zoom if the input is large enough
        if (Mathf.Abs(scroll) > 0.0001f && cam != null)
        {
            //controls field of view (FOV)
            var lens = cam.Lens;
            lens.FieldOfView -= scroll * zoomSensitivity * 5f;
            lens.FieldOfView = Mathf.Clamp(lens.FieldOfView, minFov, maxFov);
            cam.Lens = lens;
        }

        //rotate around the target when right mouse button is held
        if (!Input.GetMouseButton(1))
            return;

        yaw   += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        orbital.HorizontalAxis.Value = yaw;
        orbital.VerticalAxis.Value = pitch;
    }
}
