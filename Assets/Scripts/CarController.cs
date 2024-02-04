using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CarController : MonoBehaviour
{
    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentbreakForce;
    private bool isBreaking;

    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] Transform firstPersonLookPoint;
    [SerializeField ] Transform ThirdPersonLookPoint;

    [SerializeField] private Vector3 firstPersonOffset;
    [SerializeField] private Vector3 thirdPersonOffset;
    private bool isFirstPersonView = false;

    // Settings
    [SerializeField] private float motorForce, breakForce, maxSteerAngle;

    // Wheel Colliders
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    // Wheels
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;

    // Center of Mass
    [SerializeField] private Transform centerOfMass;

    // Start is called before the first frame update
    void Start()
    {
        // Adjust the center of mass
        if (centerOfMass)
        {
            GetComponent<Rigidbody>().centerOfMass = centerOfMass.localPosition;
        }
        
    }
    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        CameronView();
    }

    private void GetInput()
    {
        // Steering Input
        horizontalInput = Input.GetAxis("Horizontal");

        // Acceleration Input
        verticalInput = Input.GetAxis("Vertical");

        // Breaking Input
        isBreaking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce;
        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();
    }

    private void ApplyBreaking()
    {
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform, true);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform, false);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform, false);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform, true);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform, bool isLeft)
    {
       
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);

 

        if (isLeft )
            rot *= Quaternion.Euler(0, 0, -90);
        else
            rot *= Quaternion.Euler(0, 0, 90);


        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    private void CameronView()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isFirstPersonView = !isFirstPersonView;
        }

        if (isFirstPersonView)
        {
            virtualCamera.LookAt = firstPersonLookPoint;
            virtualCamera.Follow = firstPersonLookPoint;
            // Set Cinemachine Virtual Camera body follow offset for first person
            CinemachineTransposer transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer)
            {
                transposer.m_FollowOffset = firstPersonOffset;
            }
        }
        else
        {
            virtualCamera.LookAt = ThirdPersonLookPoint;
            virtualCamera.Follow = ThirdPersonLookPoint;
            // Set Cinemachine Virtual Camera body follow offset for third person
            CinemachineTransposer transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer)
            {
                transposer.m_FollowOffset = thirdPersonOffset;
            }
        }
    }
}