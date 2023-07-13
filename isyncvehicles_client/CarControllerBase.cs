using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script responsible for the following:
// - Headless controller of vehicle
public class CarControllerBase : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    public GameObject localPlayer;
    public VehicleBase vb;
    public float horizontalInput;
    public float verticalInput;
    private float currentSteerAngle;
    private float currentbreakForce;
    public bool isBreaking;

    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle;
    [SerializeField] private Vector3 gravityOffset;

    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [SerializeField] private int brakeMaterialIndex;
    private Material brakeMaterial = null;
    [SerializeField] private Color brakeLightEmissionColor;
    [SerializeField] private float brakeLightEmissionIntensity;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheeTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = gravityOffset;

        if(gameObject.GetComponent<MeshRenderer>())
        {
            brakeMaterial = gameObject.GetComponent<MeshRenderer>().materials[brakeMaterialIndex];
        }
    }

    private void FixedUpdate()
    {
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        GetInput();
    }

    private void GetInput()
    {
        if(vb.isVehiclePilot(localPlayer))
        {
            horizontalInput = Input.GetAxis(HORIZONTAL);
            verticalInput = Input.GetAxis(VERTICAL);
            isBreaking = Input.GetKey(KeyCode.Space);
        }
    }

    private void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce;
        rearLeftWheelCollider.motorTorque = verticalInput * motorForce;
        rearRightWheelCollider.motorTorque = verticalInput * motorForce;
        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();       
    }

    private void ApplyBreaking()
    {
        if(brakeMaterial != null){
            if(isBreaking){
                brakeMaterial.SetVector("_EmissionColor", brakeLightEmissionColor * brakeLightEmissionIntensity);
            } else {
                brakeMaterial.SetVector("_EmissionColor", brakeLightEmissionColor * 0);
            }
        }

        vb.sendBrake(isBreaking);
        
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

        rearLeftWheelCollider.steerAngle = -currentSteerAngle;
        rearRightWheelCollider.steerAngle = -currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheeTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }
}