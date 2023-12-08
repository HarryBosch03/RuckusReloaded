using System;
using System.Collections.Generic;
using RuckusReloaded.Runtime.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RuckusReloaded.Runtime.Player
{
    [RequireComponent(typeof(PlayerMovement))]
    public class PlayerController : MonoBehaviour, IPersonality
    {
        public InputActionAsset inputAsset;
        public float mouseSensitivity = 0.3f;

        private Camera mainCam;
        private bool jumpFlag;

        public InputAction MoveAction { get; private set; }
        public InputAction JumpAction { get; private set; }
        public InputAction ShootAction { get; private set; }
        public PlayerMovement Biped { get; private set; }
        public Vector3 LookTarget => Biped.Center;

        public static readonly List<PlayerController> All = new();

        private void Awake()
        {
            mainCam = Camera.main;
            Biped = GetComponent<PlayerMovement>();

            MoveAction = inputAsset.FindAction("Move");
            JumpAction = inputAsset.FindAction("Jump");
            ShootAction = inputAsset.FindAction("Shoot");
        }

        private void OnEnable()
        {
            inputAsset.Enable();
            Cursor.lockState = CursorLockMode.Locked;
            
            All.Add(this);
        }

        private void OnDisable()
        {
            inputAsset.Disable();
            Cursor.lockState = CursorLockMode.None;
            
            All.Remove(this);
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