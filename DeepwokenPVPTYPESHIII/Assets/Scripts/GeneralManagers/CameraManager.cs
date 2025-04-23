using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    private PlayerManager player;
    public static CameraManager instance {get; private set;}

    private Transform targetTransform;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Transform cam;
    private Vector3 cameraFollowVelocity = Vector3.zero;
    private Vector3 cameraVectorPos;
    private float defaultPos;
    
    [Header("Camera Rotation and Pivot")]
    const float minPivotAngle = 90f;
    const float maxPivotAngle = -90f;
    private float lookAngle;
    private float pivotAngle;

    
    [Header("Mouse Input")]
    public float camHInput;
    public float camVInput;

    [Header("Collision Detection")]
    private float cameraCollisionRadius = 0.2f;
    private LayerMask collisionLayers;
    private float cameraCollisionOffset = 0.1f;
    private float minCollisionOffset = 0.1f;

    private void Awake()
    {
        //Creates singleton patter
        if(instance == null)
        {
            instance = this; 
        }
        else
        {
            Destroy(gameObject);
        }

        //Cursor.lockState = CursorLockMode.Locked;
        defaultPos = cam.localPosition.z;
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void HandleAllCameraMovement()
    {
        if(player == null) return;
        FollowTarget();
        RotateCamera();
        HandleCameraCollisions();
    }

    public void SetPlayer(PlayerManager player)
    {
        this.player = player;
    }

    private void FollowTarget()
    {
        float followSpeed = 0.2f;
        // Smoothly move camera to follow the player by transitioning from one position to the next over a set speed
        Vector3 targetPos = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraFollowVelocity, followSpeed);
        transform.position = targetPos;
    }

    private void RotateCamera()
    {
        float camLookSpeed = 0.2f;
        float camPivotSpeed = 0.2f;
        
        Vector3 rotation;
        Quaternion targetRotation;

         // Handle horizontal and vertical rotation
        lookAngle += camHInput * camLookSpeed;
        pivotAngle -= camVInput * camPivotSpeed;

        //clamps camera vertical angle to not go further than the body
        pivotAngle = Mathf.Clamp(pivotAngle, maxPivotAngle, minPivotAngle);

        // Rotate camera around body
        rotation = Vector3.zero;
        rotation.y = lookAngle;
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

         // Rotate camera pivot (up/down)
        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }

    private void HandleCameraCollisions()
    {   
        float targetPos = defaultPos;
        //Calculates direction from camera to body
        Vector3 dir = cam.position - cameraPivot.position;
        dir.Normalize();

          // Check for obstacles behind player using SphereCast
        if(Physics.SphereCast(cameraPivot.position, cameraCollisionRadius, dir, out RaycastHit hit, Mathf.Abs(targetPos), collisionLayers))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPos -= (distance - cameraCollisionOffset);
        }

         // Ensure camera doesn't clip too close to the pivot
        if(Mathf.Abs(targetPos) < minCollisionOffset)
        {
            targetPos -= minCollisionOffset;
        }

        // interpolates camera's Z position
        cameraVectorPos.z = Mathf.Lerp(cam.localPosition.z, targetPos, 0.2f);
        cam.localPosition = cameraVectorPos;
    }
}
