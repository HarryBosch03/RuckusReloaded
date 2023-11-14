using System;
using UnityEngine;

namespace RuckusReloaded.Runtime.Player
{
    public class WeaponSway : MonoBehaviour
    {
        public float amplitude;
        public Vector3 offset;

        private Quaternion rotation = Quaternion.identity;

        private void LateUpdate()
        {
            if (!transform.parent)
            {
                enabled = false;
                return;
            }

            rotation = Quaternion.Slerp(rotation, transform.parent.rotation, Time.deltaTime * amplitude);
            transform.rotation = rotation * Quaternion.Euler(offset);
        }
    }
}