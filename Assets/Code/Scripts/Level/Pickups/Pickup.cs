using System;
using UnityEngine;

namespace RuckusReloaded.Runtime.Level.Pickups
{
    public class Pickup : MonoBehaviour
    {
        private Transform hover;

        private void Awake()
        {
            hover = transform.Find("Hover");
        }

        private void Update()
        {
            hover.localPosition = Vector3.up * (1.3f + Mathf.Sin(Time.time * 2.0f) * 0.15f);
            hover.localRotation = Quaternion.Euler(0.0f, Time.time * 25.0f, 0.0f);
        }
    }
}
