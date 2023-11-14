using System;
using RuckusReloaded.Runtime.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RuckusReloaded.Runtime.Player
{
    public class PlayerController : MonoBehaviour
    {
        public InputActionAsset inputAsset;
        public float mouseSensitivity = 0.3f;
        public float moveSpeed = 12.0f;
        public float accelerationTime = 0.1f;
        public float jumpHeight = 2.2f;

        [Range(0.0f, 1.0f)]
        public float airborneAccelerationPenalty = 0.6f;

        public float upGravity = 2.0f;
        public float downGravity = 3.0f;
        public float stepHeight = 0.6f;

        [Range(0.0f, 1.0f)]
        public float heightSmoothing = 0.4f;

        private Camera mainCam;
        private Vector2 cameraRotation;
        private Rigidbody body;

        private Transform view;
        private RaycastHit groundHit;
        private float height;

        public InputAction MoveAction { get; private set; }
        public InputAction JumpAction { get; private set; }
        public InputAction ShootAction { get; private set; }
        
        public bool IsOnGround { get; private set; }
        public float Movement
        {
            get
            {
                var v = body.velocity;
                return Mathf.Sqrt(v.x * v.x + v.z * v.z) / moveSpeed;
            }
        }

        private bool jumpFlag;

        private Vector3 Gravity => Physics.gravity * (JumpAction.IsPressed() && body.velocity.y > 0.0f ? upGravity : downGravity);

        private void Awake()
        {
            mainCam = Camera.main;
            body = gameObject.GetOrAddComponent<Rigidbody>();

            MoveAction = inputAsset.FindAction("Move");
            JumpAction = inputAsset.FindAction("Jump");
            ShootAction = inputAsset.FindAction("Shoot");

            view = transform.Find("View");
        }

        private void OnEnable()
        {
            inputAsset.Enable();
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnDisable()
        {
            inputAsset.Disable();
            Cursor.lockState = CursorLockMode.None;
        }

        private void FixedUpdate()
        {
            LookForGround();
            Move();
            Jump();
            ApplyGravity();
            ResetFlags();
        }

        private void ApplyGravity()
        {
            if (!body.useGravity) return;
            body.AddForce(Gravity - Physics.gravity, ForceMode.Acceleration);
        }

        private void ResetFlags()
        {
            jumpFlag = false;
        }

        private void Jump()
        {
            if (!IsOnGround) return;
            if (!jumpFlag) return;

            var force = Mathf.Sqrt(2.0f * -Physics.gravity.y * upGravity * jumpHeight);
            body.AddForce(Vector3.up * force, ForceMode.VelocityChange);
        }

        private void LookForGround()
        {
            var wasOnGround = IsOnGround;
            var ray = new Ray(body.position + Vector3.up, Vector3.down);
            var castDistance = wasOnGround ? 1.0f + stepHeight : 1.0f;
            IsOnGround = Physics.Raycast(ray, out groundHit, castDistance) && body.velocity.y < float.Epsilon;


            if (!IsOnGround) height = body.position.y;
            else height = Mathf.Lerp(height, groundHit.point.y, heightSmoothing);

            if (!IsOnGround) return;

            body.position = new Vector3(body.position.x, height, body.position.z);
            body.velocity = new Vector3(body.velocity.x, Mathf.Max(body.velocity.y, 0.0f), body.velocity.z);

            if (groundHit.rigidbody) body.position += groundHit.rigidbody.velocity * Time.deltaTime;
        }

        private void Move()
        {
            var input = MoveAction.ReadValue<Vector2>();
            var target = transform.TransformDirection(input.x, 0.0f, input.y) * moveSpeed;

            var acceleration = 2.0f / accelerationTime;
            if (!IsOnGround) acceleration *= 1.0f - airborneAccelerationPenalty;
            var force = (target - body.velocity) * acceleration;
            force.y = 0.0f;

            body.AddForce(force, ForceMode.Acceleration);
        }

        private void Update()
        {
            var delta = Vector2.zero;
            delta += Mouse.current.delta.ReadValue() * mouseSensitivity;
            cameraRotation += delta;

            cameraRotation.x %= 360.0f;
            cameraRotation.y = Mathf.Clamp(cameraRotation.y, -90.0f, 90.0f);

            transform.rotation = Quaternion.Euler(0.0f, cameraRotation.x, 0.0f);

            view.position = transform.position + Vector3.up * 1.8f;
            view.rotation = Quaternion.Euler(-cameraRotation.y, cameraRotation.x, 0.0f);

            mainCam.transform.position = view.position;
            mainCam.transform.rotation = view.rotation;

            if (JumpAction.WasPressedThisFrame()) jumpFlag = true;
        }
    }
}