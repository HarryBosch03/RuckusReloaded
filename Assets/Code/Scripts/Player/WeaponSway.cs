using System;
using UnityEngine;

namespace RuckusReloaded.Runtime.Player
{
    public class WeaponSway : MonoBehaviour
    {
        public float translationLag = 1.0f;
        public float rotationLag = 1.0f;
        public float smoothing = 1.0f;
        public Vector3 originOffset;

        private Transform mainCam;
        private Vector3 lastPosition;
        private Vector2 lastRotation;

        private Vector2 smoothedRotationalOffset;
        private Vector3 smoothedTranslationalOffset;

        private void OnEnable()
        {
            mainCam = Camera.main.transform;
        }

        private void FixedUpdate()
        {
            UpdateTranslationLag();
        }

        private void Update()
        {
            UpdateRotationLag();

            ApplyLag();
        }

        private void UpdateTranslationLag()
        {
            if (Time.deltaTime < float.Epsilon) return;
            var position = mainCam.position;
            var velocity = (position - lastPosition) / Time.deltaTime;

            smoothedTranslationalOffset = Vector3.Lerp(smoothedTranslationalOffset, velocity, Time.deltaTime / Mathf.Max(Time.deltaTime, smoothing));

            lastPosition = position;
        }

        private void ApplyLag()
        {
            transform.localRotation = Quaternion.Euler(-smoothedRotationalOffset.y * rotationLag, smoothedRotationalOffset.x * rotationLag, 0.0f);

            transform.localPosition = originOffset - transform.localRotation * originOffset;
            transform.position += smoothedTranslationalOffset * translationLag;
        }

        private void UpdateRotationLag()
        {
            if (Time.deltaTime < float.Epsilon) return;

            var rotation = new Vector2(-mainCam.eulerAngles.y, mainCam.eulerAngles.x);
            var delta = new Vector2
            {
                x = Mathf.DeltaAngle(lastRotation.x, rotation.x),
                y = Mathf.DeltaAngle(lastRotation.y, rotation.y),
            } / Time.deltaTime;

            var offset = delta;
            smoothedRotationalOffset = Vector2.Lerp(smoothedRotationalOffset, offset, Time.deltaTime / Mathf.Max(Time.deltaTime, smoothing));
            lastRotation = rotation;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(originOffset, 0.06f);
        }
    }
}