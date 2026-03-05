using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 5f;
    public bool rotateOnDrag = true;
    public bool smoothRotation = true;
    
    [Header("Limits")]
    public bool limitVerticalRotation = true;
    public float minVerticalAngle = -30f;
    public float maxVerticalAngle = 30f;
    
    private Vector3 lastMousePosition;
    private bool isDragging = false;
    private float currentXRotation = 0f;
    private float currentYRotation = 0f;
    
    void Update()
    {
        if (!GameManager.Instance.IsGameActive())
            return;
            
        HandleRotation();
    }
    
    void HandleRotation()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Check if not clicking on UI or shape
            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                isDragging = true;
                lastMousePosition = Input.mousePosition;
            }
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
        
        if (isDragging && rotateOnDrag)
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            
            // Rotate horizontally (around Y axis)
            float rotY = delta.x * rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, -rotY, Space.World);
            
            // Optional vertical rotation (around X axis)
            if (!limitVerticalRotation)
            {
                float rotX = delta.y * rotationSpeed * Time.deltaTime;
                transform.Rotate(Vector3.right, rotX, Space.World);
            }
            else
            {
                // Limited vertical rotation
                currentXRotation += delta.y * rotationSpeed * Time.deltaTime;
                currentXRotation = Mathf.Clamp(currentXRotation, minVerticalAngle, maxVerticalAngle);
                
                // Apply rotation
                transform.rotation = Quaternion.Euler(currentXRotation, transform.eulerAngles.y, 0);
            }
            
            lastMousePosition = Input.mousePosition;
        }
    }
    
    // Public method to reset rotation
    public void ResetRotation()
    {
        transform.rotation = Quaternion.identity;
        currentXRotation = 0f;
        currentYRotation = 0f;
    }
}
