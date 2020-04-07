using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Car : MonoBehaviour
{
    public List<AxleInfo> axleInfos; // the information about each individual axle
    public float maxMotorTorque; // maximum torque the motor can apply to wheel
    public float maxSteeringAngle; // maximum steer angle the wheel can have
    [SerializeField] private float maxRpm = 10000;
    [SerializeField] private float maxBrakeTorque = 36000;
    [SerializeField] private float force;
    
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.centerOfMass = Vector3.zero;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void FixedUpdate()
    {
        ApplyDownforce();
        
        // Debug.Log(_rigidbody.velocity.magnitude);

        var motor = maxMotorTorque * -Input.GetAxis("Vertical");
        var steering = maxSteeringAngle * Input.GetAxis("Horizontal");

        foreach (var axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }

            if (motor < 0)
            {
                axleInfo.leftWheel.brakeTorque = 0;
                axleInfo.rightWheel.brakeTorque = 0;
                
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
            else if (motor > 0)
            {
                axleInfo.leftWheel.motorTorque = 0;
                axleInfo.rightWheel.motorTorque = 0;
                
                axleInfo.leftWheel.brakeTorque = maxBrakeTorque;
                axleInfo.rightWheel.brakeTorque = maxBrakeTorque;
            }
            
            UpdateWheelsVisual(axleInfo);

            // Debug.Log("motor " + motor);
            // Debug.Log("motorTorque " + axleInfo.leftWheel.motorTorque);
            // Debug.Log("brakeTorque " + axleInfo.leftWheel.brakeTorque);
            // Debug.Log("rpm " + Mathf.Abs(axleInfo.leftWheel.rpm));
            Debug.Log("steer " + axleInfo.leftWheel.steerAngle);
        }
    }

    private void ApplyDownforce()
    {
        var calcForce = (2 * _rigidbody.mass * _rigidbody.velocity.magnitude) / (1.81f * 1.225f);
        Debug.Log(calcForce);
        _rigidbody.AddForce(Vector3.down * calcForce);
    }

    private void UpdateWheelsVisual(AxleInfo axleInfo)
    {
        axleInfo.leftWheel.GetWorldPose(out var lwPosition, out var lwQuaternion);
        axleInfo.rightWheel.GetWorldPose(out var rwPosition, out var rwQuaternion);

        axleInfo.leftWheelTransform.position = lwPosition;
        axleInfo.leftWheelTransform.rotation = lwQuaternion;
        axleInfo.rightWheelTransform.position = rwPosition;
        axleInfo.rightWheelTransform.rotation = rwQuaternion;
    }
}