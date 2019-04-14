using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum Mode
    {
        Confined = 0,
        AllRange = 1
    }

    public Mode mode = Mode.Confined;
    [Header("Camera")]
    public Camera attachedCamera;
    public float cameraYSpeed = 20f;
    [Header("Controller")]
    public float tiltAmount = .3f;
    public float resolveSpeed = 5f;
    public float movementSpeed = 20f;
    public float rotationSpeed = 20f;

    [Header("Ship")]
    public Transform ship;
    public float shipDistance = 30f;
    public float shipMargin = 15f;
    [Header("Aim Target")]
    public Transform aimTarget;
    public float aimTargetDistance = 20f;
    public float aimTargetMargin = 15f;

    [Header("Debugging")]
    public float boundsDistance = .1f;

    private Vector3 desiredUp = Vector3.up;
    private Transform cameraTransform;

    void OnDrawGizmosSelected()
    {
        attachedCamera.DrawFrustumPlaneAtDistance(shipDistance, shipMargin);
        attachedCamera.DrawFrustumPlaneAtDistance(aimTargetDistance, aimTargetMargin);
    }

    // Use this for initialization
    void Awake()
    {
        shipDistance = Vector3.Distance(transform.position, ship.position);
        aimTargetDistance = Vector3.Distance(transform.position, aimTarget.position);
        
        float distanceToShip = aimTargetDistance - shipDistance;
        aimTarget.position = ship.position + transform.forward * distanceToShip;

        cameraTransform = attachedCamera.transform;
    }

    #region Controls
    void MoveToTarget(float inputH, float inputV)
    {
        // Move Aim Target
        Vector3 direction = new Vector3(inputH, inputV, 0);
        direction = cameraTransform.TransformDirection(direction);
        aimTarget.position += direction * movementSpeed * Time.deltaTime;

        // Move Ship
        ship.position = Vector3.MoveTowards(ship.position, aimTarget.position, movementSpeed * Time.deltaTime);

        // Filter Ship Position
        ship.position = attachedCamera.PositionInBounds(ship.position, shipDistance, shipMargin);

        // Is there any input?
        if (inputH != 0 || inputV != 0)
        {
            // Filter Aim Target Position
            aimTarget.position = attachedCamera.PositionInBounds(aimTarget.position, aimTargetDistance, aimTargetMargin);
        }
        else
        {
            float distanceToShip = aimTargetDistance - shipDistance;
            // Move aim target back in front of ship
            Vector3 desiredPos = ship.position + cameraTransform.forward * distanceToShip;
            aimTarget.position = Vector3.MoveTowards(aimTarget.position, desiredPos, resolveSpeed * Time.deltaTime);
        }
    }

    void RotateToTarget(float inputH)
    {
        desiredUp = new Vector2(tiltAmount * inputH, 1f);
        // Rotate Ship
        Vector3 direction = aimTarget.position - ship.position;
        Quaternion rotation = Quaternion.LookRotation(direction.normalized, desiredUp);
        ship.rotation = Quaternion.Lerp(ship.rotation, rotation, rotationSpeed * Time.deltaTime);
    }
    void CameraFollow()
    {
        Vector3 position = cameraTransform.position;
        Quaternion rotation = cameraTransform.rotation;

        Vector3 shipLocation = ship.position;
        Vector3 currLocation = cameraTransform.position;

        Vector2 shipOffset = aimTarget.localPosition - ship.localPosition;

        rotation *= Quaternion.AngleAxis(shipOffset.x * rotationSpeed * Time.deltaTime, Vector3.up);
        position.y += shipOffset.y * cameraYSpeed * Time.deltaTime;

        cameraTransform.position = position;
        cameraTransform.rotation = rotation;
    }
    #endregion

    #region Actions
    void Movement()
    {
        float inputH = Input.GetAxisRaw("Horizontal");
        float inputV = Input.GetAxisRaw("Vertical");
        MoveToTarget(inputH, inputV);
        RotateToTarget(inputH);
    }
    void Shooting()
    {

    }
    #endregion


    // Update is called once per frame
    void Update()
    {
        Movement();
        if(mode == Mode.AllRange)
        {
            CameraFollow();
        }
        Shooting();
    }
}
