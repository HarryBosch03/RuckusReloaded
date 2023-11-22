using System;
using RuckusReloaded.Runtime.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RuckusReloaded.Runtime.Player
{
    [RequireComponent(typeof(BipedController))]
    public class PlayerController : MonoBehaviour
    {
        public InputActionAsset inputAsset;
        public float mouseSensitivity = 0.3f;

        private Camera mainCam;
        private float height;
        private bool jumpFlag;

        public InputAction MoveAction { get; private set; }
        public InputAction JumpAction { get; private set; }
        public InputAction ShootAction { get; private set; }
        public BipedController Biped { get; private set; }
        
        private void Awake()
        {
            mainCam = Camera.main;
            Biped = GetComponent<BipedController>();

            MoveAction = inputAsset.FindAction("Move");
            JumpAction = inputAsset.FindAction("Jump");
            ShootAction = inputAsset.FindAction("Shoot");
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
            var moveInput = MoveAction.ReadValue<Vector2>();
            Biped.moveInput = transform.TransformDirection(moveInput.x, 0.0f, moveInput.y);
            
            Biped.jump = jumpFlag;
            jumpFlag = false;
        }

        private void Update()
        {
            var delta = Vector2.zero;
            delta += Mouse.current.delta.ReadValue() * mouseSensitivity * Mathf.Min(1.0f, Time.timeScale);
            Biped.viewRotation += delta;

            if (JumpAction.WasPressedThisFrame()) jumpFlag = true;
        }

        private void LateUpdate()
        {
            mainCam.transform.position = Biped.view.position;
            mainCam.transform.rotation = Biped.view.rotation;
        }
    }
}