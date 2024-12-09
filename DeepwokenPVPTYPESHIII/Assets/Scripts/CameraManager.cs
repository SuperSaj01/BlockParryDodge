using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public PlayerManager player;
    public static CameraManager instance {get; private set;}

    private Transform targetTransform;
    public Transform cameraPivot;
    public Transform cam;
    private Vector3 cameraFollowVelocity = Vector3.zero;
    private Vector3 cameraVectorPos;

    private float defaultPos;

    public float followSpeed = 0.2f;
    public float camLookSpeed = 0.2f;
    public float camPivotSpeed = 0.2f;

    public float minPivotAngle = 90f;
    public float maxPivotAngle = -90f;
    

    public float camHInput;
    public float camVInput;

    public float lookAngle;
    public float pivotAngle;

    public float cameraCollisionRadius = 0.2f;
    public LayerMask collisionLayers;
    public float cameraCollisionOffset = 0.1f;
    public float minCollisionOffset = 0.1f;

    private void Awake()
    {
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

    private void FollowTarget()
    {

        Vector3 targetPos = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraFollowVelocity, followSpeed);

        transform.position = targetPos;
    }

    private void RotateCamera()
    {
        
        Vector3 rotation;
        Quaternion targetRotation;

        lookAngle += camHInput * camLookSpeed;
        pivotAngle -= camVInput * camPivotSpeed;

        pivotAngle = Mathf.Clamp(pivotAngle, maxPivotAngle, minPivotAngle);

        rotation = Vector3.zero;
        rotation.y = lookAngle;
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }

    private void HandleCameraCollisions()
    {   
        float targetPos = defaultPos;

        RaycastHit hit;

        Vector3 dir = cam.position - cameraPivot.position;
        dir.Normalize();

        if(Physics.SphereCast(cameraPivot.position, cameraCollisionRadius, dir, out hit, Mathf.Abs(targetPos), collisionLayers))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPos -= (distance - cameraCollisionOffset);
        }

        if(Mathf.Abs(targetPos) < minCollisionOffset)
        {
            targetPos -= minCollisionOffset;
        }

        cameraVectorPos.z = Mathf.Lerp(cam.localPosition.z, targetPos, 0.2f);
        cam.localPosition = cameraVectorPos;
    }
}
