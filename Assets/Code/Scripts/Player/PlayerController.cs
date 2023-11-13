using System;
using RuckusReloaded.Runtime.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RuckusReloaded.Runtime.Player
{
    public class PlayerController : MonoBehaviour
    {
        public InputActionAsset inputAsset;
        public float moveSpeed = 12.0f;
        public float accelerationTime = 0.1f;
        public float jumpHeight = 2.2f;
        [Range(0.0f, 1.0f)]
        public float airborneAccelerationPenalty = 0.6f;

        private Camera mainCam;
        private Vector2 cameraRotation;
        private Rigidbody body;

        private bool isOnGround;
        private RaycastHit groundHit;

        private InputAction moveAction;

        private void Awake()
        {
            mainCam = Camera.main;
            body = gameObject.GetOrAddComponent<Rigidbody>();

            moveAction = inputAsset.FindAction("Move");
        }

        private void OnEnable()
        {
            inputAsset.Enable();
        }

        private void OnDisable()
        {
            inputAsset.Disable();
        }

        private void FixedUpdate()
        {
            LookForGround();
            Move();
        }

        private void LookForGround()
        {
            
        }

        private void Move()
        {
            var input = moveAction.ReadValue<Vector2>();
            var target = transform.TransformDirection(input.x, 0.0f, input.y) * moveSpeed;

            var acceleration = 2.0f / accelerationTime;
            if (isOnGround) acceleration *= 1.0f - airborneAccelerationPenalty;
            var force = (target - body.velocity) * acceleration * Time.deltaTime;
            force.y = 0.0f;
            
            body.AddForce(force, ForceMode.Acceleration);
        }

        private void Update()
        {
            cameraRotation.x %= 360.0f;
            cameraRotation.y = Mathf.Clamp(cameraRotation.y, -90.0f, 90.0f);
            
            transform.rotation = Quaternion.Euler(0.0f, cameraRotation.x, 0.0f);
            
            mainCam.transform.position = transform.position + Vector3.up * 1.8f;
            mainCam.transform.rotation = Quaternion.Euler(-cameraRotation.y, cameraRotation.x, 0.0f);
        }
    }
}
