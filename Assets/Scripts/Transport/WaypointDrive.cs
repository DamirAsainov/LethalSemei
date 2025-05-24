using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Transports
{
    public class WaypointDrive : MonoBehaviour
    {
        [Header("Waypoints")] 
        [SerializeField] private List<Transform> waypoints;
        [SerializeField] private GameObject cam;

        [Header("Transport Settings")]
        [SerializeField] private float motorTorque = 1200f;
        [SerializeField] private float brakeForce = 2500f;
        [SerializeField] private float maxSteerAngle = 20f;

        [Header("Wheel Colliders")] 
        [SerializeField] private WheelCollider[] driveWheels;
        [SerializeField] private WheelCollider[] steerWheels;

        [Header("Wheel Transforms")] 
        [SerializeField] private Transform[] wheelMeshes;
        [SerializeField] private Transform[] driveWheelMeshes; // длина = driveWheels.Length


        [Header("Drift FX")]
        [SerializeField] private ParticleSystem[] driftParticles;
        [SerializeField] private TrailRenderer[] tireSkidMarks;
        [SerializeField] private float slipThreshold = 0.4f;

        [Header("Audio")]
        [SerializeField] private AudioSource engineSound;
        [SerializeField] private float minPitch = 0.8f;
        [SerializeField] private float maxPitch = 2f;
        [SerializeField] private float pitchSpeedFactor = 0.02f;
        
        public bool finishedDriving = false;
        private int currentWaypointIndex = 0;
        private bool stoppedDriving = false;

        private Rigidbody rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            if (finishedDriving && stoppedDriving)
                return;
            if (finishedDriving)
            {
                StopVehicle();
            }
            Transform target = waypoints[currentWaypointIndex];
            Vector3 localTarget = transform.InverseTransformPoint(target.position);
            float steer = Mathf.Clamp(localTarget.x / localTarget.magnitude, -1f, 1f);
            float steerAngle = steer * maxSteerAngle;

            foreach (WheelCollider wheel in steerWheels)
                wheel.steerAngle = steerAngle;

            float distance = Vector3.Distance(transform.position, target.position);
            bool shouldBrake = distance < 4f;

            foreach (WheelCollider wheel in driveWheels)
            {
                wheel.motorTorque = shouldBrake ? 0f : motorTorque;
                wheel.brakeTorque = shouldBrake ? brakeForce : 0f;
            }

            if (distance < 4f)
                currentWaypointIndex++;

            
            
            UpdateWheelVisuals();
            UpdateEngineSound();
        }

        private void UpdateWheelVisuals()
        {
            for (int i = 0; i < driveWheels.Length; i++)
            {
                WheelCollider wheel = driveWheels[i];
                Vector3 pos;
                Quaternion rot;
                wheel.GetWorldPose(out pos, out rot);
                if (i < wheelMeshes.Length)
                {
                    wheelMeshes[i].position = pos;
                    wheelMeshes[i].rotation = rot;
                }
            }
            for (int i = 0; i < steerWheels.Length; i++)
            {
                WheelCollider collider = steerWheels[i];
                Transform mesh = driveWheelMeshes[i];

                Vector3 pos;
                Quaternion rot;
                collider.GetWorldPose(out pos, out rot);

                mesh.position = pos;
                mesh.rotation = rot;
            }
        }

        

        private void UpdateEngineSound()
        {
            if (engineSound == null || rb == null) return;

            float speed = rb.velocity.magnitude;
            engineSound.pitch = Mathf.Lerp(minPitch, maxPitch, speed * pitchSpeedFactor);
        }

        private void StopVehicle()
        {
            finishedDriving = true;
            stoppedDriving = true;
            cam.SetActive(false);
            Debug.Log("Stop vehicle KAMAZ");
            StartCoroutine(StopVehicleSmoothly());
            

            if (rb != null)
                rb.isKinematic = true;
        }
        private IEnumerator StopVehicleSmoothly()
        {
            float duration = 6f;
            float elapsed = 0f;

            Rigidbody rb = GetComponent<Rigidbody>();

            // Запоминаем начальную скорость
            

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                elapsed += Time.deltaTime;
                yield return null;
            }

            foreach (WheelCollider wheel in driveWheels)
            {
                wheel.motorTorque = 0f;
                wheel.brakeTorque = brakeForce;
            }

            foreach (WheelCollider wheel in steerWheels)
                wheel.steerAngle = 0f;

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }
}
